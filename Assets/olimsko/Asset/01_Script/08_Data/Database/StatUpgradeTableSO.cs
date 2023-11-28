using System;
using System.Collections.Generic;
using UnityEngine;
using olimsko;

[CreateAssetMenu(fileName = "StatUpgradeTableSO", menuName = "olimsko/Data/StatUpgradeTableSO", order = 1)]
public class StatUpgradeTableSO : GoogleSheetData
{
    [SerializeField] public List<StatUpgradeTable> StatUpgradeTable = new List<StatUpgradeTable>();
    public override void SetData(string[,] data)
    {
        SetDataToList(data, StatUpgradeTable);
    }

    public StatUpgradeTable GetStatTable(int charID, StatType statType, int level)
    {
        return StatUpgradeTable.Find(x => x.CharID == charID && x.Stat == statType && x.Level == level);
    }
}
