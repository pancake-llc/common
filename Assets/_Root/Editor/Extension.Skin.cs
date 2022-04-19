using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Pancake.Editor
{
    public static class Uniform
    {
        private static readonly Dictionary<string, GUIStyle> CustomStyles = new Dictionary<string, GUIStyle>();
        private static GUISkin skin;
        private const string SKIN_PATH = "Assets/_Root/Editor/GUISkins/";
        private const string UPM_SKIN_PATH = "Packages/com.pancake.common/Editor/GUISkins/";
        private static GUIStyle uppercaseSectionHeaderExpand;
        private static GUIStyle uppercaseSectionHeaderCollapse;
        private static GUIStyle toggleButtonToolbar;
        private static Texture2D chevronUp;
        private static Texture2D chevronDown;
        private static Texture2D eraserIcon;
        private static Texture2D pinIcon;
        private static Texture2D extrudeIcon;
        private const int CHEVRON_ICON_WIDTH = 10;
        private const int CHEVRON_ICON_RIGHT_MARGIN = 5;
        private const float SPACE_HALF_LINE = 2f;
        private const float SPACE_ONE_LINE = 4f;
        private const float SPACE_TWO_LINE = 8f;
        private const float SPACE_THREE_LINE = 12f;

        public static UtilEditor.ProjectSetting<UniformFoldoutState> FoldoutSettings { get; set; } = new UtilEditor.ProjectSetting<UniformFoldoutState>();

        public static GUIStyle UppercaseSectionHeaderExpand { get { return uppercaseSectionHeaderExpand ??= GetCustomStyle("Uppercase Section Header"); } }

        public static GUIStyle UppercaseSectionHeaderCollapse
        {
            get { return uppercaseSectionHeaderCollapse ??= new GUIStyle(GetCustomStyle("Uppercase Section Header")) {normal = new GUIStyleState()}; }
        }

        public static GUIStyle ToggleButtonToolbar { get { return toggleButtonToolbar ??= new GUIStyle(GetCustomStyle("ToggleButton")); } }

        public static GUIStyle GetCustomStyle(string styleName)
        {
            if (CustomStyles.ContainsKey(styleName)) return CustomStyles[styleName];

            if (Skin != null)
            {
                var style = Skin.FindStyle(styleName);

                if (style == null) Debug.LogError("Couldn't find style " + styleName);
                else CustomStyles.Add(styleName, style);

                return style;
            }

            return null;
        }

        public static GUISkin Skin
        {
            get
            {
                if (skin != null) return skin;

                const string upmPath = UPM_SKIN_PATH + "Dark.guiskin";
                string path = !File.Exists(Path.GetFullPath(upmPath)) ? SKIN_PATH + "Dark.guiskin" : upmPath;
                skin = AssetDatabase.LoadAssetAtPath(path, typeof(GUISkin)) as GUISkin;

                if (skin == null) Debug.LogError("Couldn't load the GUISkin at " + path);

                return skin;
            }
        }

        public static Texture2D ChevronDown
        {
            get
            {
                if (chevronDown != null) return chevronDown;
                const string upmPath = UPM_SKIN_PATH + "Icons/icon-chevron-down-dark.psd";
                string path = !File.Exists(Path.GetFullPath(upmPath)) ? SKIN_PATH + "Icons/icon-chevron-down-dark.psd" : upmPath;
                chevronDown = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
                return chevronDown;
            }
        }

        public static Texture2D ChevronUp
        {
            get
            {
                if (chevronUp != null) return chevronUp;
                const string upmPath = UPM_SKIN_PATH + "Icons/icon-chevron-up-dark.psd";
                string path = !File.Exists(Path.GetFullPath(upmPath)) ? SKIN_PATH + "Icons/icon-chevron-up-dark.psd" : upmPath;
                chevronUp = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;

                return chevronUp;
            }
        }

        public static Texture2D PinIcon
        {
            get
            {
                if (pinIcon != null) return pinIcon;
                const string upmPath = UPM_SKIN_PATH + "Icons/pin.png";
                string path = !File.Exists(Path.GetFullPath(upmPath)) ? SKIN_PATH + "Icons/pin.png" : upmPath;
                pinIcon = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;

                return pinIcon;
            }
        }

        public static Texture2D ExtrudeIcon
        {
            get
            {
                if (extrudeIcon != null) return extrudeIcon;
                const string upmPath = UPM_SKIN_PATH + "Icons/extrude.png";
                string path = !File.Exists(Path.GetFullPath(upmPath)) ? SKIN_PATH + "Icons/extrude.png" : upmPath;
                extrudeIcon = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;

                return extrudeIcon;
            }
        }

        public static Texture2D EraserIcon
        {
            get
            {
                if (eraserIcon != null) return eraserIcon;
                const string upmPath = UPM_SKIN_PATH + "Icons/eraser.png";
                string path = !File.Exists(Path.GetFullPath(upmPath)) ? SKIN_PATH + "Icons/eraser.png" : upmPath;
                eraserIcon = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;

                return eraserIcon;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="foldout"></param>
        /// <returns></returns>
        public static Texture2D GetChevronIcon(bool foldout) { return foldout ? ChevronUp : ChevronDown; }

        /// <summary>
        /// Draw group selection with header
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sectionName"></param>
        /// <param name="drawer"></param>
        /// <param name="defaultFoldout"></param>
        public static void DrawUppercaseSection(string key, string sectionName, Action drawer, bool defaultFoldout = true)
        {
            if (!FoldoutSettings.Settings.ContainsKey(key)) FoldoutSettings.Settings.Add(key, defaultFoldout);

            bool foldout = FoldoutSettings.Settings[key];

            EditorGUILayout.BeginVertical(GetCustomStyle("Uppercase Section Box"), GUILayout.MinHeight(foldout ? 30 : 0));
            EditorGUILayout.BeginHorizontal(foldout ? UppercaseSectionHeaderExpand : UppercaseSectionHeaderCollapse);

            // Header label (and button).
            if (GUILayout.Button(sectionName, GetCustomStyle("Uppercase Section Header Label")))
                FoldoutSettings.Settings[key] = !FoldoutSettings.Settings[key];

            // The expand/collapse icon.
            var buttonRect = GUILayoutUtility.GetLastRect();
            var iconRect = new Rect(buttonRect.x + buttonRect.width - CHEVRON_ICON_WIDTH - CHEVRON_ICON_RIGHT_MARGIN,
                buttonRect.y,
                CHEVRON_ICON_WIDTH,
                buttonRect.height);
            GUI.Label(iconRect, GetChevronIcon(foldout), GetCustomStyle("Uppercase Section Header Chevron"));

            EditorGUILayout.EndHorizontal();

            // Draw the section content.
            if (foldout) GUILayout.Space(5);
            if (foldout && drawer != null) drawer();

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draw group selection with header
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sectionName"></param>
        /// <param name="drawer"></param>
        /// <param name="actionRightClick"></param>
        /// <param name="defaultFoldout"></param>
        public static void DrawUppercaseSectionWithRightClick(string key, string sectionName, Action drawer, Action actionRightClick, bool defaultFoldout = true)
        {
            if (!FoldoutSettings.Settings.ContainsKey(key)) FoldoutSettings.Settings.Add(key, defaultFoldout);

            bool foldout = FoldoutSettings.Settings[key];

            EditorGUILayout.BeginVertical(GetCustomStyle("Uppercase Section Box"), GUILayout.MinHeight(foldout ? 30 : 0));
            EditorGUILayout.BeginHorizontal(foldout ? UppercaseSectionHeaderExpand : UppercaseSectionHeaderCollapse);

            // Header label (and button).
            if (GUILayout.Button(sectionName, GetCustomStyle("Uppercase Section Header Label")))
            {
                if (Event.current.button == 1)
                {
                    actionRightClick?.Invoke();
                    return;
                }

                FoldoutSettings.Settings[key] = !FoldoutSettings.Settings[key];
            }

            // The expand/collapse icon.
            var buttonRect = GUILayoutUtility.GetLastRect();
            var iconRect = new Rect(buttonRect.x + buttonRect.width - CHEVRON_ICON_WIDTH - CHEVRON_ICON_RIGHT_MARGIN,
                buttonRect.y,
                CHEVRON_ICON_WIDTH,
                buttonRect.height);
            GUI.Label(iconRect, GetChevronIcon(foldout), GetCustomStyle("Uppercase Section Header Chevron"));

            EditorGUILayout.EndHorizontal();

            // Draw the section content.
            if (foldout) GUILayout.Space(5);
            if (foldout && drawer != null) drawer();

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Icon content
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tooltip"></param>
        /// <returns></returns>
        public static GUIContent IconContent(string name, string tooltip)
        {
            var builtinIcon = EditorGUIUtility.IconContent(name);
            return new GUIContent(builtinIcon.image, tooltip);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldTitle"></param>
        /// <param name="text"></param>
        /// <param name="labelWidth"></param>
        /// <param name="textFieldWidthOption"></param>
        /// <returns></returns>
        public static string DrawTextField(string fieldTitle, string text, GUILayoutOption labelWidth, GUILayoutOption textFieldWidthOption = null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(4);
            EditorGUILayout.LabelField(new GUIContent(fieldTitle), labelWidth);
            GUILayout.Space(4);
            text = textFieldWidthOption == null ? GUILayout.TextField(text) : GUILayout.TextField(text, textFieldWidthOption);
            GUILayout.Space(4);
            GUILayout.EndHorizontal();
            GUILayout.Space(4);

            return text;
        }

        /// <summary>
        /// Draw vertical group
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="options"></param>
        public static void Vertical(Action callback, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(options);
            callback();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draw vertical group
        /// </summary>
        /// <param name="style"></param>
        /// <param name="callback"></param>
        public static void Vertical(GUIStyle style, Action callback)
        {
            EditorGUILayout.BeginVertical(style);
            callback();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draw vertical group
        /// </summary>
        /// <param name="style"></param>
        /// <param name="callback"></param>
        /// <param name="options"></param>
        public static void Vertical(GUIStyle style, Action callback, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(style, options);
            callback();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draw vertical group
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="options"></param>
        public static void VerticalScope(Action callback, params GUILayoutOption[] options)
        {
            using (new EditorGUILayout.VerticalScope(options))
            {
                callback();
            }
        }

        /// <summary>
        /// Draw vertical group
        /// </summary>
        /// <param name="style"></param>
        /// <param name="callback"></param>
        public static void VerticalScope(GUIStyle style, Action callback)
        {
            using (new EditorGUILayout.VerticalScope(style))
            {
                callback();
            }
        }

        /// <summary>
        /// Draw vertical group
        /// </summary>
        /// <param name="style"></param>
        /// <param name="callback"></param>
        /// <param name="options"></param>
        public static void VerticalScope(GUIStyle style, Action callback, params GUILayoutOption[] options)
        {
            using (new EditorGUILayout.VerticalScope(style, options))
            {
                callback();
            }
        }

        /// <summary>
        /// Draw horizontal group
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="options"></param>
        public static void Horizontal(Action callback, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(options);
            callback();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draw horizontal group
        /// </summary>
        /// <param name="style"></param>
        /// <param name="callback"></param>
        public static void Horizontal(GUIStyle style, Action callback)
        {
            EditorGUILayout.BeginHorizontal(style);
            callback();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draw horizontal group
        /// </summary>
        /// <param name="style"></param>
        /// <param name="callback"></param>
        /// <param name="options"></param>
        public static void Horizontal(GUIStyle style, Action callback, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(style, options);
            callback();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draw horizontal scope
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="options"></param>
        public static void HorizontalScope(Action callback, params GUILayoutOption[] options)
        {
            using (new EditorGUILayout.HorizontalScope(options))
            {
                callback();
            }
        }

        /// <summary>
        /// Draw horizontal scope
        /// </summary>
        /// <param name="style"></param>
        /// <param name="callback"></param>
        public static void HorizontalScope(GUIStyle style, Action callback)
        {
            using (new EditorGUILayout.HorizontalScope(style))
            {
                callback();
            }
        }

        /// <summary>
        /// Draw horizontal scope
        /// </summary>
        /// <param name="style"></param>
        /// <param name="callback"></param>
        /// <param name="options"></param>
        public static void HorizontalScope(GUIStyle style, Action callback, params GUILayoutOption[] options)
        {
            using (new EditorGUILayout.HorizontalScope(style, options))
            {
                callback();
            }
        }

        /// <summary>
        /// Create button in editor gui
        /// </summary>
        /// <param name="label"></param>
        /// <param name="callback"></param>
        /// <param name="color"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static bool Button(string label, Action callback = null, Color? color = null, params GUILayoutOption[] options)
        {
            return Button(new GUIContent(label), callback, color, options);
        }

        /// <summary>
        /// Create button in editor gui
        /// </summary>
        /// <param name="content"></param>
        /// <param name="callback"></param>
        /// <param name="color"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static bool Button(GUIContent content, Action callback = null, Color? color = null, params GUILayoutOption[] options)
        {
            var c = GUI.color;
            GUI.color = color ?? c;
            bool b = GUILayout.Button(content, options);
            if (b) callback?.Invoke();
            GUI.color = c;
            return b;
        }

        /// <summary>
        /// create mini button
        /// </summary>
        /// <param name="label"></param>
        /// <param name="callback"></param>
        /// <param name="color"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static bool MiniButton(string label, Action callback = null, Color? color = null, params GUILayoutOption[] options)
        {
            var c = GUI.color;
            GUI.color = color ?? c;
            bool b = GUILayout.Button(new GUIContent(label), new GUIStyle(EditorStyles.miniButton) {fontSize = 11, font = EditorStyles.label.font}, options);
            if (b) callback?.Invoke();
            GUI.color = c;
            return b;
        }

        /// <summary>
        /// Show panel to pickup folder
        /// </summary>
        /// <param name="pathResult"></param>
        /// <param name="defaultPath"></param>
        /// <param name="keySave"></param>
        /// <param name="options"></param>
        public static void PickFolderPath(ref string pathResult, string defaultPath = "", string keySave = "", params GUILayoutOption[] options)
        {
            GUI.backgroundColor = Color.gray;
            if (GUILayout.Button(IconContent("d_Project", "Pick folder"), options))
            {
                string path = EditorUtility.OpenFolderPanel("Select folder", string.IsNullOrEmpty(pathResult) ? defaultPath : pathResult, "");
                if (!string.IsNullOrEmpty(path))
                {
                    pathResult = path;
                    if (!string.IsNullOrEmpty(keySave))
                    {
                        EditorPrefs.SetString(keySave, pathResult);
                    }
                }

                GUI.FocusControl(null);
            }

            GUI.backgroundColor = Color.white;
        }

        /// <summary>
        /// Show panel to pickup file
        /// </summary>
        /// <param name="pathResult"></param>
        /// <param name="defaultPath"></param>
        /// <param name="extension">extension type of file</param>
        /// <param name="keySave"></param>
        /// <param name="options"></param>
        public static void PickFilePath(ref string pathResult, string defaultPath = "", string extension = "db", string keySave = "", params GUILayoutOption[] options)
        {
            GUI.backgroundColor = Color.gray;
            if (GUILayout.Button(IconContent("d_editicon.sml", "Pick file"), options))
            {
                var path = EditorUtility.OpenFilePanel("Select file", string.IsNullOrEmpty(pathResult) ? defaultPath : pathResult, extension);
                if (!string.IsNullOrEmpty(path))
                {
                    pathResult = path;
                    if (!string.IsNullOrEmpty(keySave)) EditorPrefs.SetString(keySave, pathResult);
                }

                GUI.FocusControl(null);
            }

            GUI.backgroundColor = Color.white;
        }

        /// <summary>
        /// Disable groups
        /// </summary>
        public static void DisabledSection(Action onGUI = null, Func<bool> isDisabled = null)
        {
            EditorGUI.BeginDisabledGroup(isDisabled?.Invoke() ?? true);
            onGUI?.Invoke();
            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public static void HelpBox(string message, MessageType type = MessageType.None) { EditorGUILayout.HelpBox(message, type); }

        public static void SpaceHalfLine() => GUILayout.Space(SPACE_HALF_LINE);
        public static void SpaceOneLine() => GUILayout.Space(SPACE_ONE_LINE);
        public static void SpaceTwoLine() => GUILayout.Space(SPACE_TWO_LINE);
        public static void SpaceThreeLine() => GUILayout.Space(SPACE_THREE_LINE);
    }
}