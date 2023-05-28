using UnityEngine;
using UnityEditor;

namespace olimsko
{
    [CustomPropertyDrawer(typeof(UISelectableMove))]
    public class UISelectableMoveDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // base.OnGUI(position, property, label);
            var m_UseSelectableMove = property.FindPropertyRelative("m_UseSelectableMove");
            var m_HoverMoveLocal = property.FindPropertyRelative("m_HoverMoveLocal");
            var m_PressedMoveLocal = property.FindPropertyRelative("m_PressedMoveLocal");

            m_UseSelectableMove.boolValue = SubTitleLabel("UseMoveTransition", m_UseSelectableMove.boolValue);

            if (m_UseSelectableMove.boolValue)
            {
                GUILayout.Space(20);
                EditorGUI.indentLevel++;
                // EditorGUILayout.PropertyField(m_UseSelectableMove);
                EditorGUILayout.PropertyField(m_HoverMoveLocal);
                EditorGUILayout.PropertyField(m_PressedMoveLocal);
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