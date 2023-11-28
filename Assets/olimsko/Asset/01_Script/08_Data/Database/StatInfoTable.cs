using System;
using UnityEngine;
using olimsko;

[Serializable]
public class StatInfoTable : ITableData<int>
{
    [SerializeField] private int m_Idx;
    [SerializeField] private string m_Name;
    [SerializeField] private string m_Desc;
    [SerializeField] private float m_MinValue;
    [SerializeField] private float m_MaxValue;
    [SerializeField] private float m_UpgradeValue;

    public StatInfoTable() { }

    public StatInfoTable(int idx, string name, string desc, float minvalue, float maxvalue, float upgradevalue)
    {
        m_Idx = idx;
        m_Name = name;
        m_Desc = desc;
        m_MinValue = minvalue;
        m_MaxValue = maxvalue;
        m_UpgradeValue = upgradevalue;
    }

    public int Idx { get => m_Idx; set => m_Idx = value; }
    public string Name { get => m_Name; set => m_Name = value; }
    public string Desc { get => m_Desc; set => m_Desc = value; }
    public float MinValue { get => m_MinValue; set => m_MinValue = value; }
    public float MaxValue { get => m_MaxValue; set => m_MaxValue = value; }
    public float UpgradeValue { get => m_UpgradeValue; set => m_UpgradeValue = value; }

    public int GetKey()
    {
        return Idx;
    }

    public void SetDataFromRow(string[,] data, int row)
    {
        m_Idx = string.IsNullOrEmpty(data[row, 0]) ? default : int.Parse(data[row, 0]);
        m_Name = data[row, 1];
        m_Desc = data[row, 2];
        m_MinValue = string.IsNullOrEmpty(data[row, 3]) ? default : float.Parse(data[row, 3]);
        m_MaxValue = string.IsNullOrEmpty(data[row, 4]) ? default : float.Parse(data[row, 4]);
        m_UpgradeValue = string.IsNullOrEmpty(data[row, 5]) ? default : float.Parse(data[row, 5]);
    }
}
