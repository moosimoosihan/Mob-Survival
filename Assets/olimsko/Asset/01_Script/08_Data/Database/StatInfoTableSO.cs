using UnityEngine;
using olimsko;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StatInfoTableSO", menuName = "olimsko/Data/StatInfoTableSO", order = 1)]
public class StatInfoTableSO : GoogleSheetData
{
    [SerializeField] public List<StatInfoTable> StatInfoTable = new List<StatInfoTable>();

    public override void SetData(string[,] data)
    {
        SetDataToList(data, StatInfoTable);
    }
}