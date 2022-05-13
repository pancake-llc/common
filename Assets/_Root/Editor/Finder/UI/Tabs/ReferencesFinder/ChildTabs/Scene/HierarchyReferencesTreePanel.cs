

namespace Pancake.Editor.Finder
{
	using UnityEditor;
	using UnityEditor.IMGUI.Controls;
	using UnityEngine;

	internal class HierarchyReferencesTreePanel
	{
		private TreeModel<HierarchyReferenceItem> _treeModel;
		private HierarchyReferencesTreeView<HierarchyReferenceItem> _treeView;
		private SearchField _searchField;

		private HierarchyReferenceItem[] _treeElements;

		public void Refresh(bool newData)
		{
			if (newData)
			{
				UserSettings.References.hierarchyReferencesTreeViewState = new TreeViewState();
				_treeModel = null;
			}

			if (_treeModel == null)
			{
				UpdateTreeModel();
			}
		}

		public void SelectRow(long objectId, long componentId)
		{
			_treeView.SelectRow(objectId, componentId);
		}

		private void UpdateTreeModel()
		{
			var savedHeaderState = UserSettings.References.sceneReferencesTreeHeaderState;
			var firstInit = savedHeaderState == null || savedHeaderState.columns == null || savedHeaderState.columns.Length == 0;
			var headerState = HierarchyReferencesTreeView<HierarchyReferenceItem>.CreateDefaultMultiColumnHeaderState();
			if (MultiColumnHeaderState.CanOverwriteSerializedFields(savedHeaderState, headerState))
			{
				MultiColumnHeaderState.OverwriteSerializedFields(savedHeaderState, headerState);
			}
			UserSettings.References.sceneReferencesTreeHeaderState = headerState;

			var multiColumnHeader = new FinderMultiColumnHeader(headerState);

			if (firstInit)
			{
				UserSettings.References.hierarchyReferencesTreeViewState = new TreeViewState();
			}

			_treeElements = LoadLastTreeElements();
			_treeModel = new TreeModel<HierarchyReferenceItem>(_treeElements);
			_treeView = new HierarchyReferencesTreeView<HierarchyReferenceItem>(UserSettings.References.hierarchyReferencesTreeViewState, multiColumnHeader, _treeModel);
			_treeView.SetSearchString(UserSettings.References.sceneTabSearchString);
			_treeView.Reload();

			_searchField = new SearchField();
			_searchField.downOrUpArrowKeyPressed += _treeView.SetFocusAndEnsureSelectedItem;

			if (firstInit)
			{
				multiColumnHeader.ResizeToFit();
			}
		}

		public void Draw()
		{
			GUILayout.Space(3);
			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Space(5);
				using (new GUILayout.VerticalScope())
				{
					EditorGUI.BeginChangeCheck();
					var searchString =
						_searchField.OnGUI(
							GUILayoutUtility.GetRect(0, 0, 20, 20, GUILayout.ExpandWidth(true),
								GUILayout.ExpandHeight(false)), UserSettings.References.sceneTabSearchString);
					if (EditorGUI.EndChangeCheck())
					{
						UserSettings.References.sceneTabSearchString = searchString;
						_treeView.SetSearchString(searchString);
						_treeView.Reload();
					}

					GUILayout.Space(3);

					using (new GUILayout.VerticalScope())
					{
						_treeView.OnGUI(GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true),
							GUILayout.ExpandHeight(true)));
						GUILayout.Space(2f);
					}
				}

				GUILayout.Space(5);
			}
		}

		public void CollapseAll()
		{
			_treeView.CollapseAll();
		}

		public void ExpandAll()
		{
			_treeView.ExpandAll();
		}

		private HierarchyReferenceItem[] LoadLastTreeElements()
		{
			var loaded = SearchResultsStorage.HierarchyReferencesSearchResults;
			if (loaded == null || loaded.Length == 0)
			{
				loaded = new HierarchyReferenceItem[1];
				loaded[0] = new HierarchyReferenceItem { id = 0, depth = -1, name = "root" };
			}
			return loaded;
		}
	}
}