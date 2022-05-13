

namespace Pancake.Editor.Finder
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEditor.IMGUI.Controls;
	using UnityEngine;

	internal class ProjectReferencesTreeView<T> : FinderTreeView<T> where T : ProjectReferenceItem
	{
		private enum SortOption
		{
			AssetPath,
			AssetType,
			AssetSize,
			ReferencesCount,
		}

		private enum Columns
		{
			Path,
			Type,
			Size,
			ReferencesCount
		}

		// count should be equal to columns count
		private readonly SortOption[] _sortOptions =
		{
			SortOption.AssetPath,
			SortOption.AssetType,
			SortOption.AssetSize,
			SortOption.ReferencesCount
		};

		public ProjectReferencesTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader, TreeModel<T> model) : base(state, multiColumnHeader, model) {}

		public void SelectRowWithPath(string path)
		{
			foreach (var row in rows)
			{
				var rowLocal = (FinderTreeViewItem<T>)row;
				if (rowLocal.data.assetPath == path)
				{
					SelectRowInternal(rowLocal);
					break;
				}
			}
		}

		protected override TreeViewItem GetNewTreeViewItemInstance(int id, int depth, string name, T data)
		{
			return new ProjectReferencesTreeViewItem<T>(id, depth, name, data);
		}

		protected override void SortByMultipleColumns()
		{
			var sortedColumns = multiColumnHeader.state.sortedColumns;

			if (sortedColumns.Length == 0)
				return;

			var myTypes = rootItem.children.Cast<ProjectReferencesTreeViewItem<T>>();
			var orderedQuery = InitialOrder(myTypes, sortedColumns);
			for (var i = 1; i < sortedColumns.Length; i++)
			{
				var sortOption = _sortOptions[sortedColumns[i]];
				var ascending = multiColumnHeader.IsSortedAscending(sortedColumns[i]);

				switch (sortOption)
				{
					case SortOption.AssetPath:
						orderedQuery = orderedQuery.ThenBy(l => l.data.assetPath, ascending);
						break;
					case SortOption.AssetType:
						orderedQuery = orderedQuery.ThenBy(l => l.data.assetTypeName, ascending);
						break;
					case SortOption.AssetSize:
						orderedQuery = orderedQuery.ThenBy(l => l.data.assetSize, ascending);
						break;
					case SortOption.ReferencesCount:
						orderedQuery = orderedQuery.ThenBy(l => l.data.ChildrenCount, ascending);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			rootItem.children = orderedQuery.Cast<TreeViewItem>().ToList();
		}

		private IOrderedEnumerable<ProjectReferencesTreeViewItem<T>> InitialOrder(IEnumerable<ProjectReferencesTreeViewItem<T>> myTypes, IList<int> history)
		{
			var sortOption = _sortOptions[history[0]];
			var ascending = multiColumnHeader.IsSortedAscending(history[0]);

			switch (sortOption)
			{
				case SortOption.AssetPath:
					return myTypes.Order(l => l.data.assetPath, ascending);
				case SortOption.AssetType:
					return myTypes.Order(l => l.data.assetTypeName, ascending);
				case SortOption.AssetSize:
					return myTypes.Order(l => l.data.assetSize, ascending);
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

			for (var i = 0; i < objectReferences.Length; i++)
			{
				var monoBehaviour = objectReferences[i] as MonoBehaviour;
				if (monoBehaviour == null) continue;

				var monoScript = MonoScript.FromMonoBehaviour(monoBehaviour);
				if (monoScript == null) continue;

				objectReferences[i] = monoScript;
			}

			var ids = ObjectTools.GetObjectsInstanceIDs(objectReferences);
			var assetsPaths = ProjectScopeReferencesFinder.GetAssetsFromInstances(ids);
			if (assetsPaths.Length == 0)
			{
				return DragAndDropVisualMode.Rejected;
			}

			if (Event.current.type == EventType.DragPerform)
			{
				EditorApplication.delayCall += () => { ReferencesFinder.FindAssetsReferences(assetsPaths.ToArray()); };
				DragAndDrop.AcceptDrag();
			}

			return DragAndDropVisualMode.Generic;
		}

		protected override void DrawCell(ref Rect cellRect, FinderTreeViewItem<T> genericItem, int columnValue, RowGUIArgs args)
		{
			base.DrawCell(ref cellRect, genericItem, columnValue, args);

			var column = (Columns)columnValue;
			var item = (ProjectReferencesTreeViewItem<T>)genericItem;

			switch (column)
			{
				case Columns.Path:

					var entryRect = DrawIconAndGetEntryRect(cellRect, item);

					Rect lastRect;

					var eyeButtonRect = entryRect;
					eyeButtonRect.width = UIHelpers.EYE_BUTTON_SIZE;
					eyeButtonRect.height = UIHelpers.EYE_BUTTON_SIZE;
					eyeButtonRect.x += UIHelpers.EYE_BUTTON_PADDING;

					lastRect = eyeButtonRect;

					if (UIHelpers.IconButton(eyeButtonRect, Uniform.ShowIcon))
					{
						ShowItem(item);
					}

					if (item.depth == 1 && item.data.isReferenced)
					{
						var findButtonRect = entryRect;
						findButtonRect.width = UIHelpers.EYE_BUTTON_SIZE;
						findButtonRect.height = UIHelpers.EYE_BUTTON_SIZE;
						findButtonRect.x += UIHelpers.EYE_BUTTON_PADDING*2 + UIHelpers.EYE_BUTTON_SIZE;

						lastRect = findButtonRect;

						if (UIHelpers.IconButton(findButtonRect, Uniform.FindIcon, "Search for references"))
						{
							EditorApplication.delayCall += ()=>ProjectScopeReferencesFinder.FindAssetReferencesFromResults(item.data.assetPath);
						}
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

				case Columns.Type:

					DefaultGUI.Label(cellRect, item.data.assetTypeName, args.selected, args.focused);
					break;

				case Columns.Size:

					DefaultGUI.Label(cellRect, item.data.assetSizeFormatted, args.selected, args.focused);
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
			var item = (ProjectReferencesTreeViewItem<T>)clickedItem;
			var assetPath = item.data.assetPath;
			if (item.data.assetSettingsKind == AssetSettingsKind.NotSettings)
			{
				if (!SelectionTools.RevealAndSelectFileAsset(assetPath))
				{
					FinderWindow.ShowNotification("Can't show it properly");
				}
			}
			else
			{
				if (!EditorTools.RevealInSettings(item.data.assetSettingsKind, assetPath))
				{
					FinderWindow.ShowNotification("Can't show it properly");
				}
			}
		}

		public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState()
		{
			var columns = new[]
			{
				new MultiColumnHeaderState.Column
				{
					headerContent = new GUIContent("Path", "Paths to the assets"),
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
					headerContent = new GUIContent("Type", EditorIcons.FilterByType, "Assets types"),
					headerTextAlignment = TextAlignment.Left,
					sortedAscending = true,
					canSort = true,
					sortingArrowAlignment = TextAlignment.Center,
					width = 100,
					minWidth = 70,
					autoResize = false,
					allowToggleVisibility = true
				},
				new MultiColumnHeaderState.Column
				{
					headerContent = new GUIContent("Size", "Assets sizes"),
					headerTextAlignment = TextAlignment.Left,
					sortedAscending = false,
					canSort = true,
					sortingArrowAlignment = TextAlignment.Center,
					width = 100,
					minWidth = 70,
					autoResize = false,
					allowToggleVisibility = true
				},
				new MultiColumnHeaderState.Column
				{
					headerContent = new GUIContent("Refs", "Shows how much times asset was referenced somewhere"),
					headerTextAlignment = TextAlignment.Left,
					sortedAscending = false,
					canSort = true,
					sortingArrowAlignment = TextAlignment.Center,
					width = 33,
					minWidth = 33,
					maxWidth = 50,
					autoResize = false,
					allowToggleVisibility = true
				},
			};

			var state = new MultiColumnHeaderState(columns)
			{
				sortedColumns = new[] {0, 3},
				sortedColumnIndex = 3
			};
			return state;
		}
	}
}