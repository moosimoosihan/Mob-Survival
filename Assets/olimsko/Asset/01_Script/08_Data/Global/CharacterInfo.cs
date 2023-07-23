using System.Collections.Generic;
using UnityEngine;
using olimsko;

public class CharacterInfo
{
    private List<Dictionary<StatType, StatUpgradeTable>> m_ListCharUpgradeStat = new List<Dictionary<StatType, StatUpgradeTable>>();

    public List<Dictionary<StatType, StatUpgradeTable>> ListCharUpgradeStat { get => m_ListCharUpgradeStat; set => m_ListCharUpgradeStat = value; }

    public CharacterInfo()
    {
        ListCharUpgradeStat = new List<Dictionary<StatType, StatUpgradeTable>>();

        CharacterTableSO characterTableSO = OSManager.GetService<DataManager>().GetData<CharacterTableSO>();
        StatUpgradeTableSO statUpgradeTableSO = OSManager.GetService<DataManager>().GetData<StatUpgradeTableSO>();

        for (int i = 0; i < characterTableSO.CharacterTable.Count; i++)
        {
            Dictionary<StatType, StatUpgradeTable> dicStat = new Dictionary<StatType, StatUpgradeTable>();

            foreach (var statUpgradeTable in statUpgradeTableSO.StatUpgradeTable)
            {
                dicStat.Add(statUpgradeTable.Key, statUpgradeTable.Value);
            }

            ListCharUpgradeStat.Add(dicStat);
        }
    }
}