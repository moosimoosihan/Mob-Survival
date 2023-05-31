using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UI;

namespace olimsko
{


    public class CustomUIAssetPostprocessor : UnityEditor.AssetModificationProcessor
    {
        [DidReloadScripts(100)] // 실행 순서를 지정합니다. 작은 숫자가 먼저 실행됩니다.
        private static void OnScriptsReloaded()
        {
            if (EditorPrefs.GetBool("olimsko_CustomUI_AddScript", false))
            {
                EditorPrefs.DeleteKey("olimsko_CustomUI_AddScript");

                string targetPrefabPath = EditorPrefs.GetString("olimsko_CustomUIPrefab_TargetPath");
                GameObject newAsset = AssetDatabase.LoadAssetAtPath<GameObject>(targetPrefabPath);

                string targetScriptPath = EditorPrefs.GetString("olimsko_CustomUIScript_TargetPath");
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(targetScriptPath);

                var instance = GameObject.Instantiate(newAsset);
                instance.AddComponent(script.GetClass());

                Component newComponent = instance.GetComponent(script.GetClass());
                int indexOfTransform = instance.GetComponents<Component>().ToList().IndexOf(instance.transform);
                UnityEditorInternal.ComponentUtility.MoveComponentUp(newComponent);
                while (instance.GetComponents<Component>().ToList().IndexOf(newComponent) > indexOfTransform + 1)
                {
                    UnityEditorInternal.ComponentUtility.MoveComponentUp(newComponent);
                }

                Component existingComponent = instance.GetComponent<CustomUI>();
                if (existingComponent != null)
                {
                    UnityEngine.Object.DestroyImmediate(existingComponent, true);
                }

                PrefabUtility.SaveAsPrefabAssetAndConnect(instance, targetPrefabPath, InteractionMode.AutomatedAction);

                GameObject.DestroyImmediate(instance);
                ProjectWindowUtil.ShowCreatedAsset(newAsset);
            }
        }


    }
}
