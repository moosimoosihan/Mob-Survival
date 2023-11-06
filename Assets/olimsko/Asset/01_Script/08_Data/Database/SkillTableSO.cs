using System;
using System.Collections.Generic;
using UnityEngine;
using olimsko;

[CreateAssetMenu(fileName = "SkillTableSO", menuName = "olimsko/Data/SkillTableSO", order = 1)]
public class SkillTableSO : GoogleSheetData
{
    [SerializeField] public List<SkillTable> SkillTable = new List<SkillTable>();
    public override void SetData(string[,] data)
    {
        SetDataToList(data, SkillTable);
    }
}
