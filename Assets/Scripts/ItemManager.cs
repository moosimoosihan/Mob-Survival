using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField]
    TextAsset itemDatabase;
    [SerializeField]
    public List<ItemData> itemDataList = new List<ItemData>();
    public GameObject itemPrefab;
    public Sprite[] itemSprite;
    public int itemPoolMaxSize;
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
            tempItemSpawnData.spriteNum = i;

            itemDataList.Add(tempItemSpawnData);
        }
    }
}
[System.Serializable]
public class ItemData
{
    public string itemName;
    public int count; // 골드 혹은 경험치 추가되는 양
    public int spriteNum;
}