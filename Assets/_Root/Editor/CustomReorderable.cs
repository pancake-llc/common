

namespace Snorlax.Editor
{
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    

    public class CustomReorderable
    {
        public delegate void CreateButtonDelegate(Rect rect, ref SerializedProperty property, int index);

        private static class Style
        {
            public static readonly GUIContent AddContent;
            public static readonly GUIStyle AddStyle;
            public static readonly GUIContent SubContent;
            public static readonly GUIStyle SubStyle;

            static Style()
            {
                AddContent = EditorGUIUtility.TrIconContent("Toolbar Plus", "Add to list");
                AddStyle = "RL FooterButton";
                SubContent = EditorGUIUtility.TrIconContent("Toolbar Minus", "Remove selection from list");
                SubStyle = "RL FooterButton";
            }
        }

        private readonly ReorderableList _reorderableList;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="onAddCallback"></param>
        /// <param name="onRemoveCallback"></param>
        /// <param name="onReorderCallbackWithDetails"></param>
        /// <param name="onChangedCallback"></param>
        /// <param name="actionCreateCustomButton"></param>
        public CustomReorderable(
            SerializedProperty property,
            ReorderableList.AddCallbackDelegate onAddCallback = null,
            ReorderableList.RemoveCallbackDelegate onRemoveCallback = null,
            ReorderableList.ReorderCallbackDelegateWithDetails onReorderCallbackWithDetails = null,
            ReorderableList.ChangedCallbackDelegate onChangedCallback = null,
            CreateButtonDelegate actionCreateCustomButton = null)
        {
            _reorderableList = CreateInstance(property,
                onAddCallback,
                onRemoveCallback,
                onReorderCallbackWithDetails,
                onChangedCallback,
                actionCreateCustomButton);
        }

        /// <summary>
        /// </summary>
        public void DoLayoutList() { _reorderableList.DoLayoutList(); }

        private ReorderableList CreateInstance(
            SerializedProperty property,
            ReorderableList.AddCallbackDelegate onAddCallback,
            ReorderableList.RemoveCallbackDelegate onRemoveCallback,
            ReorderableList.ReorderCallbackDelegateWithDetails onReorderCallbackWithDetails,
            ReorderableList.ChangedCallbackDelegate onChangedCallback,
            CreateButtonDelegate actionCreateCustomButton)
        {
            return new ReorderableList(property.serializedObject,
                property,
                true,
                true,
                false,
                false)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, $"{property.displayName}: {property.arraySize}", EditorStyles.boldLabel);
                    var position = new Rect(rect.width - System.Math.Max(EditorGUI.indentLevel - property.depth, 1) * 15f, rect.y, 20f, 13f);
                    if (GUI.Button(position, Style.AddContent, Style.AddStyle))
                    {
                        property.serializedObject.UpdateIfRequiredOrScript();
                        property.InsertArrayElementAtIndex(property.arraySize);
                        property.serializedObject.ApplyModifiedProperties();
                        onAddCallback?.Invoke(_reorderableList);
                    }
                },
                drawElementCallback = (rect, index, _, _) =>
                {
                    if (property.arraySize <= index)
                        return;

                    var indentWidth = 30f;
                    if (actionCreateCustomButton != null) indentWidth = 50f;
                    DrawElement(property, rect, index, indentWidth);

                    rect.xMin = rect.width - (EditorGUI.indentLevel - property.depth) * 15f;
                    rect.width = 20f;
                    if (GUI.Button(rect, Style.SubContent, Style.SubStyle))
                    {
                        property.serializedObject.UpdateIfRequiredOrScript();
                        property.DeleteArrayElementAtIndex(index);
                        property.serializedObject.ApplyModifiedProperties();
                        onRemoveCallback?.Invoke(_reorderableList);
                    }

                    // todo
                    if (actionCreateCustomButton != null)
                    {
                        rect.xMin -= 20;
                        rect.width = 20f;
                        actionCreateCustomButton.Invoke(rect, ref property, index);
                    }
                },
                onChangedCallback = list => { onChangedCallback?.Invoke(list); },
                onReorderCallbackWithDetails = (list, index, newIndex) => { onReorderCallbackWithDetails?.Invoke(list, index, newIndex); },
                drawFooterCallback = _ => { },
                footerHeight = 0f,
                elementHeightCallback = index =>
                {
                    if (property.arraySize <= index)
                        return 0;

                    return EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(index));
                }
            };
        }

        private void DrawElement(SerializedProperty property, Rect rect, int index, float indentWidth)
        {
            var indexName = index.ToString();

            rect.x += 5f;
            rect.width -= indentWidth;
            var elementProperty = property.GetArrayElementAtIndex(index);
            if (elementProperty.propertyType != SerializedPropertyType.Generic)
            {
                EditorGUI.PropertyField(rect, elementProperty, new GUIContent(indexName));
                return;
            }

            rect.x += 10f;
            rect.width -= 20f;
            rect.height = EditorGUIUtility.singleLineHeight;

            elementProperty.isExpanded = EditorGUI.Foldout(rect, elementProperty.isExpanded, new GUIContent(indexName));
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (!elementProperty.isExpanded)
                return;

            int depth = -1;
            while (elementProperty.NextVisible(true) || depth == -1)
            {
                if (depth != -1 && elementProperty.depth != depth)
                    break;
                depth = elementProperty.depth;
                rect.height = EditorGUI.GetPropertyHeight(elementProperty);
                EditorGUI.PropertyField(rect, elementProperty, true);
                rect.y += rect.height;
            }
        }
    }
}