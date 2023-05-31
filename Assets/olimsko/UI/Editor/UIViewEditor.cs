using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace olimsko
{
    [CustomEditor(typeof(UIView), true), CanEditMultipleObjects]
    public class UIViewEditor : Editor
    {
        #region SerializedProperties
        SerializedProperty m_IsDependenceInScene;
        SerializedProperty m_IsInteractable;
        SerializedProperty m_VisibleOnAwake;
        SerializedProperty m_AnimTime;
        SerializedProperty m_IgnoreTimeScale;
        SerializedProperty m_HideWhenLoading;
        SerializedProperty m_UseCancelHotKey;
        SerializedProperty m_UseBlackOveray;
        SerializedProperty m_BlackOverayColor;
        SerializedProperty m_IsModalUI;
        SerializedProperty m_OnShow;
        SerializedProperty m_OnShowComplete;
        SerializedProperty m_OnHide;
        SerializedProperty m_OnHideComplete;
        #endregion

        #region PrivateField
        private bool m_FoldoutSection0 = true;
        private bool m_FoldoutSection1 = true;
        private bool m_FoldoutSection2 = true;
        private bool m_FoldoutSection3 = true;
        private bool m_FoldoutSection4 = true;
        #endregion

        private void OnEnable()
        {
            m_IsDependenceInScene = serializedObject.FindProperty("m_IsDependenceInScene");
            m_IsInteractable = serializedObject.FindProperty("m_IsInteractable");
            m_VisibleOnAwake = serializedObject.FindProperty("m_VisibleOnAwake");
            m_AnimTime = serializedObject.FindProperty("m_AnimTime");
            m_IgnoreTimeScale = serializedObject.FindProperty("m_IgnoreTimeScale");
            m_HideWhenLoading = serializedObject.FindProperty("m_HideWhenLoading");
            m_UseCancelHotKey = serializedObject.FindProperty("m_UseCancelHotKey");
            m_UseBlackOveray = serializedObject.FindProperty("m_UseBlackOveray");
            m_BlackOverayColor = serializedObject.FindProperty("m_BlackOverayColor");
            m_IsModalUI = serializedObject.FindProperty("m_IsModalUI");
            m_OnShow = serializedObject.FindProperty("m_OnShow");
            m_OnShowComplete = serializedObject.FindProperty("m_OnShowComplete");
            m_OnHide = serializedObject.FindProperty("m_OnHide");
            m_OnHideComplete = serializedObject.FindProperty("m_OnHideComplete");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            UIView uIView = (UIView)target;
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

            serializedObject.Update();

            EditorGUILayout.BeginVertical("GroupBox");
            EditorGUILayout.BeginVertical("textArea", GUILayout.Height(30));
            EditorGUILayout.LabelField(uIView.name, style, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();

            GUILayout.BeginVertical("", "GroupBox");
            EditorGUI.indentLevel++;
            m_FoldoutSection0 = Foldout("Custom", m_FoldoutSection0);
            if (m_FoldoutSection0)
            {
                GUILayout.Space(10);
                base.OnInspectorGUI();
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            GUILayout.BeginVertical("", "GroupBox");
            // EditorGUILayout.LabelField("Visible And Interactable", style2);
            EditorGUI.indentLevel++;
            m_FoldoutSection1 = Foldout("Visible And Interactable", m_FoldoutSection1);
            if (m_FoldoutSection1)
            {
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(m_IsDependenceInScene);
                EditorGUILayout.PropertyField(m_IsInteractable);
                EditorGUILayout.PropertyField(m_VisibleOnAwake);
                EditorGUILayout.PropertyField(m_HideWhenLoading);
                EditorGUILayout.PropertyField(m_UseCancelHotKey);

            }
            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();

            GUILayout.BeginVertical("", "GroupBox");
            // EditorGUILayout.LabelField("Animation", style2);
            m_FoldoutSection2 = Foldout("Animation", m_FoldoutSection2);

            EditorGUI.indentLevel++;
            if (m_FoldoutSection2)
            {
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(m_AnimTime);
                EditorGUILayout.PropertyField(m_IgnoreTimeScale);
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();

            GUILayout.BeginVertical("", "GroupBox");
            // EditorGUILayout.LabelField("Overay", style2);
            m_FoldoutSection3 = Foldout("Overay", m_FoldoutSection3);

            EditorGUI.indentLevel++;
            if (m_FoldoutSection3)
            {
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(m_UseBlackOveray);
                if (uIView.UseBlackOveray)
                {
                    EditorGUILayout.PropertyField(m_BlackOverayColor);
                }
                EditorGUILayout.PropertyField(m_IsModalUI);
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();

            GUILayout.BeginVertical("", "GroupBox");
            // EditorGUILayout.LabelField("Events", style2);
            m_FoldoutSection4 = Foldout("Events", m_FoldoutSection4);

            EditorGUI.indentLevel++;
            if (m_FoldoutSection4)
            {
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(m_OnShow);
                EditorGUILayout.PropertyField(m_OnShowComplete);
                EditorGUILayout.PropertyField(m_OnHide);
                EditorGUILayout.PropertyField(m_OnHideComplete);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            // FieldInfo[] childFields = target.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            // foreach (FieldInfo field in childFields)
            // {
            //     //if(field.IsNotSerialized || field.IsStatic)
            //     //{
            //     //    continue;
            //     //}

            //     if (field.IsPublic || field.GetCustomAttribute(typeof(SerializeField)) != null)
            //     {

            //         EditorGUILayout.PropertyField(serializedObject.FindProperty(field.Name));
            //     }
            // }

            serializedObject.ApplyModifiedProperties();
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
    }
}