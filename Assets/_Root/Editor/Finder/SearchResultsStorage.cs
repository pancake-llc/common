namespace Pancake.Editor.Finder
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using UnityEditor;
    using UnityEngine;
    using Debug = UnityEngine.Debug;

    internal static class SearchResultsStorage
    {
        private const string Directory = "Temp";

        private const string ProjectReferencesResultsPath = Directory + "/FinderProjectReferencesResults.bin";
        private const string ProjectReferencesLastSearchedPath = Directory + "/FinderProjectReferencesLastSearched.bin";

        private const string SceneReferencesResultsPath = Directory + "/FinderSceneReferencesResults.bin";
        private const string SceneReferencesLastSearchedPath = Directory + "/FinderSceneReferencesLastSearched.bin";

        private static ProjectReferenceItem[] projectReferencesSearchResults;
        private static FilterItem[] projectReferencesLastSearched;

        private static HierarchyReferenceItem[] sceneReferencesSearchResults;
        private static int[] sceneReferencesLastSearched;

        public static void Clear()
        {
            FileTools.DeleteFile(ProjectReferencesResultsPath);
            FileTools.DeleteFile(ProjectReferencesLastSearchedPath);

            FileTools.DeleteFile(SceneReferencesResultsPath);
            FileTools.DeleteFile(SceneReferencesLastSearchedPath);
        }

        public static ProjectReferenceItem[] ProjectReferencesSearchResults
        {
            get
            {
                if (projectReferencesSearchResults == null)
                {
                    projectReferencesSearchResults = LoadItemsFromJson<ProjectReferenceItem>(ProjectReferencesResultsPath);
                }

                return projectReferencesSearchResults;
            }
            set
            {
                projectReferencesSearchResults = value;
                SaveItemsToJson(ProjectReferencesResultsPath, projectReferencesSearchResults);
            }
        }

        public static HierarchyReferenceItem[] HierarchyReferencesSearchResults
        {
            get
            {
                if (sceneReferencesSearchResults == null)
                {
                    sceneReferencesSearchResults = LoadItemsFromJson<HierarchyReferenceItem>(SceneReferencesResultsPath);
                }

                return sceneReferencesSearchResults;
            }
            set
            {
                sceneReferencesSearchResults = value;
                SaveItemsToJson(SceneReferencesResultsPath, sceneReferencesSearchResults);
            }
        }

        public static FilterItem[] ProjectReferencesLastSearched
        {
            get
            {
                if (projectReferencesLastSearched == null)
                {
                    projectReferencesLastSearched = LoadItems<FilterItem>(ProjectReferencesLastSearchedPath);
                }

                return projectReferencesLastSearched;
            }
            set
            {
                projectReferencesLastSearched = value;
                SaveItems(ProjectReferencesLastSearchedPath, projectReferencesLastSearched);
            }
        }

        public static int[] HierarchyReferencesLastSearched
        {
            get
            {
                if (sceneReferencesLastSearched == null)
                {
                    sceneReferencesLastSearched = LoadItems<int>(SceneReferencesLastSearchedPath);
                }

                return sceneReferencesLastSearched;
            }
            set
            {
                sceneReferencesLastSearched = value;
                SaveItems(SceneReferencesLastSearchedPath, sceneReferencesLastSearched);
            }
        }

        private static void SaveItems<T>(string path, T[] items)
        {
            if (items == null)
            {
                items = new T[0];
            }

            if (!System.IO.Directory.Exists(Directory)) System.IO.Directory.CreateDirectory(Directory);

            if (items.Length > 40000)
            {
                EditorUtility.DisplayProgressBar("Finder", "Saving items, please wait...", 0.5f);
            }

            var bf = new BinaryFormatter();
            var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            bf.Serialize(stream, items);
            stream.Close();

            EditorUtility.ClearProgressBar();
        }

        private static T[] LoadItems<T>(string path)
        {
            T[] results = null;

            if (File.Exists(path))
            {
                var bf = new BinaryFormatter();
                var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                if (stream.Length > 500000)
                {
                    EditorUtility.DisplayProgressBar("Finder", "Loading items, please wait...", 0.5f);
                }

                try
                {
                    results = bf.Deserialize(stream) as T[];
                }
                catch (Exception e)
                {
                    Debug.LogWarning(
                        Finder.LogPrefix + "Can't read search results from " + path + "\n" + e);
                }
                finally
                {
                    stream.Close();
                    EditorUtility.ClearProgressBar();
                }

                if (results == null)
                {
                    results = new T[0];
                    FileTools.DeleteFile(path);
                }
            }
            else
            {
                results = new T[0];
            }

            return results;
        }

        private static void SaveItemsToJson<T>(string path, T[] items)
        {
            if (items == null)
            {
                items = new T[0];
            }

            if (!System.IO.Directory.Exists(Directory)) System.IO.Directory.CreateDirectory(Directory);

            if (items.Length > 40000)
            {
                EditorUtility.DisplayProgressBar("Finder", "Saving items, please wait...", 0.5f);
            }

            var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            var streamWriter = new StreamWriter(stream);

            var wrapper = new ItemsWrapper<T> {items = items};

            var toWrite = JsonUtility.ToJson(wrapper);
            streamWriter.Write(toWrite);
            streamWriter.Flush();
            stream.Close();

            EditorUtility.ClearProgressBar();
        }

        private static T[] LoadItemsFromJson<T>(string path)
        {
            T[] results = null;

            if (File.Exists(path))
            {
                var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                if (stream.Length > 500000)
                {
                    EditorUtility.DisplayProgressBar("Finder", "Loading items, please wait...", 0.5f);
                }

                try
                {
                    var streamReader = new StreamReader(stream);
                    var wrapper = JsonUtility.FromJson<ItemsWrapper<T>>(streamReader.ReadToEnd());
                    results = wrapper.items;
                }
                catch (Exception e)
                {
                    Debug.LogWarning(
                        Finder.LogPrefix + "Can't read search results from " + path + "\n" + e);
                }
                finally
                {
                    stream.Close();
                    EditorUtility.ClearProgressBar();
                }

                if (results == null)
                {
                    results = new T[0];
                    FileTools.DeleteFile(path);
                }
            }
            else
            {
                results = new T[0];
            }

            return results;
        }

        [Serializable]
        public class ItemsWrapper<T>
        {
            public T[] items;
        }
    }
}