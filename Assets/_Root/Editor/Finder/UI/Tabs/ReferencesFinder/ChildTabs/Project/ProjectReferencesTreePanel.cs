
namespace Pancake.Editor.Finder
{
	using System;
	using UnityEditor;
	using UnityEditor.IMGUI.Controls;
	using UnityEngine;

	internal class ProjectReferencesTreePanel
	{
		private ProjectReferenceItem[] _treeElements;
		private TreeModel<ProjectReferenceItem> _treeModel;
		private ProjectReferencesTreeView<ProjectReferenceItem> _treeView;
		private SearchField _searchField;

		private readonly ProjectExactReferencesListPanel _exactReferencesPanel;

		private object _splitterState;

		public ProjectReferencesTreePanel(FinderWindow window)
		{
			_exactReferencesPanel = new ProjectExactReferencesListPanel(window);
		}

		public void Refresh(bool newData)
		{
			if (newData)
			{
				UserSettings.References.projectReferencesTreeViewState = new TreeViewState();
				_treeModel = null;
			}

			if (_treeModel == null)
			{
				UpdateTreeModel();
			}

			_exactReferencesPanel.Refresh(newData);
		}

		public void SelectItemWithPath(string path)
		{
			_treeView.SelectRowWithPath(path);
		}

		private void UpdateTreeModel()
		{
			var firstInit = UserSettings.References.projectReferencesTreeHeaderState == null || UserSettings.References.projectReferencesTreeHeaderState.columns == null || UserSettings.References.projectReferencesTreeHeaderState.columns.Length == 0;
			var headerState = ProjectReferencesTreeView<ProjectReferenceItem>.CreateDefaultMultiColumnHeaderState();
			if (MultiColumnHeaderState.CanOverwriteSerializedFields(UserSettings.References.projectReferencesTreeHeaderState, headerState))
				MultiColumnHeaderState.OverwriteSerializedFields(UserSettings.References.projectReferencesTreeHeaderState, headerState);
			UserSettings.References.projectReferencesTreeHeaderState = headerState;

			var multiColumnHeader = new FinderMultiColumnHeader(headerState);

			if (firstInit)
			{
				UserSettings.References.projectReferencesTreeViewState = new TreeViewState();
			}

			_treeElements = LoadLastTreeElements();
			_treeModel = new TreeModel<ProjectReferenceItem>(_treeElements);
			_treeView = new ProjectReferencesTreeView<ProjectReferenceItem>(UserSettings.References.projectReferencesTreeViewState, multiColumnHeader, _treeModel);
			_treeView.SetSearchString(UserSettings.References.projectTabSearchString);
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
								GUILayout.ExpandHeight(false)), UserSettings.References.projectTabSearchString);
					if (EditorGUI.EndChangeCheck())
					{
						UserSettings.References.projectTabSearchString = searchString;
						_treeView.SetSearchString(searchString);
						_treeView.Reload();
					}

					GUILayout.Space(3);

					GetSplitterState();

					ReflectionTools.BeginVerticalSplit(_splitterState, new GUILayoutOption[0]);

					using (new GUILayout.VerticalScope())
					{
						_treeView.OnGUI(GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true),
							GUILayout.ExpandHeight(true)));
						GUILayout.Space(2f);
					}

					using (new GUILayout.VerticalScope())
					{
						GUILayout.Space(2f);

						using (new GUILayout.VerticalScope(UIHelpers.panelWithoutBackground))
						{
							GUILayout.Label("Exact references", UIHelpers.centeredLabel);
							GUILayout.Space(1f);
						}

						GUILayout.Space(-1f);

						var selected = _treeView.GetSelection();
						if (selected != null && selected.Count > 0)
						{
							var selectedRow = _treeView.GetRow(selected[0]);
							_exactReferencesPanel.Draw(selectedRow);
						}
						else
						{
							_exactReferencesPanel.Draw(null);
						}
					}

					ReflectionTools.EndVerticalSplit();

					SaveSplitterState();
				}

				GUILayout.Space(5);
			}
		}

		private void GetSplitterState()
		{
			if (_splitterState != null)
			{
				return;
			}

			var savedState = UserSettings.References.splitterState;
			object result;

			try
			{
				if (!string.IsNullOrEmpty(savedState))
				{
					result = JsonUtility.FromJson(savedState, ReflectionTools.splitterStateType);
				}
				else
				{
					result = Activator.CreateInstance(ReflectionTools.splitterStateType,
						new [] {100f, 50f},
						new [] {90, 47},
						null);
				}
			}
			catch (Exception e)
			{
				Debug.LogError(Finder.ConstructError("Couldn't create instance of the SplitterState class!\n" + e, ReferencesFinder.ModuleName));
				throw e;
			}

			_splitterState = result;
		}

		private void SaveSplitterState()
		{
			UserSettings.References.splitterState = EditorJsonUtility.ToJson(_splitterState, false);
		}

		public void CollapseAll()
		{
			_treeView.CollapseAll();
		}

		public void ExpandAll()
		{
			_treeView.ExpandAll();
		}

		private ProjectReferenceItem[] LoadLastTreeElements()
		{
			var loaded = SearchResultsStorage.ProjectReferencesSearchResults;
			if (loaded == null || loaded.Length == 0)
			{
				loaded = new ProjectReferenceItem[1];
				loaded[0] = new ProjectReferenceItem { id = 0, depth = -1, name = "root" };
			}
			return loaded;
		}
	}
}