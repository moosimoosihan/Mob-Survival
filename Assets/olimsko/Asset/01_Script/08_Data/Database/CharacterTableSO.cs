using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using olimsko;

[CreateAssetMenu(fileName = "CharacterTableSO", menuName = "olimsko/Data/CharacterTableSO", order = 1)]
public class CharacterTableSO : GoogleSheetData
{
    [SerializeField] public List<CharacterTable> CharacterTable = new List<CharacterTable>();

    public override void SetData(string[,] data)
    {
        SetDataToList(data, CharacterTable);
    }
}
