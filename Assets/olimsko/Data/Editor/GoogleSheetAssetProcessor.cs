using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UI;

namespace olimsko
{
    public class GoogleSheetAssetProcessor : UnityEditor.AssetModificationProcessor
    {
        [DidReloadScripts(100)]
        private static void OnScriptsReloaded()
        {
            if (EditorPrefs.GetBool("olimsko_GoogleSheetSO_AddScript", false))
            {
                EditorPrefs.DeleteKey("olimsko_GoogleSheetSO_AddScript");

                string targetPrefabPath = EditorPrefs.GetString("olimsko_GoogleSheetSO_TargetPath");
                string className = EditorPrefs.GetString("olimsko_GoogleSheetSO_ClassName");
                string sheetId = EditorPrefs.GetString("olimsko_GoogleSheetSO_SheetID");

                EditorPrefs.DeleteKey("olimsko_GoogleSheetSO_TargetPath");
                EditorPrefs.DeleteKey("olimsko_GoogleSheetSO_ClassName");
                EditorPrefs.DeleteKey("olimsko_GoogleSheetSO_SheetID");

                ScriptableObject instance = ScriptableObject.CreateInstance(className);
                AssetDatabase.CreateAsset(instance, targetPrefabPath);

                GoogleSheetData googleSheetData = (GoogleSheetData)instance;
                ConfigurationProvider.LoadOrDefault<DataConfiguration>().ListGoogleSheetData.Add(new GoogleSheetDataList(sheetId, googleSheetData));

                AssetDatabase.Refresh();

                ProjectWindowUtil.ShowCreatedAsset(instance);
            }
        }


    }
}
