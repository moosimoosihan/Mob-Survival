using UnityEngine;
using UnityEditor;

namespace olimsko
{
    [CustomPropertyDrawer(typeof(UISelectableScale))]
    public class UISelectableScaleDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // base.OnGUI(position, property, label);
            var m_UseSelectableScale = property.FindPropertyRelative("m_UseSelectableScale");
            var m_HoverScaleValue = property.FindPropertyRelative("m_HoverScaleValue");
            var m_HoverCurve = property.FindPropertyRelative("m_HoverCurve");
            var m_PressedScaleValue = property.FindPropertyRelative("m_PressedScaleValue");
            var m_PressedCurve = property.FindPropertyRelative("m_PressedCurve");

            m_UseSelectableScale.boolValue = SubTitleLabel("UseScaleTransition", m_UseSelectableScale.boolValue);

            if (m_UseSelectableScale.boolValue)
            {
                GUILayout.Space(20);
                EditorGUI.indentLevel++;
                // EditorGUILayout.PropertyField(m_UseSelectableScale);
                EditorGUILayout.PropertyField(m_HoverScaleValue);
                EditorGUILayout.PropertyField(m_HoverCurve);
                EditorGUILayout.PropertyField(m_PressedScaleValue);
                EditorGUILayout.PropertyField(m_PressedCurve);
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