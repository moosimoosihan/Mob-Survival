using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField]
    TextAsset itemDatabase;
    [SerializeField]
    public List<ItemData> itemDataList = new List<ItemData>();
    
    void Awake()
    {
        // 아이템 데이터 불러오기
        string seperator = "\r\n";
        string[] lines = itemDatabase.text.Substring(0).Split(seperator);

        for (int i = 0; i < lines.Length; i++)
        {
            string[] rows = lines[i].Split('\t');

            //아이템 생성
            ItemData tempItemSpawnData = new ItemData();
            tempItemSpawnData.itemName = rows[0];
            tempItemSpawnData.count = System.Convert.ToInt32(rows[1]);
            tempItemSpawnData.itemPrefab = GetSpinePrefabObjFromResourceFolder("Item", tempItemSpawnData.itemName);

            itemDataList.Add(tempItemSpawnData);
        }
        GameObject GetSpinePrefabObjFromResourceFolder(string _folderName, string _prefabName)
        {
            string tempPath = $"{_folderName}/{_prefabName}";
            GameObject loadedObj = Resources.Load<GameObject>(tempPath);

            return loadedObj;
        }
    }
}
[System.Serializable]
public class ItemData
{
    public string itemName;
    public int count; // 골드 혹은 경험치 추가되는 양
    public GameObject itemPrefab;
}