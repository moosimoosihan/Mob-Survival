using System;
using UnityEngine;
using olimsko;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Spine.Unity;
using Cysharp.Threading.Tasks;
public enum Elemental {
    땅,
    불,
    물,
    바람,
    Default
}

[Serializable]
public class MonsterTable : ITableData<int>
{
    [SerializeField] private int m_Index;
    [SerializeField] private string m_NameKey;
    [SerializeField] private string m_Name;
    [SerializeField] private string m_Desc;
    [SerializeField] private float m_HP;
    [SerializeField] private float m_Speed;
    [SerializeField] private float m_Attack;
    [SerializeField] private float m_Def;
    [SerializeField] private float m_HPRegen;
    [SerializeField] private float m_Avoidance;
    [SerializeField] private Elemental m_Elemental;
    [SerializeField] private SkeletonDataAsset m_SkeletonDataAsset;

    public MonsterTable() { }

    public MonsterTable(int index, string namekey, string name, string desc, float hp, float speed, float attack, float def, float hpregen, float avoidance, Elemental elemental)
    {
        m_Index = index;
        m_NameKey = namekey;
        m_Name = name;
        m_Desc = desc;
        m_HP = hp;
        m_Speed = speed;
        m_Attack = attack;
        m_Def = def;
        m_HPRegen = hpregen;
        m_Avoidance = avoidance;
        m_Elemental = elemental;
    }

    public int Index { get => m_Index; set => m_Index = value; }
    public string NameKey { get => m_NameKey; set => m_NameKey = value; }
    public string Name { get => m_Name; set => m_Name = value; }
    public string Desc { get => m_Desc; set => m_Desc = value; }
    public float HP { get => m_HP; set => m_HP = value; }
    public float Speed { get => m_Speed; set => m_Speed = value; }
    public float Attack { get => m_Attack; set => m_Attack = value; }
    public float Def { get => m_Def; set => m_Def = value; }
    public float HPRegen { get => m_HPRegen; set => m_HPRegen = value; }
    public float Avoidance { get => m_Avoidance; set => m_Avoidance = value; }
    public Elemental Elemental { get => m_Elemental; set => m_Elemental = value; }

    public int GetKey()
    {
        return Index;
    }
    public async UniTask<SkeletonDataAsset> GetSkeletonDataAsset()
    {
        if (m_SkeletonDataAsset == null)
        {
            m_SkeletonDataAsset = await Addressables.LoadAssetAsync<SkeletonDataAsset>($"Monster/{m_NameKey}.asset");
        }

        return m_SkeletonDataAsset;
    }

    public void SetDataFromRow(string[,] data, int row)
    {
        m_Index = string.IsNullOrEmpty(data[row, 0]) ? default : int.Parse(data[row, 0]);
        m_NameKey = data[row, 1];
        m_Name = data[row, 2];
        m_Desc = data[row, 3];
        m_HP = string.IsNullOrEmpty(data[row, 4]) ? default : float.Parse(data[row, 4]);
        m_Speed = string.IsNullOrEmpty(data[row, 5]) ? default : float.Parse(data[row, 5]);
        m_Attack = string.IsNullOrEmpty(data[row, 6]) ? default : float.Parse(data[row, 6]);
        m_Def = string.IsNullOrEmpty(data[row, 7]) ? default : float.Parse(data[row, 7]);
        m_HPRegen = string.IsNullOrEmpty(data[row, 8]) ? default : float.Parse(data[row, 8]);
        m_Avoidance = string.IsNullOrEmpty(data[row, 9]) ? default : float.Parse(data[row, 9]);
        m_Elemental = string.IsNullOrEmpty(data[row, 10]) ? Elemental.Default : (Elemental)Enum.Parse(typeof(Elemental), data[row, 10]);
    }
}
