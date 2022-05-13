namespace Pancake.Editor.Finder
{
    using System;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    /// <summary>
    /// References Finder module settings saved in Library or UserSettings (since Unity 2020.1) folder.
    /// </summary>
    /// Contains user-specific settings for this module.
    /// See IDE hints for all list.
    [Serializable]
    public class ReferencesFinderPersonalSettings
    {
        public bool showAssetsWithoutReferences;

        public bool selectedFindClearsProjectResults;
        public bool clearHierarchyResults;

        public bool fullProjectScanWarningShown;
        public string projectTabSearchString;
        public string sceneTabSearchString;

        [SerializeField] internal TreeViewState projectReferencesTreeViewState;
        [SerializeField] internal MultiColumnHeaderState projectReferencesTreeHeaderState;
        [SerializeField] internal TreeViewState hierarchyReferencesTreeViewState;
        [SerializeField] internal MultiColumnHeaderState sceneReferencesTreeHeaderState;
        [SerializeField] internal string splitterState;

        public ReferenceFinderTab selectedTab;
    }
}