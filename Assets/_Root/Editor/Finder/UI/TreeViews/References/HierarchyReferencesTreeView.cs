﻿

namespace Pancake.Editor.Finder
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEditor.IMGUI.Controls;
	using UnityEngine;
	using Object = UnityEngine.Object;
	

	internal class HierarchyReferencesTreeView<T> : FinderTreeView<T> where T : HierarchyReferenceItem
	{
		private enum SortOption
		{
			Icon,
			GameObjectName,
			ComponentName,
			PropertyPath,
			ReferencesCount,
		}

		private enum Columns
		{
			Icon,
			GameObject,
			Component,
			Property,
			ReferencesCount
		}

		// count should be equal to columns count
		private readonly SortOption[] _sortOptions =
		{
			SortOption.Icon,
			SortOption.GameObjectName,
			SortOption.ComponentName,
			SortOption.PropertyPath,
			SortOption.ReferencesCount,
		};

		public HierarchyReferencesTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader, TreeModel<T> model) : base(state, multiColumnHeader, model) {}

		public void SelectRow(long objectId, long componentId)
		{
			foreach (var row in rows)
			{
				var rowLocal = (FinderTreeViewItem<T>)row;
				if (rowLocal.data.reference.objectId == objectId && rowLocal.data.reference.componentId == componentId)
				{
					SelectRowInternal(rowLocal);
					break;
				}
			}
		}

		protected override TreeViewItem GetNewTreeViewItemInstance(int id, int depth, string name, T data)
		{
			return new HierarchyReferencesTreeViewItem<T>(id, depth, name, data);
		}

		protected override void PostInit()
		{
			columnIndexForTreeFoldouts = 1;
		}

		protected override int GetDepthIndentation()
		{
			return UIHelpers.EYE_BUTTON_SIZE;
		}

		protected override void SortByMultipleColumns()
		{
			var sortedColumns = multiColumnHeader.state.sortedColumns;

			if (sortedColumns.Length == 0)
				return;

			var myTypes = rootItem.children.Cast<HierarchyReferencesTreeViewItem<T>>();
			var orderedQuery = InitialOrder(myTypes, sortedColumns);
			for (var i = 1; i < sortedColumns.Length; i++)
			{
				var sortOption = _sortOptions[sortedColumns[i]];
				var ascending = multiColumnHeader.IsSortedAscending(sortedColumns[i]);

				switch (sortOption)
				{
					case SortOption.GameObjectName:
						orderedQuery = orderedQuery.ThenBy(l => l.data.name, ascending);
						break;
					case SortOption.ComponentName:
						orderedQuery = orderedQuery.ThenBy(l => l.data.reference.componentName, ascending);
						break;
					case SortOption.PropertyPath:
						orderedQuery = orderedQuery.ThenBy(l => l.data.reference.propertyPath, ascending);
						break;
					case SortOption.ReferencesCount:
						orderedQuery = orderedQuery.ThenBy(l => l.data.ChildrenCount, ascending);
						break;
					case SortOption.Icon:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			rootItem.children = orderedQuery.Cast<TreeViewItem>().ToList();
		}

		private IOrderedEnumerable<HierarchyReferencesTreeViewItem<T>> InitialOrder(IEnumerable<HierarchyReferencesTreeViewItem<T>> myTypes, IList<int> history)
		{
			var sortOption = _sortOptions[history[0]];
			var ascending = multiColumnHeader.IsSortedAscending(history[0]);

			switch (sortOption)
			{
				case SortOption.GameObjectName:
					return myTypes.Order(l => l.data.name, ascending);
				case SortOption.ComponentName:
					return myTypes.Order(l => l.data.reference.componentName, ascending);
				case SortOption.PropertyPath:
					return myTypes.Order(l => l.data.reference.propertyPath, ascending);
				case SortOption.ReferencesCount:
					return myTypes.Order(l => l.data.ChildrenCount, ascending);
				default:
					return myTypes.Order(l => l.data.name, ascending);
			}
		}

		protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
		{
			var objectReferences = DragAndDrop.objectReferences;

			if (objectReferences == null || objectReferences.Length == 0)
			{
				return DragAndDropVisualMode.Rejected;
			}

#if UNITY_2021_1_OR_NEWER
			var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
#else
			var prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
#endif
			
			var validItems = new List<Object>(objectReferences.Length);

			foreach (var reference in objectReferences)
			{
				// reject any objects from assets
				if (AssetDatabase.Contains(reference))
				{
					continue;
				}

				var validObject = false;
				var component = reference as Component;
				var gameObject = reference as GameObject;
				if (component != null)
				{
					gameObject = component.gameObject;
				}
				else if (gameObject == null)
				{
					continue;
				}

				if (gameObject != null)
				{
#if UNITY_2021_1_OR_NEWER
					var temp = UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject);
#else
					var temp = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject);
#endif
					if (prefabStage != null && temp == prefabStage)
					{
						validObject = true;
					}
					else if (gameObject.scene.IsValid())
					{
						validObject = true;
					}
				}

				if (validObject)
				{
					validItems.Add(reference);
				}
			}

			if (validItems.Count == 0)
			{
				return DragAndDropVisualMode.Rejected;
			}

			if (Event.current.type == EventType.DragPerform)
			{
				var alt = Event.current.alt;

				EditorApplication.delayCall += () =>
				{
					ReferencesFinder.FindObjectsReferencesInHierarchy(validItems.ToArray(), !alt);
				};
				DragAndDrop.AcceptDrag();
			}

			return DragAndDropVisualMode.Generic;
		}

		protected override void DrawCell(ref Rect cellRect, FinderTreeViewItem<T> genericItem, int columnValue, RowGUIArgs args)
		{
			base.DrawCell(ref cellRect, genericItem, columnValue, args);

			var column = (Columns)columnValue;
			var item = (HierarchyReferencesTreeViewItem<T>)genericItem;

			switch (column)
			{
				case Columns.Icon:

					if (item.depth == 0)
					{
						if (item.icon != null)
						{
							var iconRect = cellRect;
							iconRect.width = ICON_WIDTH;
							iconRect.height = EditorGUIUtility.singleLineHeight;

							GUI.DrawTexture(iconRect, item.icon, ScaleMode.ScaleToFit);
						}
					}

					break;
				case Columns.GameObject:

					var entryRect = cellRect;
					entryRect.xMin += baseIndent + UIHelpers.EYE_BUTTON_PADDING;
					
					if (item.depth == 1)
					{
						if (item.icon != null)
						{
							var iconRect = entryRect;
							iconRect.xMin -= UIHelpers.EYE_BUTTON_SIZE - UIHelpers.EYE_BUTTON_PADDING;
							iconRect.width = ICON_WIDTH;
							iconRect.x += ICON_PADDING;
							iconRect.height = EditorGUIUtility.singleLineHeight;

							GUI.DrawTexture(iconRect, item.icon, ScaleMode.ScaleToFit);
						}
					}
					else
					{
						/*entryRect.xMin += baseIndent + UIHelpers.EyeButtonPadding;*/
					}
					
					Rect lastRect;
					var eyeButtonRect = entryRect;
					eyeButtonRect.xMin += ICON_PADDING;
					eyeButtonRect.width = UIHelpers.EYE_BUTTON_SIZE;
					eyeButtonRect.height = UIHelpers.EYE_BUTTON_SIZE;
					eyeButtonRect.x += UIHelpers.EYE_BUTTON_PADDING;

					lastRect = eyeButtonRect;

					if (UIHelpers.IconButton(eyeButtonRect, Uniform.ShowIcon))
					{
						ShowItem(item);
					}

					var labelRect = entryRect;
					labelRect.xMin = lastRect.xMax + UIHelpers.EYE_BUTTON_PADDING;

					if (item.data.depth == 0 && !item.data.HasChildren)
					{
						GUI.contentColor = Colors.labelDimmedColor;
					}
					DefaultGUI.Label(labelRect, args.label, args.selected, args.focused);

					GUI.contentColor = Color.white;
					break;
				case Columns.Component:

					var componentName = item.data.reference.componentName;
					if (!string.IsNullOrEmpty(componentName))
					{
						DefaultGUI.Label(cellRect, componentName, args.selected, args.focused);
					}

					break;
				case Columns.Property:

					var propertyPath = item.data.reference.propertyPath;
					if (!string.IsNullOrEmpty(propertyPath))
					{
						DefaultGUI.Label(cellRect, propertyPath, args.selected, args.focused);
					}

					break;
				case Columns.ReferencesCount:

					if (item.depth == 0)
					{
						DefaultGUI.Label(cellRect, item.data.ChildrenCount.ToString(), args.selected, args.focused);
					}

					break;
				default:
					throw new ArgumentOutOfRangeException("column", column, null);
			}
		}

		protected override void ShowItem(TreeViewItem clickedItem)
		{
			var item = (HierarchyReferencesTreeViewItem<T>)clickedItem;
			var target = item.data.Reference;
			var assetPath = item.data.AssetPath;
			
			SelectionTools.RevealAndSelectReferencingEntry(assetPath, target);
		}

		public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState()
		{
			var columns = new[]
			{
				new MultiColumnHeaderState.Column
				{
					headerContent = new GUIContent(string.Empty, "Referenced Object kind: Game Object or Component"),
					headerTextAlignment = TextAlignment.Center,
					canSort = false,
					width = 22,
					minWidth = 22,
					maxWidth = 22,
					autoResize = false,
					allowToggleVisibility = false,
					contextMenuText = "Icon"
				},
				new MultiColumnHeaderState.Column
				{
					headerContent = new GUIContent(EditorTools.NicifyName(Columns.GameObject.ToString()), "Game Object name including full hierarchy path"),
					headerTextAlignment = TextAlignment.Left,
					sortedAscending = true,
					canSort = true,
					sortingArrowAlignment = TextAlignment.Center,
					width = 200,
					minWidth = 200,
					autoResize = true,
					allowToggleVisibility = false
				},
				new MultiColumnHeaderState.Column
				{
					headerContent = new GUIContent(EditorTools.NicifyName(Columns.Component.ToString()), "Component name"),
					headerTextAlignment = TextAlignment.Left,
					sortedAscending = true,
					canSort = true,
					sortingArrowAlignment = TextAlignment.Center,
					width = 100,
					minWidth = 90,
					autoResize = false,
					allowToggleVisibility = true
				},
				new MultiColumnHeaderState.Column
				{
					headerContent = new GUIContent(EditorTools.NicifyName(Columns.Property.ToString()), "Full property path to the referenced item"),
					headerTextAlignment = TextAlignment.Left,
					sortedAscending = true,
					canSort = true,
					sortingArrowAlignment = TextAlignment.Center,
					width = 100,
					minWidth = 90,
					autoResize = false,
					allowToggleVisibility = true
				},
				new MultiColumnHeaderState.Column
				{
					headerContent = new GUIContent("Refs", "Shows how much times object was referenced in the scene"),
					headerTextAlignment = TextAlignment.Left,
					sortedAscending = false,
					canSort = true,
					sortingArrowAlignment = TextAlignment.Center,
					width = 33,
					minWidth = 33,
					maxWidth = 50,
					autoResize = false,
					allowToggleVisibility = true,
				}
			};

			var state = new MultiColumnHeaderState(columns)
			{
				sortedColumns = new[] {1, 4},
				sortedColumnIndex = 4
			};
			return state;
		}
	}
}