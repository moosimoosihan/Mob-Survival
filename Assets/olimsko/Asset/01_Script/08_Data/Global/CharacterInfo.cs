using System.Collections.Generic;
using UnityEngine;
using olimsko;

public class CharacterInfo
{
    private Dictionary<int, Dictionary<StatType, int>> m_DicCharUpgradedStat = new Dictionary<int, Dictionary<StatType, int>>();

    public Dictionary<int, Dictionary<StatType, int>> DicCharUpgradedStat { get => m_DicCharUpgradedStat; set => m_DicCharUpgradedStat = value; }

    public CharacterInfo()
    {
        DicCharUpgradedStat = new Dictionary<int, Dictionary<StatType, int>>();

        CharacterTableSO characterTableSO = OSManager.GetService<DataManager>().GetData<CharacterTableSO>();
        StatUpgradeTableSO statUpgradeTableSO = OSManager.GetService<DataManager>().GetData<StatUpgradeTableSO>();

        for (int i = 0; i < characterTableSO.CharacterTable.Count; i++)
        {
            if (!DicCharUpgradedStat.ContainsKey(characterTableSO.CharacterTable[i].Idx))
            {
                DicCharUpgradedStat.Add(characterTableSO.CharacterTable[i].Idx, new Dictionary<StatType, int>());
            }

            for (int j = 0; j < statUpgradeTableSO.StatUpgradeTable.Count; j++)
            {
                if (!DicCharUpgradedStat[characterTableSO.CharacterTable[i].Idx].ContainsKey(statUpgradeTableSO.StatUpgradeTable[j].Stat))
                {
                    DicCharUpgradedStat[characterTableSO.CharacterTable[i].Idx].Add(statUpgradeTableSO.StatUpgradeTable[j].Stat, 0);
                }
            }
        }
    }
}