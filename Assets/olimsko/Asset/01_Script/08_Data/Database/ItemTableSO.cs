using System;
using System.Collections.Generic;
using UnityEngine;
using olimsko;

[CreateAssetMenu(fileName = "ItemTableSO", menuName = "olimsko/Data/ItemTableSO", order = 1)]
public class ItemTableSO : GoogleSheetData
{
    [SerializeField] public List<ItemTable> ItemTable = new List<ItemTable>();
    public override void SetData(string[,] data)
    {
        SetDataToList(data, ItemTable);
    }
}
