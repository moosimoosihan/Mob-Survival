using UnityEngine;
using UnityEditor;

namespace olimsko
{
    [CustomPropertyDrawer(typeof(UISelectableColor))]
    public class UISelectableColorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // base.OnGUI(position, property, label);
            var m_UseSelectableColor = property.FindPropertyRelative("m_UseSelectableColor");
            var m_UseSelected = property.FindPropertyRelative("m_UseSelected");
            var m_DefaultColor = property.FindPropertyRelative("m_DefaultColor");
            var m_HoverColor = property.FindPropertyRelative("m_HoverColor");
            var m_PressedColor = property.FindPropertyRelative("m_PressedColor");
            var m_SelectedColor = property.FindPropertyRelative("m_SelectedColor");
            var m_DisableColor = property.FindPropertyRelative("m_DisableColor");

            m_UseSelectableColor.boolValue = SubTitleLabel("UseColorTransition", m_UseSelectableColor.boolValue);

            if (m_UseSelectableColor.boolValue)
            {
                GUILayout.Space(20);
                EditorGUI.indentLevel++;
                // EditorGUILayout.PropertyField(m_UseSelectableColor);
                EditorGUILayout.PropertyField(m_UseSelected);
                EditorGUILayout.PropertyField(m_DefaultColor);
                EditorGUILayout.PropertyField(m_HoverColor);
                EditorGUILayout.PropertyField(m_PressedColor);
                EditorGUILayout.PropertyField(m_SelectedColor);
                EditorGUILayout.PropertyField(m_DisableColor);
                EditorGUI.indentLevel--;
                GUILayout.Space(10);
            }
        }

        private bool SubTitleLabel(string title, bool display)
        {
            var style = new GUIStyle(GUI.skin.GetStyle("HelpBox"));
            style.font = new GUIStyle(EditorStyles.label).font;
            style.border = new RectOffset(15, 15, 4, 4);
            style.fixedHeight = 30;
            style.contentOffset = new Vector2(25f, 0f);
            style.alignment = TextAnchor.MiddleLeft;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 12;

            var rect = GUILayoutUtility.GetRect(16f, 20f, style);
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
