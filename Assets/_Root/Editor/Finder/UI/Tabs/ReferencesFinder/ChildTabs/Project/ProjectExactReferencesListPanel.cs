
namespace Pancake.Editor.Finder
{
	using UnityEditor.IMGUI.Controls;
	using UnityEngine;

	internal class ProjectExactReferencesListPanel
	{
		private HierarchyReferenceItem[] _listElements;
		private TreeModel<HierarchyReferenceItem> _listModel;
		private ExactReferencesList<HierarchyReferenceItem> _list;

		private FinderTreeViewItem<ProjectReferenceItem> _lastSelectedRow;

		internal ProjectExactReferencesListPanel(FinderWindow window)
		{
		}

		internal void Refresh(bool newData)
		{
			if (newData)
			{
				_listModel = null;
			}

			if (_listModel == null && _lastSelectedRow != null)
			{
				UpdateTreeModel();
			}
		}

		internal virtual void Draw(FinderTreeViewItem<ProjectReferenceItem> selectedRow)
		{
				if (selectedRow == null)
				{
					DrawRow("Please select any child item above to look for exact references location.");
					return;
				}

				if (selectedRow.data == null)
				{
					DrawRow("Selected item has no exact references support.");
					return;
				}

				var entries = selectedRow.data.referencingEntries;

				if (entries == null || entries.Length == 0)
				{
					if (selectedRow.data.depth == 0)
					{
						DrawRow("Please select any child item above to look for exact references location.");
						return;
					}

					DrawRow("Selected item has no exact references support.");
					return;
				}

				if (_lastSelectedRow != selectedRow)
				{
					_lastSelectedRow = selectedRow;
					UpdateTreeModel();
				}

				DrawReferencesPanel();
		}

		private void DrawRow(string label)
		{
			_lastSelectedRow = new ListTreeViewItem<ProjectReferenceItem>(0, 0, label, null)
			{
				depth = 0,
				id = 1
			};
			UpdateTreeModel();
			DrawReferencesPanel();
		}

		private void DrawReferencesPanel()
		{
			_list.OnGUI(GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)));
		}

		private void UpdateTreeModel()
		{
			_listElements = GetTreeElementsFromRow(_lastSelectedRow);
			_listModel = new TreeModel<HierarchyReferenceItem>(_listElements);
			_list = new ExactReferencesList<HierarchyReferenceItem>(new TreeViewState(), _listModel);
			_list.Reload();
		}

		private HierarchyReferenceItem[] GetTreeElementsFromRow(FinderTreeViewItem<ProjectReferenceItem> item)
		{
			var data = item.data;
			var entries = data != null ? data.referencingEntries : null;

			int count;
			if (entries != null && entries.Length > 0)
			{
				count = entries.Length + 1;
			}
			else
			{
				count = 2;
			}

			var result = new HierarchyReferenceItem[count];
			result[0] = new HierarchyReferenceItem
			{
				id = 0,
				name = "root",
				depth = -1
			};

			if (entries == null || entries.Length == 0)
			{
				result[1] = new HierarchyReferenceItem
				{
					id = 1,
					reference = null,
					name = item.displayName
				};

				return result;
			}

			for (var i = 0; i < entries.Length; i++)
			{
				var entry = entries[i];
				var newItem = new HierarchyReferenceItem
				{
					id = i + 1,
					reference = entry
				};
				newItem.SetAssetPath(item.data.assetPath);
				result[i + 1] = newItem;
			}

			return result;
		}
	}
}