namespace Pancake.Editor.Finder
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    internal static class FinderMenu
    {
        private const string HIERARCHY_MENU = "GameObject/Pancake/";
        private const string CONTEXT_MENU = "CONTEXT/";

        private const string REFERENCES_FINDER_MENU_NAME = "Find References in Project";
        private const string CONTEXT_COMPONENT_MENU = CONTEXT_MENU + "Component/";
        private const string SCRIPT_REFERENCES_CONTEXT_MENU_NAME = "Finder: Script File References";
        private const string COMPONENT_CONTEXT_SCENE_REFERENCES_MENU_NAME = "Finder: References In Scene";
        private const string HIERARCHY_CONTEXT_SCENE_REFERENCES_MENU_NAME = "Finder/Game Object References In Scene";
        private const string HIERARCHY_CONTEXT_SCENE_REFERENCES_WITH_COMPONENTS_MENU_NAME = "Finder/Game Object && Components References In Scene";
        private const string SCRIPT_REFERENCES_CONTEXT_MENU = CONTEXT_COMPONENT_MENU + SCRIPT_REFERENCES_CONTEXT_MENU_NAME;
        private const string SCENE_REFERENCES_CONTEXT_MENU = CONTEXT_COMPONENT_MENU + COMPONENT_CONTEXT_SCENE_REFERENCES_MENU_NAME;
        private const string SCENE_REFERENCES_HIERARCHY_MENU = HIERARCHY_MENU + HIERARCHY_CONTEXT_SCENE_REFERENCES_MENU_NAME;
        private const string SCENE_REFERENCES_WITH_COMPONENTS_HIERARCHY_MENU = HIERARCHY_MENU + HIERARCHY_CONTEXT_SCENE_REFERENCES_WITH_COMPONENTS_MENU_NAME;
        private const string PROJECT_BROWSER_CONTEXT_START = "Assets/";
        private const string PROJECT_BROWSER_CONTEXT_REFERENCES_FINDER_NAME = "Finder/" + REFERENCES_FINDER_MENU_NAME;
        private const string PROJECT_BROWSER_CONTEXT_REFERENCES_FINDER_NO_HOT_KEY = PROJECT_BROWSER_CONTEXT_START + PROJECT_BROWSER_CONTEXT_REFERENCES_FINDER_NAME;
        private const string PROJECT_BROWSER_CONTEXT_REFERENCES_FINDER = PROJECT_BROWSER_CONTEXT_REFERENCES_FINDER_NO_HOT_KEY + " %#&s";
        private const string MAIN_MENU = "Tools/Pancake/Finder/";

        private static float lastMenuCallTimestamp;

        [MenuItem(MAIN_MENU + "Show #`", false, 900)]
        private static void ShowWindow() { FinderWindow.Create(); }

        [MenuItem(MAIN_MENU + "Find All Assets References %#=", false, 1002)]
        private static void FindAllReferences() { ReferencesFinder.FindAllAssetsReferences(); }

        [MenuItem(PROJECT_BROWSER_CONTEXT_REFERENCES_FINDER, true, 39)]
        public static bool ValidateFindReferences() { return ProjectScopeReferencesFinder.GetSelectedAssets().Length > 0; }

        [MenuItem(PROJECT_BROWSER_CONTEXT_REFERENCES_FINDER, false, 39)]
        public static void FindReferences() { ReferencesFinder.FindSelectedAssetsReferences(); }

        [MenuItem(SCRIPT_REFERENCES_CONTEXT_MENU, true, 144445)]
        public static bool ValidateFindScriptReferences(MenuCommand command)
        {
            var scriptPath = ObjectTools.GetScriptPathFromObject(command.context);
            return !string.IsNullOrEmpty(scriptPath) && Path.GetExtension(scriptPath).ToLower() != ".dll";
        }

        [MenuItem(SCRIPT_REFERENCES_CONTEXT_MENU, false, 144445)]
        public static void FindScriptReferences(MenuCommand command)
        {
            var scriptPath = ObjectTools.GetScriptPathFromObject(command.context);
            ReferencesFinder.FindAssetReferences(scriptPath);
        }

        [MenuItem(SCENE_REFERENCES_CONTEXT_MENU, true, 144444)]
        public static bool ValidateFindComponentReferences(MenuCommand command) { return command.context is Component && !AssetDatabase.Contains(command.context); }

        [MenuItem(SCENE_REFERENCES_CONTEXT_MENU, false, 144444)]
        public static void FindComponentReferences(MenuCommand command)
        {
            HierarchyScopeReferencesFinder.FindComponentReferencesInHierarchy(command.context as Component);
        }

        [MenuItem(SCENE_REFERENCES_HIERARCHY_MENU, false, -100)]
        public static void FindGameObjectReferences()
        {
            if (Time.unscaledTime.Equals(lastMenuCallTimestamp)) return;
            if (Selection.gameObjects.Length == 0) return;

            ReferencesFinder.FindObjectsReferencesInHierarchy(Selection.gameObjects);

            lastMenuCallTimestamp = Time.unscaledTime;
        }

        [MenuItem(SCENE_REFERENCES_WITH_COMPONENTS_HIERARCHY_MENU, false, -99)]
        public static void FindGameObjectWithComponentsReferences()
        {
            if (Time.unscaledTime.Equals(lastMenuCallTimestamp)) return;
            if (Selection.gameObjects.Length == 0) return;

            ReferencesFinder.FindObjectsReferencesInHierarchy(Selection.gameObjects, true);

            lastMenuCallTimestamp = Time.unscaledTime;
        }
    }
}