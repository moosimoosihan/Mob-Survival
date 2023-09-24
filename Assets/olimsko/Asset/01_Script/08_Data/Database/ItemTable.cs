using System;
using UnityEngine;
using olimsko;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

public enum ItemGrade
{
    Normal,
    Rare,
    Unique,
    Legendary,
    Default
}

public enum StatAddType
{
    Addition,
    Subtraction,
    Multiplication,
    Division,
    Is,
    Default
}

[Serializable]
public class ItemTable : ITableData<int>
{
    [SerializeField] private int m_Idx;
    [SerializeField] private string m_Name;
    [SerializeField] private string m_Desc;
    [SerializeField] private ItemGrade m_Grade;
    [SerializeField] private StatType m_StatType1;
    [SerializeField] private StatAddType m_StatAddType1;
    [SerializeField] private int m_StatValue1;
    [SerializeField] private StatType m_StatType2;
    [SerializeField] private StatAddType m_StatAddType2;
    [SerializeField] private int m_StatValue2;
    [SerializeField] private StatType m_StatType3;
    [SerializeField] private StatAddType m_StatAddType3;
    [SerializeField] private int m_StatValue3;
    private Sprite m_Sprite;

    public ItemTable() { }

    public ItemTable(int idx, string name, string desc, ItemGrade grade, StatType stattype1, StatAddType stataddtype1, int statvalue1, StatType stattype2, StatAddType stataddtype2, int statvalue2, StatType stattype3, StatAddType stataddtype3, int statvalue3)
    {
        m_Idx = idx;
        m_Name = name;
        m_Desc = desc;
        m_Grade = grade;
        m_StatType1 = stattype1;
        m_StatAddType1 = stataddtype1;
        m_StatValue1 = statvalue1;
        m_StatType2 = stattype2;
        m_StatAddType2 = stataddtype2;
        m_StatValue2 = statvalue2;
        m_StatType3 = stattype3;
        m_StatAddType3 = stataddtype3;
        m_StatValue3 = statvalue3;
        m_Sprite = null;
    }

    public int Idx { get => m_Idx; set => m_Idx = value; }
    public string Name { get => m_Name; set => m_Name = value; }
    public string Desc { get => m_Desc; set => m_Desc = value; }
    public ItemGrade Grade { get => m_Grade; set => m_Grade = value; }
    public StatType StatType1 { get => m_StatType1; set => m_StatType1 = value; }
    public StatAddType StatAddType1 { get => m_StatAddType1; set => m_StatAddType1 = value; }
    public int StatValue1 { get => m_StatValue1; set => m_StatValue1 = value; }
    public StatType StatType2 { get => m_StatType2; set => m_StatType2 = value; }
    public StatAddType StatAddType2 { get => m_StatAddType2; set => m_StatAddType2 = value; }
    public int StatValue2 { get => m_StatValue2; set => m_StatValue2 = value; }
    public StatType StatType3 { get => m_StatType3; set => m_StatType3 = value; }
    public StatAddType StatAddType3 { get => m_StatAddType3; set => m_StatAddType3 = value; }
    public int StatValue3 { get => m_StatValue3; set => m_StatValue3 = value; }

    public int GetKey()
    {
        return Idx;
    }

    public async UniTask<Sprite> GetItemSprite()
    {
        if (m_Sprite == null)
        {
            m_Sprite = await Addressables.LoadAssetAsync<Sprite>($"InventoryItem/{Idx}.png");
        }

        return m_Sprite;
    }

    public void SetDataFromRow(string[,] data, int row)
    {
        m_Idx = string.IsNullOrEmpty(data[row, 0]) ? default : int.Parse(data[row, 0]);
        m_Name = data[row, 1];
        m_Desc = data[row, 2];
        m_Grade = string.IsNullOrEmpty(data[row, 3]) ? ItemGrade.Default : (ItemGrade)Enum.Parse(typeof(ItemGrade), data[row, 3]);
        m_StatType1 = string.IsNullOrEmpty(data[row, 4]) ? StatType.Default : (StatType)Enum.Parse(typeof(StatType), data[row, 4]);
        m_StatAddType1 = string.IsNullOrEmpty(data[row, 5]) ? StatAddType.Default : (StatAddType)Enum.Parse(typeof(StatAddType), data[row, 5]);
        m_StatValue1 = string.IsNullOrEmpty(data[row, 6]) ? default : int.Parse(data[row, 6]);
        m_StatType2 = string.IsNullOrEmpty(data[row, 7]) ? StatType.Default : (StatType)Enum.Parse(typeof(StatType), data[row, 7]);
        m_StatAddType2 = string.IsNullOrEmpty(data[row, 8]) ? StatAddType.Default : (StatAddType)Enum.Parse(typeof(StatAddType), data[row, 8]);
        m_StatValue2 = string.IsNullOrEmpty(data[row, 9]) ? default : int.Parse(data[row, 9]);
        m_StatType3 = string.IsNullOrEmpty(data[row, 10]) ? StatType.Default : (StatType)Enum.Parse(typeof(StatType), data[row, 10]);
        m_StatAddType3 = string.IsNullOrEmpty(data[row, 11]) ? StatAddType.Default : (StatAddType)Enum.Parse(typeof(StatAddType), data[row, 11]);
        m_StatValue3 = string.IsNullOrEmpty(data[row, 12]) ? default : int.Parse(data[row, 12]);
    }
}
