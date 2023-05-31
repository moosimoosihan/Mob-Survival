using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace olimsko
{
    [OverrideSettings]
    public class UIConfigurationSetting : ConfigurationSettingsProvider<UIConfiguration>
    {
        private const float headerLeftMargin = 5;
        private const float paddingWidth = 10;

        private ReorderableList predefinedVariablesList;

        private static readonly GUIContent nameContent = new GUIContent("Name", "Prefab Name");
        private static readonly GUIContent valueContent = new GUIContent("Prefab", "Prefab GameObject");

        protected override Dictionary<string, Action<SerializedProperty>> OverrideConfigurationDrawers()
        {
            var drawers = base.OverrideConfigurationDrawers();
            drawers[nameof(UIConfiguration.ListPrefabs)] = DrawPredefinedVariablesEditor;
            return drawers;
        }

        protected override void DrawConfigurationEditor()
        {
            DrawDefaultEditor();
        }

        private bool m_FoldoutSection0 = true;
        private bool m_FoldoutSection1 = true;
        private bool m_FoldoutSection2 = true;
        private bool m_FoldoutSection3 = true;
        private bool m_FoldoutSection4 = true;

        Dictionary<string, SerializedProperty> m_DicProperty = new Dictionary<string, SerializedProperty>();

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);

            m_DicProperty.Clear();

            m_DicProperty.Add("UIRenderMode", SerializedObject.FindProperty("UIRenderMode"));
            m_DicProperty.Add("CustomUICamera", SerializedObject.FindProperty("CustomUICamera"));
            m_DicProperty.Add("DefaultScreenSize", SerializedObject.FindProperty("DefaultScreenSize"));
            m_DicProperty.Add("Match", SerializedObject.FindProperty("Match"));
            m_DicProperty.Add("UseHideHotKey", SerializedObject.FindProperty("UseHideHotKey"));
            m_DicProperty.Add("HideHotKeyCode", SerializedObject.FindProperty("HideHotKeyCode"));
            m_DicProperty.Add("OverrideObjectLayer", SerializedObject.FindProperty("OverrideObjectLayer"));
            m_DicProperty.Add("ObjectLayer", SerializedObject.FindProperty("ObjectLayer"));
            m_DicProperty.Add("ListPrefabs", SerializedObject.FindProperty("ListPrefabs"));
            m_DicProperty.Add("UINameWhenNoHotKeyList", SerializedObject.FindProperty("UINameWhenNoHotKeyList"));
            m_DicProperty.Add("AutoSpawnInputSystem", SerializedObject.FindProperty("AutoSpawnInputSystem"));
            m_DicProperty.Add("CustomInputSystem", SerializedObject.FindProperty("CustomInputSystem"));
            m_DicProperty.Add("MakeScriptWhenAddPrefab", SerializedObject.FindProperty("MakeScriptWhenAddPrefab"));

        }

        private new void DrawDefaultEditor()
        {
            var property = SerializedObject.GetIterator();

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

            // EditorGUI.indentLevel++;
            GUILayout.Space(10);


            m_FoldoutSection0 = Foldout("Camera Setting", m_FoldoutSection0);
            if (m_FoldoutSection0)
            {
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(m_DicProperty["UIRenderMode"], true);
                EditorGUILayout.PropertyField(m_DicProperty["CustomUICamera"], true);
                GUILayout.Space(10);

            }

            m_FoldoutSection1 = Foldout("Screen Setting", m_FoldoutSection1);
            if (m_FoldoutSection1)
            {
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(m_DicProperty["DefaultScreenSize"], true);
                EditorGUILayout.PropertyField(m_DicProperty["Match"]);
                GUILayout.Space(10);

            }

            m_FoldoutSection4 = Foldout("Input System", m_FoldoutSection4);
            if (m_FoldoutSection4)
            {
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(m_DicProperty["AutoSpawnInputSystem"], true);
                EditorGUILayout.PropertyField(m_DicProperty["CustomInputSystem"]);
                GUILayout.Space(10);

            }

            m_FoldoutSection2 = Foldout("UI Setting", m_FoldoutSection2);
            if (m_FoldoutSection2)
            {
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(m_DicProperty["UseHideHotKey"], true);
                EditorGUILayout.PropertyField(m_DicProperty["HideHotKeyCode"], true);
                EditorGUILayout.PropertyField(m_DicProperty["UINameWhenNoHotKeyList"], true);
                EditorGUILayout.PropertyField(m_DicProperty["OverrideObjectLayer"], true);

                EditorGUI.BeginChangeCheck();
                int newLayer = EditorGUILayout.LayerField("Object Layer", m_DicProperty["ObjectLayer"].intValue);
                if (EditorGUI.EndChangeCheck())
                {
                    m_DicProperty["ObjectLayer"].intValue = newLayer;
                }


                GUILayout.Space(10);

            }

            m_FoldoutSection3 = Foldout("UI Prefab", m_FoldoutSection3);
            if (m_FoldoutSection3)
            {
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(m_DicProperty["MakeScriptWhenAddPrefab"], true);
                OverrideConfigurationDrawers()[m_DicProperty["ListPrefabs"].propertyPath]?.Invoke(m_DicProperty["ListPrefabs"]);
                GUILayout.Space(10);

            }

            GUILayout.Space(10);
            // EditorGUI.indentLevel--;

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
            predefinedVariablesList = new ReorderableList(SerializedObject, SerializedObject.FindProperty(nameof(UIConfiguration.ListPrefabs)), true, true, true, true);
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
            var nameProperty = elementProperty.FindPropertyRelative("m_Name");
            var valueProperty = elementProperty.FindPropertyRelative("m_Prefab");

            EditorGUI.PropertyField(propertyRect, nameProperty, GUIContent.none);

            propertyRect.x += propertyRect.width + paddingWidth;

            EditorGUI.PropertyField(propertyRect, valueProperty, GUIContent.none);
        }

        public bool Foldout(string title, bool display)
        {
            var style = new GUIStyle(GUI.skin.GetStyle("HelpBox"));
            style.font = new GUIStyle(EditorStyles.label).font;
            style.border = new RectOffset(15, 15, 4, 4);
            style.fixedHeight = 30;
            style.contentOffset = new Vector2(0f, -2f);
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 13;

            var rect = GUILayoutUtility.GetRect(16f, 30f, style);
            GUI.Box(rect, title, style);

            var e = Event.current;

            var toggleRect = new Rect(rect.x + 8f, rect.y + 8f, 13f, 13f);
            if (e.type == EventType.Repaint)
            {
                EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
            }

            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                display = !display;
                e.Use();
            }

            return display;
        }

        public bool SubTitleLabel(string title, bool display)
        {
            var style = new GUIStyle(GUI.skin.GetStyle("HelpBox"));
            style.font = new GUIStyle(EditorStyles.label).font;
            style.border = new RectOffset(15, 15, 4, 4);
            style.fixedHeight = 30;
            style.contentOffset = new Vector2(25f, 2f);
            style.alignment = TextAnchor.MiddleLeft;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 12;

            var rect = GUILayoutUtility.GetRect(16f, 30f, style);
            GUI.Box(rect, title, style);

            var e = Event.current;

            var toggleRect = new Rect(rect.x + 8f, rect.y + 8f, 13f, 13f);
            if (e.type == EventType.Repaint)
            {
                EditorStyles.toggle.Draw(toggleRect, false, false, display, false);
            }

            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                display = !display;
                e.Use();
            }

            return display;
        }
    }

}

