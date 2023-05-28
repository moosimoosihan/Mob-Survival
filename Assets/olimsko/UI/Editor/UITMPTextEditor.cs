using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.UI;
using TMPro;
using TMPro.EditorUtilities;

namespace olimsko
{
    [CustomEditor(typeof(UITMPText), true), CanEditMultipleObjects]
    public class UITMPTextEditor : TMP_EditorPanelUI
    {
        Dictionary<string, SerializedProperty> m_DicProperty = new Dictionary<string, SerializedProperty>();

        protected override void OnEnable()
        {
            base.OnEnable();
            m_DicProperty.Clear();

            var property = serializedObject.GetIterator();
            var enterChildren = true;
            while (property.NextVisible(enterChildren))
            {
                SerializedProperty temp = serializedObject.FindProperty(property.propertyPath);
                m_DicProperty.Add(temp.propertyPath, temp);
            }
        }

        public override void OnInspectorGUI()
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                fixedHeight = 20,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 14,
            };

            serializedObject.Update();

            if (!m_DicProperty["m_IsUseCustomBindPath"].boolValue) m_DicProperty["m_BindPath"].stringValue = target.name;
            if (string.IsNullOrEmpty(m_DicProperty["m_BindPath"].stringValue)) m_DicProperty["m_BindPath"].stringValue = target.name;

            GUILayout.BeginVertical("", "GroupBox");

            EditorGUILayout.BeginVertical("textArea", GUILayout.Height(30));
            EditorGUILayout.LabelField($"UITMPText", style, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);

            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(m_DicProperty["m_IsBind"]);
            EditorGUILayout.PropertyField(m_DicProperty["m_IsUseCustomBindPath"]);

            EditorGUI.BeginDisabledGroup(!m_DicProperty["m_IsBind"].boolValue);
            EditorGUILayout.PropertyField(m_DicProperty["m_BindPath"]);
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }

}
