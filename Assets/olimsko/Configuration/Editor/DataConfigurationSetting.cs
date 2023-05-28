
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace olimsko
{
    public class DataConfigurationSetting : ConfigurationSettingsProvider<DataConfiguration>
    {
        private const float headerLeftMargin = 5;
        private const float paddingWidth = 10;

        private ReorderableList predefinedVariablesList;

        private static readonly GUIContent nameContent = new GUIContent("SheetID", "Google Spreadsheet sheet ID");
        private static readonly GUIContent valueContent = new GUIContent("GoogleSheetDataSO", "ScriptableObject of GoogleSheetData");

        protected override Dictionary<string, Action<SerializedProperty>> OverrideConfigurationDrawers()
        {
            var drawers = base.OverrideConfigurationDrawers();
            drawers[nameof(DataConfiguration.ListGoogleSheetData)] = DrawPredefinedVariablesEditor;
            return drawers;
        }

        protected override void DrawConfigurationEditor()
        {
            DrawDefaultEditor();
        }

        Dictionary<string, SerializedProperty> m_DicProperty = new Dictionary<string, SerializedProperty>();

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);

            m_DicProperty.Clear();

            var property = SerializedObject.GetIterator();
            var enterChildren = true;
            while (property.NextVisible(enterChildren))
            {
                SerializedProperty temp = SerializedObject.FindProperty(property.propertyPath);
                m_DicProperty.Add(temp.propertyPath, temp);
            }
        }

        private new void DrawDefaultEditor()
        {
            DataConfiguration dataConfiguration = SerializedObject.targetObject as DataConfiguration;

            var style = new GUIStyle(GUI.skin.label)
            {
                fixedHeight = 25f,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 20,
            };
            var style2 = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 13,
            };

            EditorGUILayout.BeginVertical("GroupBox");
            EditorGUILayout.BeginVertical("textArea", GUILayout.Height(30));
            EditorGUILayout.LabelField($"{EditorTitle} Configuration", style, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();

            GUILayout.BeginVertical("", "GroupBox");

            GUILayout.Space(10);

            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(m_DicProperty["UseGoogleSheet"]);

            EditorGUI.BeginDisabledGroup(!m_DicProperty["UseGoogleSheet"].boolValue);

            EditorGUILayout.PropertyField(m_DicProperty["IsInitializeAtRuntime"]);
            EditorGUILayout.PropertyField(m_DicProperty["GoogleSheetKey"]);

            OverrideConfigurationDrawers()[m_DicProperty["ListGoogleSheetData"].propertyPath]?.Invoke(m_DicProperty["ListGoogleSheetData"]);

            EditorGUI.EndDisabledGroup();

            SerializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel--;

            GUILayout.Space(10);

            EditorGUILayout.EndVertical();
        }

        private void DrawPredefinedVariablesEditor(SerializedProperty property)
        {
            var label = EditorGUI.BeginProperty(Rect.zero, null, property);
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

            // Always check list's serialized object parity with the inspected object.
            if (predefinedVariablesList is null || predefinedVariablesList.serializedProperty.serializedObject != SerializedObject)
                InitializePredefinedVariablesList();

            predefinedVariablesList.DoLayoutList();

            EditorGUI.EndProperty();
        }

        private void InitializePredefinedVariablesList()
        {
            predefinedVariablesList = new ReorderableList(SerializedObject, SerializedObject.FindProperty(nameof(DataConfiguration.ListGoogleSheetData)), true, true, true, true);
            predefinedVariablesList.drawHeaderCallback = DrawPredefinedVariablesListHeader;
            predefinedVariablesList.drawElementCallback = DrawPredefinedVariablesListElement;
        }

        private void DrawPredefinedVariablesListHeader(Rect rect)
        {
            var propertyRect = new Rect(headerLeftMargin + rect.x, rect.y, (rect.width / 2f) - paddingWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(propertyRect, nameContent);
            propertyRect.x += propertyRect.width + paddingWidth;
            EditorGUI.LabelField(propertyRect, valueContent);
        }

        private void DrawPredefinedVariablesListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var propertyRect = new Rect(rect.x, rect.y + EditorGUIUtility.standardVerticalSpacing, (rect.width / 2f) - paddingWidth, EditorGUIUtility.singleLineHeight);

            var elementProperty = predefinedVariablesList.serializedProperty.GetArrayElementAtIndex(index);
            var nameProperty = elementProperty.FindPropertyRelative("m_Key");
            var valueProperty = elementProperty.FindPropertyRelative("m_Data");

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(propertyRect, nameProperty, GUIContent.none);

            if (EditorGUI.EndChangeCheck())
            {
                DataConfiguration dataConfiguration = SerializedObject.targetObject as DataConfiguration;
                GoogleSheetDataList listItem = dataConfiguration.ListGoogleSheetData[index];
                listItem.Key = nameProperty.stringValue;
                listItem.Data.Key = dataConfiguration.GoogleSheetKey;
                listItem.Data.SheetId = listItem.Key;

                SerializedObject.ApplyModifiedProperties();
            }

            propertyRect.x += propertyRect.width + paddingWidth;

            EditorGUI.PropertyField(propertyRect, valueProperty, GUIContent.none);
        }
    }
}
