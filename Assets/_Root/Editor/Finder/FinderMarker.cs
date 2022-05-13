namespace Pancake.Editor.Finder
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Use it to guess current directory of the Finder.
    /// </summary>
    public class FinderMarker : ScriptableObject
    {
        /// <summary>
        /// Returns raw path of the FinderMarker script for further reference.
        /// </summary>
        /// <returns>Path of the FinderMarker ScriptableObject asset.</returns>
        public static string GetAssetPath()
        {
            string result;

            var tempInstance = CreateInstance<FinderMarker>();
            var script = MonoScript.FromScriptableObject(tempInstance);
            if (script != null)
            {
                result = AssetDatabase.GetAssetPath(script);
            }
            else
            {
                result = AssetDatabase.FindAssets("FinderMarker")[0];
                result = AssetDatabase.GUIDToAssetPath(result);
            }

            DestroyImmediate(tempInstance);
            return result;
        }
    }
}