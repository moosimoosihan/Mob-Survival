using System;
using System.Collections.Generic;
using UnityEngine;
using olimsko;

[CreateAssetMenu(fileName = "MonsterTableSO", menuName = "olimsko/Data/MonsterTableSO", order = 1)]
public class MonsterTableSO : GoogleSheetData
{
    [SerializeField] public List<MonsterTable> MonsterTable = new List<MonsterTable>();
    public override void SetData(string[,] data)
    {
        SetDataToList(data, MonsterTable);
    }
}
