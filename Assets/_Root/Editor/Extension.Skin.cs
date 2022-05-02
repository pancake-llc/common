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
        private static GUIStyle boxArea;
        private static Texture2D chevronUp;
        private static Texture2D chevronDown;
        private static Texture2D eraserIcon;
        private static Texture2D pinIcon;
        private static Texture2D extrudeIcon;
        private static Texture2D prefabIcon;
        private static Texture2D alignLeft;
        private static Texture2D alignCenter;
        private static Texture2D alignRight;
        private static Texture2D alignBottom;
        private static Texture2D alignMiddle;
        private static Texture2D alignTop;
        private static Texture2D distributeHorizontal;
        private static Texture2D distributeVertical;
        private static Texture2D snapAllPic;
        private static Texture2D snapVerticalPic;
        private static Texture2D snapHorizontalPic;
        private static Texture2D freeParentModeOnPic;
        private static Texture2D freeParentModeOffPic;
        private static Texture2D allBorderPic;
        private static Texture2D pointPic;
        private static Texture2D verticalPointPic;
        private static Texture2D horizontalPointPic;
        private static Texture2D verticalBorderPic;
        private static Texture2D horizontalBorderPic;

        private const int CHEVRON_ICON_WIDTH = 10;
        private const int CHEVRON_ICON_RIGHT_MARGIN = 5;
        private const float SPACE_HALF_LINE = 2f;
        private const float SPACE_ONE_LINE = 4f;
        private const float SPACE_TWO_LINE = 8f;
        private const float SPACE_THREE_LINE = 12f;

        public class Property
        {
            public SerializedProperty property;
            public GUIContent content;

            public Property(SerializedProperty property, GUIContent content)
            {
                this.property = property;
                this.content = content;
            }

            public Property(GUIContent content) { this.content = content; }
        }

        public static UtilEditor.ProjectSetting<UniformFoldoutState> FoldoutSettings { get; set; } = new UtilEditor.ProjectSetting<UniformFoldoutState>();

        public static GUIStyle UppercaseSectionHeaderExpand { get { return uppercaseSectionHeaderExpand ??= GetCustomStyle("Uppercase Section Header"); } }

        public static GUIStyle UppercaseSectionHeaderCollapse
        {
            get { return uppercaseSectionHeaderCollapse ??= new GUIStyle(GetCustomStyle("Uppercase Section Header")) {normal = new GUIStyleState()}; }
        }

        public static GUIStyle ToggleButtonToolbar { get { return toggleButtonToolbar ??= new GUIStyle(GetCustomStyle("ToggleButton")); } }

        public static GUIStyle BoxArea { get { return boxArea ??= new GUIStyle(GetCustomStyle("BoxArea")); } }

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

        private static void G<T>(ref T t, string sourcePath) where T : class
        {
            if (t != null) return;
            string upmPath = UPM_SKIN_PATH + sourcePath;
            string path = !File.Exists(Path.GetFullPath(upmPath)) ? SKIN_PATH + sourcePath : upmPath;
            t = AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
        }

        public static GUISkin Skin
        {
            get
            {
                if (skin == null) G(ref skin, "Dark.guiskin");
                if (skin == null) Debug.LogError("Couldn't load the Dark.guiskin at GUISkins");

                return skin;
            }
        }

        public static Texture2D ChevronDown
        {
            get
            {
                if (chevronDown == null) G(ref chevronDown, "Icons/icon-chevron-down-dark.psd");
                return chevronDown;
            }
        }

        public static Texture2D ChevronUp
        {
            get
            {
                if (chevronUp == null) G(ref chevronUp, "Icons/icon-chevron-up-dark.psd");
                return chevronUp;
            }
        }

        public static Texture2D PinIcon
        {
            get
            {
                if (pinIcon == null) G(ref pinIcon, "Icons/pin.png");
                return pinIcon;
            }
        }

        public static Texture2D ExtrudeIcon
        {
            get
            {
                if (extrudeIcon == null) G(ref extrudeIcon, "Icons/extrude.png");
                return extrudeIcon;
            }
        }

        public static Texture2D EraserIcon
        {
            get
            {
                if (eraserIcon == null) G(ref eraserIcon, "Icons/eraser.png");
                return eraserIcon;
            }
        }

        public static Texture2D PrefabIcon
        {
            get
            {
                if (prefabIcon == null) G(ref prefabIcon, "Icons/prefab-default.png");
                return prefabIcon;
            }
        }

        public static Texture2D AlignLeft
        {
            get
            {
                if (alignLeft == null) G(ref alignLeft, "Icons/Tools/allign_left.png");
                return alignLeft;
            }
        }

        public static Texture2D AlignCenter
        {
            get
            {
                if (alignCenter == null) G(ref alignCenter, "Icons/Tools/allign_center.png");
                return alignCenter;
            }
        }

        public static Texture2D AlignRight
        {
            get
            {
                if (alignRight == null) G(ref alignRight, "Icons/Tools/allign_right.png");
                return alignRight;
            }
        }

        public static Texture2D AlignBottom
        {
            get
            {
                if (alignBottom == null) G(ref alignBottom, "Icons/Tools/allign_bottom.png");
                return alignBottom;
            }
        }

        public static Texture2D AlignMiddle
        {
            get
            {
                if (alignMiddle == null) G(ref alignMiddle, "Icons/Tools/allign_middle.png");
                return alignMiddle;
            }
        }

        public static Texture2D AlignTop
        {
            get
            {
                if (alignTop == null) G(ref alignTop, "Icons/Tools/allign_top.png");
                return alignTop;
            }
        }

        public static Texture2D DistributeHorizontal
        {
            get
            {
                if (distributeHorizontal == null) G(ref distributeHorizontal, "Icons/Tools/distribute_horizontally.png");
                return distributeHorizontal;
            }
        }

        public static Texture2D DistributeVertical
        {
            get
            {
                if (distributeVertical == null) G(ref distributeVertical, "Icons/Tools/distribute_vertically.png");
                return distributeVertical;
            }
        }

        public static Texture2D SnapAllPic
        {
            get
            {
                if (snapAllPic == null) G(ref snapAllPic, "Icons/Tools/snap_to_childs_all.png");
                return snapAllPic;
            }
        }

        public static Texture2D SnapHorizontalPic
        {
            get
            {
                if (snapHorizontalPic == null) G(ref snapHorizontalPic, "Icons/Tools/snap_to_childs_h.png");
                return snapHorizontalPic;
            }
        }

        public static Texture2D SnapVerticalPic
        {
            get
            {
                if (snapVerticalPic == null) G(ref snapVerticalPic, "Icons/Tools/snap_to_childs_v.png");
                return snapVerticalPic;
            }
        }

        public static Texture2D FreeParentModeOnPic
        {
            get
            {
                if (freeParentModeOnPic == null) G(ref freeParentModeOnPic, "Icons/Tools/free_parent_mode_on.png");
                return freeParentModeOnPic;
            }
        }

        public static Texture2D FreeParentModeOffPic
        {
            get
            {
                if (freeParentModeOffPic == null) G(ref freeParentModeOffPic, "Icons/Tools/free_parent_mode_off.png");
                return freeParentModeOffPic;
            }
        }
        
        public static Texture2D AllBorderPic
        {
            get
            {
                if (allBorderPic == null) G(ref allBorderPic, "Icons/Tools/snap_all_edges.png");
                return allBorderPic;
            }
        }
        
        public static Texture2D PointPic
        {
            get
            {
                if (pointPic == null) G(ref pointPic, "Icons/Tools/snap_all_direction_point.png");
                return pointPic;
            }
        }
        
        public static Texture2D HorizontalPointPic
        {
            get
            {
                if (horizontalPointPic == null) G(ref horizontalPointPic, "Icons/Tools/snap_horizontal_point.png");
                return horizontalPointPic;
            }
        }
        
        public static Texture2D VerticalPointPic
        {
            get
            {
                if (verticalPointPic == null) G(ref verticalPointPic, "Icons/Tools/snap_vertical_point.png");
                return verticalPointPic;
            }
        }
        
        public static Texture2D HorizontalBorderPic
        {
            get
            {
                if (horizontalBorderPic == null) G(ref horizontalBorderPic, "Icons/Tools/snap_horizontal_edges.png");
                return horizontalBorderPic;
            }
        }
        
        public static Texture2D VerticalBorderPic
        {
            get
            {
                if (verticalBorderPic == null) G(ref verticalBorderPic, "Icons/Tools/snap_vertical_edges.png");
                return verticalBorderPic;
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