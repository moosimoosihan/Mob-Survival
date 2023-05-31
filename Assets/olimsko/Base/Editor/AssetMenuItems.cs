using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using UnityEngine.UI;

namespace olimsko
{
    public class AssetMenuItems
    {
        [MenuItem("Assets/Create/olimsko/Custom UI", false, 0)]
        private static void CreateCustomUI()
        {


            CreatePrefabCopy("Templates/CustomUI", "NewCustomUI");
        }

        [MenuItem("GameObject/UI/olimsko/UIButton", false, 0)]
        private static void CreateUIButton(MenuCommand menuCommand)
        {
            CreateCanvasAndEventSystemIfNeeded();

            GameObject buttonObject = new GameObject("UIButton");
            buttonObject.AddComponent<RectTransform>();
            buttonObject.layer = LayerMask.NameToLayer("UI");
            UIImage image = buttonObject.AddComponent<UIImage>();
            UIButton button = buttonObject.AddComponent<UIButton>();
            button.transition = Selectable.Transition.None;
            image.rectTransform.anchoredPosition = Vector2.zero;

            GameObject textObject = new GameObject("UIText");
            textObject.AddComponent<RectTransform>();
            textObject.layer = LayerMask.NameToLayer("UI");
            UIText text = textObject.AddComponent<UIText>();
            text.text = "New Text";
            text.raycastTarget = false;
            text.fontSize = 32;
            text.rectTransform.anchoredPosition = Vector2.zero;

            textObject.transform.SetParent(buttonObject.transform, false);
            textObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            GameObjectUtility.SetParentAndAlign(buttonObject, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(buttonObject, "Create " + buttonObject.name);
            Selection.activeObject = buttonObject;
        }

        [MenuItem("GameObject/UI/olimsko/UIImage", false, 1)]
        private static void CreateUIImage(MenuCommand menuCommand)
        {
            CreateCanvasAndEventSystemIfNeeded();

            GameObject imageObject = new GameObject("UIImage");
            imageObject.AddComponent<RectTransform>();
            imageObject.layer = LayerMask.NameToLayer("UI");
            UIImage image = imageObject.AddComponent<UIImage>();
            image.raycastTarget = false;
            image.rectTransform.anchoredPosition = Vector2.zero;

            GameObjectUtility.SetParentAndAlign(imageObject, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(imageObject, "Create " + imageObject.name);
            Selection.activeObject = imageObject;
        }

        [MenuItem("GameObject/UI/olimsko/UITMPText", false, 2)]
        private static void CreateUITMPText(MenuCommand menuCommand)
        {
            CreateCanvasAndEventSystemIfNeeded();

            // 새로운 TMP 텍스트 생성
            GameObject newTextObject = new GameObject("UITMPText");
            newTextObject.transform.position = Vector3.zero;
            newTextObject.layer = LayerMask.NameToLayer("UI");

            UITMPText tmpText = newTextObject.AddComponent<UITMPText>();
            tmpText.text = "New TMP Text";
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.color = Color.black;
            tmpText.fontSize = 32;
            tmpText.raycastTarget = false;

            RectTransform rectTransform = newTextObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(300, 50);
            rectTransform.anchoredPosition = Vector2.zero;

            // 생성된 게임 오브젝트 선택
            GameObjectUtility.SetParentAndAlign(newTextObject, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(newTextObject, "Create " + newTextObject.name);
            Selection.activeObject = newTextObject;
        }

        [MenuItem("GameObject/UI/olimsko/UIToggle", false, 3)]
        private static void CreateUIToggle(MenuCommand menuCommand)
        {
            CreateCanvasAndEventSystemIfNeeded();

            GameObject toggleObject = new GameObject("UIToggle");
            toggleObject.AddComponent<RectTransform>();
            toggleObject.layer = LayerMask.NameToLayer("UI");

            UIToggle toggle = toggleObject.AddComponent<UIToggle>();
            toggle.targetGraphic = toggleObject.AddComponent<UIImage>();

            GameObject checkmarkObject = new GameObject("Checkmark");
            checkmarkObject.AddComponent<RectTransform>();
            checkmarkObject.AddComponent<UIImage>();

            checkmarkObject.transform.SetParent(toggleObject.transform, false);
            checkmarkObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            toggle.graphic = checkmarkObject.GetComponent<UIImage>();

            GameObject textObject = new GameObject("Text");
            textObject.AddComponent<RectTransform>();
            textObject.layer = LayerMask.NameToLayer("UI");
            UIText text = textObject.AddComponent<UIText>();
            text.text = "New Text";
            text.raycastTarget = false;
            text.fontSize = 32;
            text.rectTransform.anchoredPosition = Vector2.zero;

            textObject.transform.SetParent(toggleObject.transform, false);
            textObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            GameObjectUtility.SetParentAndAlign(toggleObject, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(toggleObject, "Create " + toggleObject.name);
            Selection.activeObject = toggleObject;
        }

        [MenuItem("GameObject/UI/olimsko/UISlider", false, 4)]
        private static void CreateUISlider(MenuCommand menuCommand)
        {
            CreateCanvasAndEventSystemIfNeeded();

            GameObject sliderObject = new GameObject("UISlider");
            sliderObject.AddComponent<RectTransform>();
            sliderObject.AddComponent<UIImage>();
            sliderObject.layer = LayerMask.NameToLayer("UI");
            UISlider slider = sliderObject.AddComponent<UISlider>();

            GameObject fillAreaObject = new GameObject("Fill Area");
            fillAreaObject.AddComponent<RectTransform>();

            fillAreaObject.transform.SetParent(sliderObject.transform, false);
            fillAreaObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            GameObject fillObject = new GameObject("Fill");
            fillObject.AddComponent<RectTransform>();
            fillObject.AddComponent<UIImage>();
            fillObject.layer = LayerMask.NameToLayer("UI");

            fillObject.transform.SetParent(fillAreaObject.transform, false);
            fillObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            slider.fillRect = fillObject.GetComponent<RectTransform>();

            GameObjectUtility.SetParentAndAlign(sliderObject, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(sliderObject, "Create " + sliderObject.name);
            Selection.activeObject = sliderObject;
        }

        [MenuItem("GameObject/UI/olimsko/UIText", false, 5)]
        private static void CreateUIText(MenuCommand menuCommand)
        {
            CreateCanvasAndEventSystemIfNeeded();

            GameObject textObject = new GameObject("UIText");
            textObject.AddComponent<RectTransform>();
            textObject.layer = LayerMask.NameToLayer("UI");
            UIText text = textObject.AddComponent<UIText>();
            text.text = "New Text";
            text.raycastTarget = false;
            text.fontSize = 32;
            text.rectTransform.anchoredPosition = Vector2.zero;

            GameObjectUtility.SetParentAndAlign(textObject, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(textObject, "Create " + textObject.name);
            Selection.activeObject = textObject;
        }

        private static GameObject CreateNewUIGameObject(string name)
        {

            GameObject go = new GameObject(name);

            if (Selection.activeGameObject == null)
            {
                Canvas canvas = GameObject.FindObjectOfType<Canvas>();

                if (canvas != null)
                {
                    go.transform.SetParent(canvas.transform);
                    go.transform.localPosition = Vector3.zero;
                }
            }

            else
            {
                go.transform.SetParent(Selection.activeGameObject.transform);
            }

            Undo.RegisterCreatedObjectUndo(go, $"Create {name}");

            Selection.activeGameObject = go;

            return go;
        }

        private static void CreateCanvasAndEventSystemIfNeeded()
        {
            if (PrefabUtility.IsPartOfAnyPrefab(Selection.activeObject))
            {
                return;
            }

            Canvas canvas = GameObject.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObject = new GameObject("Canvas");
                canvas = canvasObject.AddComponent<Canvas>();
                canvasObject.AddComponent<CanvasScaler>();
                canvasObject.AddComponent<GraphicRaycaster>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObject.layer = LayerMask.NameToLayer("UI");
            }

            UnityEngine.EventSystems.EventSystem eventSystem = GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystem == null)
            {
                GameObject eventSystemObject = new GameObject("EventSystem");
                eventSystemObject.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemObject.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
        }

        private class DoCopyAsset : EndNameEditAction
        {
            public override void Action(int instanceId, string targetPath, string sourcePath)
            {
                AssetDatabase.CopyAsset(sourcePath, targetPath);
                var newAsset = AssetDatabase.LoadAssetAtPath<GameObject>(targetPath);

                UIConfiguration uIConfig = ConfigurationProvider.LoadOrDefault<UIConfiguration>();
                Canvas uIViewCanvas = newAsset.GetComponent<Canvas>();
                uIViewCanvas.renderMode = uIConfig.UIRenderMode;

                var CanvasScaler = newAsset.GetComponent<CanvasScaler>();
                CanvasScaler.referenceResolution = uIConfig.DefaultScreenSize;
                CanvasScaler.matchWidthOrHeight = uIConfig.Match;

                uIConfig.ListPrefabs.Add(new PrefabResources(newAsset.name, newAsset));

                if (uIConfig.MakeScriptWhenAddPrefab)
                {
                    string scriptName = newAsset.name;
                    string scriptPath = $"{targetPath.Substring(0, targetPath.LastIndexOf("/"))}/" + scriptName + ".cs";
                    string scriptContents = GetScriptTemplate(newAsset.name);
                    File.WriteAllText(scriptPath, scriptContents);
                    AssetDatabase.ImportAsset(scriptPath, ImportAssetOptions.ForceUpdate);

                    EditorPrefs.SetBool("olimsko_CustomUI_AddScript", true);
                    EditorPrefs.SetString("olimsko_CustomUIPrefab_TargetPath", targetPath);
                    EditorPrefs.SetString("olimsko_CustomUIScript_TargetPath", scriptPath);

                    AssetDatabase.Refresh();
                }

                ProjectWindowUtil.ShowCreatedAsset(newAsset);
            }
        }

        private static void CreatePrefabCopy(string prefabPath, string copyName)
        {
            var assetPath = BaseUtil.AbsoluteToAssetPath(BaseUtil.CombinePath(BasePath.PrefabsPath, $"{prefabPath}.prefab"));
            CreateAssetCopy(assetPath, copyName);
        }

        private static void CreateAssetCopy(string assetPath, string copyName)
        {
            var targetPath = $"{copyName}.prefab";
            var endAction = ScriptableObject.CreateInstance<DoCopyAsset>();
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, endAction, targetPath, null, assetPath);
        }

        private static string GetScriptTemplate(string className)
        {
            return
                "using UnityEngine;\n" +
                "using UnityEngine.UI;\n" +
                "using olimsko;\n\n" +
                $"public class {className} : UIView\n" +
                "{\n" +
                "    // Add your UI logic here\n" +
                "    protected override void Awake()\n" +
                "    {\n" +
                "        base.Awake();\n" +
                "    }\n\n" +
                "    protected override void OnShow()\n" +
                "    {\n" +
                "        base.OnShow();\n" +
                "    }\n\n" +
                "    protected override void OnHide()\n" +
                "    {\n" +
                "        base.OnHide();\n" +
                "    }\n\n" +
                "}";
        }
    }
}
