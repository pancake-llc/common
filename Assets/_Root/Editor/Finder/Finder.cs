namespace Pancake.Editor.Finder
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Entry point class with few common APIs.
    /// </summary>
    public static class Finder
    {
        internal const string LogPrefix = "<b>[Finder]</b> ";
        private static string directory;

        /// <summary>
        /// Path to the Finder Directory in your project or packages.
        /// </summary>
        public static string Directory
        {
            get
            {
                if (!string.IsNullOrEmpty(directory)) return directory;

                directory = FinderMarker.GetAssetPath();

                if (!string.IsNullOrEmpty(directory))
                {
                    if (directory.IndexOf("Scripts/FinderMarker.cs", StringComparison.Ordinal) >= 0)
                    {
                        directory = directory.Replace("Scripts/FinderMarker.cs", "");
                    }
                    else
                    {
                        directory = null;
                        Debug.LogError(ConstructError("Looks like Finder is placed in project incorrectly!"));
                    }
                }
                else
                {
                    directory = null;
                    Debug.LogError(ConstructError("Can't locate the Finder directory!"));
                }

                return directory;
            }
        }

        internal static string ConstructError(string errorText, string moduleName = null)
        {
            return LogPrefix + (string.IsNullOrEmpty(moduleName) ? "" : moduleName + ": ") + errorText;
        }

        internal static string ConstructWarning(string warningText, string moduleName = null)
        {
            return LogPrefix + (string.IsNullOrEmpty(moduleName) ? "" : moduleName + ": ") + warningText;
        }
    }
}