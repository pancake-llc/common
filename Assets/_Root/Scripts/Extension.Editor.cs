#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Client.Common
{
    public static partial class Common
    {
        /// <summary>
        /// Draw a large separator with optional section header
        /// </summary>
        public static bool Section(string header = null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();

            if (header != null)
                EditorGUILayout.LabelField(header,
                    new GUIStyle(EditorStyles.largeLabel) { alignment = TextAnchor.MiddleCenter, fontSize = 20, richText = true },
                    GUILayout.Height(30));

            bool clicked = GUI.Button(GUILayoutUtility.GetLastRect(), GUIContent.none, GUIStyle.none);
            return clicked;
        }

        /// <summary>
        /// Disable groups
        /// </summary>
        public static void DisabledSection(System.Action onGUI = null, System.Func<bool> isDisabled = null)
        {
            EditorGUI.BeginDisabledGroup(isDisabled?.Invoke() ?? true);
            onGUI?.Invoke();
            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// Draw a large separator with optional section header
        /// </summary>
        public static bool SubSection(string header = null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", new GUIStyle(GUI.skin.box) { fixedHeight = 1, stretchWidth = true }, GUILayout.Height(1));
            //EditorGUILayout.Space();

            if (header != null)
                EditorGUILayout.LabelField(header,
                    new GUIStyle(EditorStyles.largeLabel) { alignment = TextAnchor.MiddleCenter, fontSize = 15, richText = true },
                    GUILayout.Height(18));

            bool clicked = GUI.Button(GUILayoutUtility.GetLastRect(), GUIContent.none, GUIStyle.none);
            return clicked;
        }

        /// <summary>
        /// Draw a boxed section for all ...GUI... calls in the callback
        /// </summary>
        public static bool MiniBoxedSection(string header = null, System.Action onGUI = null)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            if (header != null)
                EditorGUILayout.LabelField(header,
                    new GUIStyle(EditorStyles.largeLabel) { alignment = TextAnchor.MiddleCenter, fontSize = 14, richText = true },
                    GUILayout.Height(20));
            bool clicked = GUI.Button(GUILayoutUtility.GetLastRect(), GUIContent.none, GUIStyle.none);
            onGUI?.Invoke();
            EditorGUILayout.EndVertical();
            return clicked;
        }

        /// <summary>
        /// Find all components <typeparamref name="T"/> of root prefab GameObjects
        /// </summary>
        public static List<T> FindAllAssetComponents<T>() where T : Component
        {
            var gos = FindAllAssets<GameObject>();
            return gos.SelectMany(go => go.GetComponents<T>()).ToList();
        }

        /// <summary>
        /// Find all assets of type <typeparamref name="T"/>.
        /// In editor it uses AssetDatabase.
        /// In runtime it uses Resources.FindObjectsOfTypeAll
        /// </summary>
        public static List<T> FindAllAssets<T>() where T : Object
        {
            List<T> l = new List<T>();
#if UNITY_EDITOR
            var typeStr = typeof(T).ToString();
            typeStr = typeStr.Replace("UnityEngine.", "");

            if (typeof(T) == typeof(SceneAsset)) typeStr = "Scene";
            else if (typeof(T) == typeof(GameObject)) typeStr = "gameobject";

            var guids = AssetDatabase.FindAssets("t:" + typeStr);
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                T obj = AssetDatabase.LoadAssetAtPath<T>(path);
                if (obj != null) l.Add(obj);
            }
#else
            l.AddRange(Resources.FindObjectsOfTypeAll<T>());
#endif
            return l;
        }

        public static T[] FindAllAssetsWithPath<T>(string path)
        {
            ArrayList al = new ArrayList();
            string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);

            foreach (string fileName in fileEntries)
            {
                int assetPathIndex = fileName.IndexOf("Assets");
                string localPath = fileName.Substring(assetPathIndex);

                Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));

                if (t != null)
                    al.Add(t);
            }

            T[] result = new T[al.Count];
            for (int i = 0; i < al.Count; i++)
                result[i] = (T)al[i];

            return result;
        }

        public static bool Button(string title, Color? color = null, params GUILayoutOption[] options)
        {
            var c = GUI.color;
            GUI.color = color ?? c;
            var b = GUILayout.Button(new GUIContent(title), new GUIStyle(EditorStyles.toolbarButton) { fontSize = 11, font = EditorStyles.label.font, }, options);
            GUI.color = c;
            return b;
        }
    }
}
#endif