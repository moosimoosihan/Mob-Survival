using UnityEngine;
using UnityEditor;
using System.Linq;

namespace olimsko
{
    [CustomPropertyDrawer(typeof(RequireInterfaceAttribute))]
    public class RequireInterfaceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var requiredAttribute = this.attribute as RequireInterfaceAttribute;
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                EditorGUI.BeginProperty(position, label, property);

                property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, requiredAttribute.requiredType, true);

                EditorGUI.EndProperty();
            }
            else
            {
                var previousColor = GUI.color;
                GUI.color = Color.red;

                EditorGUI.LabelField(position, label, new GUIContent($"{requiredAttribute.requiredType} 유형의 객체만 참조할 수 있습니다."));

                GUI.color = previousColor;
            }
        }
    }
}
