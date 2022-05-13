#region copyright

//---------------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov - focus [https://codestage.net]
//---------------------------------------------------------------

#endregion

#pragma warning disable 0414

namespace Pancake.Editor.Finder
{
    using System;
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// All user-specific settings saved in Library or UserSettings (since Unity 2020.1) folder.
    /// Make sure to call Save() after changing settings to make sure changes will persist.
    /// </summary>
    [Serializable]
    public class UserSettings : ScriptableObject
    {
#if UNITY_2020_1_OR_NEWER
        private const string DIRECTORY = "UserSettings";
#else
		private const string DIRECTORY = "Library";
#endif
        private const string PATH = DIRECTORY + "/FinderUserSettings.asset";
        private static UserSettings instance;

        public ReferencesFinderPersonalSettings referencesFinder;
        [SerializeField] private ReferencesFinderPersonalSettings referencesFinderSettings;
        [SerializeField] internal Vector2 scroll;

        internal static UserSettings Instance
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
        public static ReferencesFinderPersonalSettings References
        {
            get
            {
                if (Instance.referencesFinder == null) Instance.referencesFinder = new ReferencesFinderPersonalSettings();

                return Instance.referencesFinder;
            }
        }

        /// <summary>
        /// Call to remove all personal settings.
        /// </summary>
        public static void Delete()
        {
            instance = null;
            FileTools.DeleteFile(PATH);
        }

        /// <summary>
        /// Call to save any changes in personal settings.
        /// </summary>
        public static void Save() { SaveInstance(Instance); }

        private static UserSettings LoadOrCreate()
        {
            UserSettings settings;

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

            return settings;
        }

        private static UserSettings CreateNewSettingsFile()
        {
            var settingsInstance = CreateInstance();

            SaveInstance(settingsInstance);

            return settingsInstance;
        }

        private static void SaveInstance(UserSettings settingsInstance)
        {
            if (!Directory.Exists(DIRECTORY)) Directory.CreateDirectory(DIRECTORY);

            try
            {
                UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new[] {settingsInstance}, PATH, true);
            }
            catch (Exception ex)
            {
                Debug.LogError(Finder.ConstructError("Can't save personal settings!\n" + ex));
            }
        }

        private static UserSettings LoadInstance()
        {
            UserSettings settingsInstance;

            try
            {
                settingsInstance = (UserSettings) UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(PATH)[0];
            }
            catch (Exception ex)
            {
                Debug.Log(Finder.LogPrefix +
                          "Can't read personal settings, resetting them to defaults.\nThis is a harmless message in most cases and can be ignored.\n" + ex);
                settingsInstance = null;
            }

            return settingsInstance;
        }

        private static UserSettings CreateInstance()
        {
            var newInstance = CreateInstance<UserSettings>();
            newInstance.referencesFinder = new ReferencesFinderPersonalSettings();
            return newInstance;
        }
    }
}