namespace Pancake.Editor.Finder
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    internal static class AssetsLoader
    {
        private static readonly Dictionary<string, Texture> CachedTextures = new Dictionary<string, Texture>();

        public static Texture GetTexture(string fileName)
        {
            Texture result;

            if (CachedTextures.ContainsKey(fileName))
            {
                result = CachedTextures[fileName];
            }
            else
            {
                result = EditorGUIUtility.FindTexture(fileName);

                if (result == null)
                {
                    Debug.LogError(Finder.LogPrefix + "Some error occurred while looking for image\n" + fileName);
                }
                else
                {
                    CachedTextures[fileName] = result;
                }
            }

            return result;
        }

        public static Texture GetCachedTypeImage(Type type)
        {
            var key = type.ToString();
            if (CachedTextures.ContainsKey(key))
            {
                return CachedTextures[key];
            }

            var texture = EditorGUIUtility.ObjectContent(null, type).image;
            CachedTextures.Add(key, texture);

            return texture;
        }
    }

    internal static class EditorIcons
    {
        public static Texture ErrorSmallIcon => AssetsLoader.GetTexture("console.erroricon.sml");
        public static Texture ErrorIcon => AssetsLoader.GetTexture("console.erroricon");
        public static Texture FolderIcon => AssetsLoader.GetTexture("Folder Icon");
        public static Texture InfoSmallIcon => AssetsLoader.GetTexture("console.infoicon.sml");
        public static Texture InfoIcon => AssetsLoader.GetTexture("console.infoicon");
        public static Texture PrefabIcon => UnityEditorInternal.InternalEditorUtility.FindIconForFile("dummy.prefab");
        public static Texture SceneIcon => UnityEditorInternal.InternalEditorUtility.FindIconForFile("dummy.unity");
        public static Texture ScriptIcon => UnityEditorInternal.InternalEditorUtility.FindIconForFile("dummy.cs");
        public static Texture WarnSmallIcon => AssetsLoader.GetTexture("console.warnicon.sml");
        public static Texture WarnIcon => AssetsLoader.GetTexture("console.warnicon");

        public static Texture HierarchyViewIcon =>
            EditorGUIUtility.isProSkin
                ? AssetsLoader.GetTexture("d_UnityEditor.SceneHierarchyWindow")
                : AssetsLoader.GetTexture("UnityEditor.SceneHierarchyWindow");

        public static Texture ProjectViewIcon => AssetsLoader.GetTexture("Project");

        public static Texture FilterByType => AssetsLoader.GetTexture("FilterByType");
        public static Texture GameObjectIcon => AssetsLoader.GetCachedTypeImage(typeof(GameObject));
    }

    internal static class Colors
    {
        public static Color labelDimmedColor = ChangeColorAlpha(GUI.skin.label.normal.textColor, 150);
        public static Color labelGreenColor = LerpColorToGreen(GUI.skin.label.normal.textColor, 0.3f);
        public static Color labelRedColor = LerpColorToRed(GUI.skin.label.normal.textColor, 0.3f);

        public static Color backgroundGreenTint = EditorGUIUtility.isProSkin ? new Color32(0, 255, 0, 150) : new Color32(0, 255, 0, 30);
        public static Color backgroundRedTint = EditorGUIUtility.isProSkin ? new Color32(255, 0, 0, 150) : new Color32(255, 0, 0, 30);

        private static Color32 LerpColorToRed(Color32 inValue, float greenAmountPercent) { return Color.Lerp(inValue, new Color32(255, 0, 0, 255), greenAmountPercent); }

        private static Color32 LerpColorToGreen(Color32 inValue, float greenAmountPercent)
        {
            return Color.Lerp(inValue, new Color32(0, 255, 0, 255), greenAmountPercent);
        }

        private static Color32 ChangeColorAlpha(Color32 inValue, byte alphaValue)
        {
            inValue.a = alphaValue;
            return inValue;
        }
    }
}