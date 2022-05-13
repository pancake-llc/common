namespace Pancake.Editor.Finder
{
    using System;
    using System.IO;
    using UnityEngine;
    using Object = UnityEngine.Object;

    /// <summary>
    /// Main settings scriptable object.
    /// Make sure to call Save() after changing any settings to make sure changes will persist.
    /// </summary>
    /// All settings in the scriptable object are saved in ProjectSettings folder.
    [Serializable]
    public class ProjectSettings : ScriptableObject
    {
#if !UNITY_2020_1_OR_NEWER
		internal const int UpdateProgressStep = 10;
#endif

        private const string DIRECTORY = "ProjectSettings";
        private const string PATH = DIRECTORY + "/FinderSettings.asset";
        private static ProjectSettings instance;

        [SerializeField] private ReferencesFinderSettings referencesFinderSettings;

        private static ProjectSettings Instance
        {
            get
            {
                if (instance != null) return instance;
                instance = LoadOrCreate();
                return instance;
            }
        }


        /// <summary>
        /// References Finder module settings.
        /// </summary>
        public static ReferencesFinderSettings References
        {
            get
            {
                if (Instance.referencesFinderSettings == null)
                {
                    Instance.referencesFinderSettings = new ReferencesFinderSettings();
                }

                return Instance.referencesFinderSettings;
            }
        }

        /// <summary>
        /// Call to remove all Finder Settings (including personal settings).
        /// </summary>
        public static void Delete()
        {
            instance = null;
            FileTools.DeleteFile(PATH);
            UserSettings.Delete();
        }

        /// <summary>
        /// Call to save any changes in any settings.
        /// </summary>
        public static void Save()
        {
            SaveInstance(Instance);
            UserSettings.Save();
        }

        private static ProjectSettings LoadOrCreate()
        {
            ProjectSettings settings;

            if (!File.Exists(PATH))
            {
                settings = CreateNewSettingsFile();
            }
            else
            {
                settings = LoadInstance();

                if (settings == null)
                {
                    FileTools.DeleteFile(PATH);
                    settings = CreateNewSettingsFile();
                }
            }

            settings.hideFlags = HideFlags.HideAndDontSave;

            return settings;
        }

        private static ProjectSettings CreateNewSettingsFile()
        {
            var settingsInstance = CreateInstance();
            SaveInstance(settingsInstance);
            return settingsInstance;
        }

        private static void SaveInstance(ProjectSettings settingsInstance)
        {
            if (!Directory.Exists(DIRECTORY)) Directory.CreateDirectory(DIRECTORY);

            try
            {
                UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new Object[] {settingsInstance}, PATH, true);
            }
            catch (Exception ex)
            {
                Debug.LogError(Finder.ConstructError("Can't save settings!\n" + ex));
            }
        }

        private static ProjectSettings LoadInstance()
        {
            ProjectSettings settingsInstance;

            try
            {
                settingsInstance = (ProjectSettings) UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(PATH)[0];
            }
            catch (Exception ex)
            {
                Debug.Log(Finder.LogPrefix + "Can't read settings, resetting them to defaults.\nThis is a harmless message in most cases and can be ignored.\n" + ex);
                settingsInstance = null;
            }

            return settingsInstance;
        }

        private static ProjectSettings CreateInstance()
        {
            var newInstance = CreateInstance<ProjectSettings>();
            newInstance.referencesFinderSettings = new ReferencesFinderSettings();
            return newInstance;
        }
    }
}