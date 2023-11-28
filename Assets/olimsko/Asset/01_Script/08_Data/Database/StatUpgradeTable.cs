using System;
using UnityEngine;
using olimsko;

public enum StatType
{
    Damage,
    CritRate,
    CritDamage,
    AttSpeed,
    ActiveDamage,
    ActiveCooldown,
    Heal,
    AttRange,
    HPRegen,
    Evasion,
    Vamp,
    DamageReduction,
    ElementalDamage,
    NumberOfProjectile,
    ProjectilePenetration,
    EXP,
    PickUpRange,
    HP,
    Def,
    MoveSpeed,
    Passive1,
    Passive2,
    Passive3,
    Default
}

[Serializable]
public class StatUpgradeTable : ITableData<int>
{
    [SerializeField] private int m_Index;
    [SerializeField] private int m_CharID;
    [SerializeField] private StatType m_Stat;
    [SerializeField] private int m_Level;
    [SerializeField] private float m_UpgradeValue;
    [SerializeField] private float m_UpgradeValue2;
    [SerializeField] private int m_UpgradeGold;
    [SerializeField] private int m_UpgradeStoneIndex;
    [SerializeField] private int m_UpgradeStoneCount;

    public StatUpgradeTable() { }

    public StatUpgradeTable(int index, int charid, StatType stat, int level, float upgradevalue, float upgradevalue2, int upgradegold, int upgradestoneindex, int upgradestonecount)
    {
        m_Index = index;
        m_CharID = charid;
        m_Stat = stat;
        m_Level = level;
        m_UpgradeValue = upgradevalue;
        m_UpgradeValue2 = upgradevalue2;
        m_UpgradeGold = upgradegold;
        m_UpgradeStoneIndex = upgradestoneindex;
        m_UpgradeStoneCount = upgradestonecount;
    }

    public int Index { get => m_Index; set => m_Index = value; }
    public int CharID { get => m_CharID; set => m_CharID = value; }
    public StatType Stat { get => m_Stat; set => m_Stat = value; }
    public int Level { get => m_Level; set => m_Level = value; }
    public float UpgradeValue { get => m_UpgradeValue; set => m_UpgradeValue = value; }
    public float UpgradeValue2 { get => m_UpgradeValue2; set => m_UpgradeValue2 = value; }
    public int UpgradeGold { get => m_UpgradeGold; set => m_UpgradeGold = value; }
    public int UpgradeStoneIndex { get => m_UpgradeStoneIndex; set => m_UpgradeStoneIndex = value; }
    public int UpgradeStoneCount { get => m_UpgradeStoneCount; set => m_UpgradeStoneCount = value; }

    public int GetKey()
    {
        return Index;
    }

    public void SetDataFromRow(string[,] data, int row)
    {
        m_Index = string.IsNullOrEmpty(data[row, 0]) ? default : int.Parse(data[row, 0]);
        m_CharID = string.IsNullOrEmpty(data[row, 1]) ? default : int.Parse(data[row, 1]);
        m_Stat = string.IsNullOrEmpty(data[row, 2]) ? StatType.Default : (StatType)Enum.Parse(typeof(StatType), data[row, 2]);
        m_Level = string.IsNullOrEmpty(data[row, 3]) ? default : int.Parse(data[row, 3]);
        m_UpgradeValue = string.IsNullOrEmpty(data[row, 4]) ? default : float.Parse(data[row, 4]);
        m_UpgradeValue2 = string.IsNullOrEmpty(data[row, 5]) ? default : float.Parse(data[row, 5]);
        m_UpgradeGold = string.IsNullOrEmpty(data[row, 6]) ? default : int.Parse(data[row, 6]);
        m_UpgradeStoneIndex = string.IsNullOrEmpty(data[row, 7]) ? default : int.Parse(data[row, 7]);
        m_UpgradeStoneCount = string.IsNullOrEmpty(data[row, 8]) ? default : int.Parse(data[row, 8]);
    }
}
