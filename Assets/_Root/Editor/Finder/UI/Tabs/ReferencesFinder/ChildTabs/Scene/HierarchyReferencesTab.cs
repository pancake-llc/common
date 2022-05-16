namespace Pancake.Editor.Finder
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.SceneManagement;


    internal class HierarchyReferencesTab : ReferencesChildTab
    {
        public static ReferencingEntryData AutoSelectHierarchyReference { get; set; }

        protected override string CaptionName { get { return "Hierarchy Objects"; } }

        protected override Texture CaptionIcon { get { return EditorIcons.HierarchyViewIcon; } }

        private readonly HierarchyReferencesTreePanel _treePanel;

        public HierarchyReferencesTab(FinderWindow window)
            : base(window)
        {
            _treePanel = new HierarchyReferencesTreePanel();
        }

        public void DrawLeftColumnHeader()
        {
            var assetPath = SceneManager.GetActiveScene().path;
#if UNITY_2021_1_OR_NEWER
            var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
#else
            var prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
#endif
            
            if (prefabStage != null)
            {
#if UNITY_2020_1_OR_NEWER
                assetPath = prefabStage.assetPath;
#else
				assetPath = prefabStage.prefabAssetPath;
#endif
            }
        }

        public void DrawSettings()
        {
            GUILayout.Space(10);

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(10);
                using (new GUILayout.VerticalScope())
                {
                    EditorGUILayout.HelpBox("Hold 'alt' key while dropping Game Objects to skip their components", MessageType.Info);
                    GUILayout.Space(10);

                    GUILayout.Label("<size=16><b>Settings</b></size>", UIHelpers.richLabel);
                    UIHelpers.Separator();
                    GUILayout.Space(10);

                    UserSettings.References.clearHierarchyResults = GUILayout.Toggle(UserSettings.References.clearHierarchyResults,
                        new GUIContent(@"Clear previous results",
                            "Check to automatically clear last results on any new search.\n" + "Uncheck to add new results to the last results."));

                    GUILayout.Space(10);
                }

                GUILayout.Space(10);
            }
        }

        public void Refresh(bool newData)
        {
            _treePanel.Refresh(newData);

            if (newData)
            {
                if (AutoSelectHierarchyReference != null)
                {
                    EditorApplication.delayCall += () =>
                    {
                        SelectRow(AutoSelectHierarchyReference);
                        AutoSelectHierarchyReference = null;
                    };
                }
            }
        }

        public void DrawRightColumn() { _treePanel.Draw(); }

        public void DrawFooter()
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(10);

                if (SearchResultsStorage.HierarchyReferencesLastSearched.Length == 0)
                {
                    GUI.enabled = false;
                }

                if (UIHelpers.ImageButton("Refresh", "Restarts references search for the previous results.", Uniform.RepeatIcon))
                {
                    if (Event.current.control && Event.current.shift)
                    {
                        ReferencesFinder.debugMode = true;
                        Event.current.Use();
                    }
                    else
                    {
                        ReferencesFinder.debugMode = false;
                    }

                    EditorApplication.delayCall += () =>
                    {
                        var sceneObjects = ObjectTools.GetObjectsFromInstanceIds(SearchResultsStorage.HierarchyReferencesLastSearched);
                        HierarchyScopeReferencesFinder.FindHierarchyObjectsReferences(sceneObjects, null);
                    };
                }

                GUI.enabled = true;

                if (SearchResultsStorage.HierarchyReferencesSearchResults.Length == 0)
                {
                    GUI.enabled = false;
                }

                if (UIHelpers.ImageButton("Collapse all", "Collapses all tree items.", Uniform.CollapseIcon))
                {
                    _treePanel.CollapseAll();
                }

                if (UIHelpers.ImageButton("Expand all", "Expands all tree items.", Uniform.ExpandIcon))
                {
                    _treePanel.ExpandAll();
                }

                if (UIHelpers.ImageButton("Clear results", "Clears results tree and empties cache.", Uniform.ClearIcon))
                {
                    ClearResults();
                }

                GUI.enabled = true;

                GUILayout.Space(10);
            }

            GUILayout.Space(10);
        }

        private void ClearResults()
        {
            SearchResultsStorage.HierarchyReferencesSearchResults = null;
            SearchResultsStorage.HierarchyReferencesLastSearched = null;
            Refresh(true);
        }

        private void SelectRow(ReferencingEntryData reference) { _treePanel.SelectRow(reference.objectId, reference.componentId); }
    }
}