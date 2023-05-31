
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UIElements;

namespace olimsko
{
    [OverrideSettings]
    public class ContextConfigurationSetting : ConfigurationSettingsProvider<ContextConfiguration>
    {

        Dictionary<string, SerializedProperty> m_DicProperty = new Dictionary<string, SerializedProperty>();


        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);

            m_DicProperty.Clear();
            m_DicProperty.Add("ListPredefinedContext", SerializedObject.FindProperty("ListPredefinedContext"));

            UpdateContextModelList();
        }

        protected override void DrawConfigurationEditor()
        {
            DrawDefaultEditor();
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

            GUILayout.Space(10);
            GUI.enabled = false;
            EditorGUILayout.PropertyField(m_DicProperty["ListPredefinedContext"]);
            GUI.enabled = true;
            GUILayout.Space(10);

            EditorGUILayout.EndVertical();
        }

        private static void UpdateContextModelList()
        {
            var context = ConfigurationProvider.LoadOrDefault<ContextConfiguration>();

            if (context == null)
            {
                Debug.LogWarning("ContextConfiguration asset not found. Please create one and place it in a Resources folder.");
                return;
            }

            var contextModelTypes = OSManager.Types
                .Where(t => !t.IsAbstract && typeof(IContextModel).IsAssignableFrom(t))
                .ToList();

            context.ListPredefinedContext = new List<string>();

            foreach (var contextModelType in contextModelTypes)
            {
                context.ListPredefinedContext.Add(contextModelType.Name);
                EditorUtility.SetDirty(context);
            }
        }
    }


}
