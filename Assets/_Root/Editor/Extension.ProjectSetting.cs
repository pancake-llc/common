using System;
using System.IO;
using Pancake.Common;
using UnityEngine;

namespace Pancake.Editor
{
    public static partial class UtilEditor
    {
        private const string DEFAULT_PROJECT_SETTING_PATH = "ProjectSettings/{0}.asset";

        /// <summary>
        /// T need has attribute [Serializable]
        /// T need has contructor (use for init data)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class ProjectSetting<T> where T : class, new()
        {
            private T _settings;

            public T Settings
            {
                get
                {
                    if (_settings == null) LoadSetting();

                    return _settings;
                }
                set
                {
                    _settings = value;
                    SaveSetting();
                }
            }

            public void SaveSetting()
            {
                if (!"ProjectSettings".DirectoryExists()) Directory.CreateDirectory("ProjectSettings");

                try
                {
                    File.WriteAllText(string.Format(DEFAULT_PROJECT_SETTING_PATH, nameof(T)), JsonUtility.ToJson(_settings, true));
                }
                catch (Exception e)
                {
                    Debug.LogError($"Unable to save {nameof(T)} to ProjectSettings!\n" + e.Message);
                }
            }

            public void LoadSetting()
            {
                _settings = new T();
                string path = string.Format(DEFAULT_PROJECT_SETTING_PATH, nameof(T));
                if (!path.FileExists()) return;
                string json = File.ReadAllText(path);
                _settings = JsonUtility.FromJson<T>(json);
            }
        }
    }
}