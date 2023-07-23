using System;
using System.Collections.Generic;
using UnityEngine;
using olimsko;

[CreateAssetMenu(fileName = "StatUpgradeTableSO", menuName = "olimsko/Data/StatUpgradeTableSO", order = 1)]
public class StatUpgradeTableSO : GoogleSheetData
{
    [SerializeField] public Dictionary<StatType,StatUpgradeTable> StatUpgradeTable = new Dictionary<StatType,StatUpgradeTable>();
    public override void SetData(string[,] data)
    {
        SetDataToList(data, StatUpgradeTable);
    }
}
