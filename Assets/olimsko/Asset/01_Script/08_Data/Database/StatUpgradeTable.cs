using System;
using UnityEngine;
using olimsko;

public enum StatType
{
    Damage,
    CritRate,
    CritDamage,
    AttSpeed,
    AttRange,
    Heal,
    HP,
    Def,
    HPRegen,
    Evasion,
    Vamp,
    MoveSpeed,
    DamageReduction,
    ElementalDamage,
    ActiveDamage,
    ActiveCooldown,
    NumberOfProjectile,
    ProjectilePenetration,
    EXP,
    PickUpRange,
    Passive1,
    Passive2,
    Passive3,
    Default
}

[Serializable]
public class StatUpgradeTable : ITableData<StatType>
{
    [SerializeField] private StatType m_Name;
    [SerializeField] private string m_Desc;
    [SerializeField] private float m_MinValue;
    [SerializeField] private float m_MaxValue;
    [SerializeField] private float m_UpgradeValue;
    [SerializeField] private int m_UpgradeCount;
    [SerializeField] private int m_UpgradeGold;
    [SerializeField] private int m_UpgradeStoneIndex;
    [SerializeField] private int m_UpgradeStone;

    public StatUpgradeTable() { }

    public StatUpgradeTable(StatType name, string desc, float minvalue, float maxvalue, float upgradevalue, int upgradecount, int upgradegold, int upgradestoneindex, int upgradestone)
    {
        m_Name = name;
        m_Desc = desc;
        m_MinValue = minvalue;
        m_MaxValue = maxvalue;
        m_UpgradeValue = upgradevalue;
        m_UpgradeCount = upgradecount;
        m_UpgradeGold = upgradegold;
        m_UpgradeStoneIndex = upgradestoneindex;
        m_UpgradeStone = upgradestone;
    }

    public StatType Name { get => m_Name; set => m_Name = value; }
    public string Desc { get => m_Desc; set => m_Desc = value; }
    public float MinValue { get => m_MinValue; set => m_MinValue = value; }
    public float MaxValue { get => m_MaxValue; set => m_MaxValue = value; }
    public float UpgradeValue { get => m_UpgradeValue; set => m_UpgradeValue = value; }
    public int UpgradeCount { get => m_UpgradeCount; set => m_UpgradeCount = value; }
    public int UpgradeGold { get => m_UpgradeGold; set => m_UpgradeGold = value; }
    public int UpgradeStoneIndex { get => m_UpgradeStoneIndex; set => m_UpgradeStoneIndex = value; }
    public int UpgradeStone { get => m_UpgradeStone; set => m_UpgradeStone = value; }

    public StatType GetKey()
    {
        return Name;
    }

    public void SetDataFromRow(string[,] data, int row)
    {
        m_Name = string.IsNullOrEmpty(data[row, 0]) ? StatType.Default : (StatType)Enum.Parse(typeof(StatType), data[row, 0]);
        m_Desc = data[row, 1];
        m_MinValue = string.IsNullOrEmpty(data[row, 2]) ? default : float.Parse(data[row, 2]);
        m_MaxValue = string.IsNullOrEmpty(data[row, 3]) ? default : float.Parse(data[row, 3]);
        m_UpgradeValue = string.IsNullOrEmpty(data[row, 4]) ? default : float.Parse(data[row, 4]);
        m_UpgradeCount = string.IsNullOrEmpty(data[row, 5]) ? default : int.Parse(data[row, 5]);
        m_UpgradeGold = string.IsNullOrEmpty(data[row, 6]) ? default : int.Parse(data[row, 6]);
        m_UpgradeStoneIndex = string.IsNullOrEmpty(data[row, 7]) ? default : int.Parse(data[row, 7]);
        m_UpgradeStone = string.IsNullOrEmpty(data[row, 8]) ? default : int.Parse(data[row, 8]);
    }
}
