using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Pancake.Common
{
    public class SnowflakeAttribute : PropertyAttribute
    {
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SnowflakeAttribute))]
    public class SnowflakeAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginDisabledGroup(true);
            if (property.propertyType == SerializedPropertyType.String)
            {
                if (string.IsNullOrEmpty(property.stringValue))
                {
                    Recreate(property);
                }

                EditorGUI.PropertyField(position, property, label);
            }
            else
            {
                EditorGUI.LabelField(position, "Id field must be a string");
            }

            EditorGUI.EndDisabledGroup();
            EditorGUI.EndProperty();

            var e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 1 && position.Contains(e.mousePosition))
            {
                var context = new GenericMenu();
                context.AddItem(new GUIContent("Copy"), false, p => Copy(property), property);
                context.AddItem(new GUIContent("Recreate"), false, p => Recreate(property), property);
                context.ShowAsContext();
            }
        }

        private void Recreate(SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.String) return;
            property.stringValue = new IdStringGeneratorWrapper(new Id64Generator(10)).GenerateId();
            property.serializedObject.ApplyModifiedProperties();
        }

        private void Copy(SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.String) return;

            property.stringValue.CopyToClipboard();
        }
    }
#endif
}