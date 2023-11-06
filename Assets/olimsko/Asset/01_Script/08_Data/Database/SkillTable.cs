using System;
using UnityEngine;
using olimsko;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

public enum SkillType
{
    BasicSkill,
    UniqueSkill,
    LevelSkill,
    Default
}

[Serializable]
public class SkillTable : ITableData<int>
{
    [SerializeField] private int m_Idx;
    [SerializeField] private int m_CharacterID;
    [SerializeField] private int m_SkillID;
    [SerializeField] private string m_Name;
    [SerializeField] private string m_Desc;
    [SerializeField] private SkillType m_Type;
    [SerializeField] private float[] m_Value;
    [SerializeField] private Sprite m_Icon;

    public SkillTable() { }

    public SkillTable(int idx, int characterid, int skillid, string name, string desc, SkillType type, float[] value)
    {
        m_Idx = idx;
        m_CharacterID = characterid;
        m_SkillID = skillid;
        m_Name = name;
        m_Desc = desc;
        m_Type = type;
        m_Value = value;
    }

    public int Idx { get => m_Idx; set => m_Idx = value; }
    public int CharacterID { get => m_CharacterID; set => m_CharacterID = value; }
    public int SkillID { get => m_SkillID; set => m_SkillID = value; }
    public string Name { get => m_Name; set => m_Name = value; }
    public string Desc { get => m_Desc; set => m_Desc = value; }
    public SkillType Type { get => m_Type; set => m_Type = value; }
    public float[] Value { get => m_Value; set => m_Value = value; }

    public int GetKey()
    {
        return Idx;
    }

    public async UniTask<Sprite> GetIcon()
    {
        if (m_Icon == null)
        {
            m_Icon = await Addressables.LoadAssetAsync<Sprite>($"SkillIcon/{m_Idx}.png");
        }

        return m_Icon;
    }

    public void SetDataFromRow(string[,] data, int row)
    {
        m_Idx = string.IsNullOrEmpty(data[row, 0]) ? default : int.Parse(data[row, 0]);
        m_CharacterID = string.IsNullOrEmpty(data[row, 1]) ? default : int.Parse(data[row, 1]);
        m_SkillID = string.IsNullOrEmpty(data[row, 2]) ? default : int.Parse(data[row, 2]);
        m_Name = $"Str_SkillNameKey_{m_Idx}";
        m_Desc = $"Str_SkillDescKey_{m_Idx}";
        m_Type = string.IsNullOrEmpty(data[row, 5]) ? SkillType.Default : (SkillType)Enum.Parse(typeof(SkillType), data[row, 5]);

        string[] temp = data[row, 6].Split('/');
        m_Value = new float[temp.Length];

        for (int i = 0; i < temp.Length; i++)
        {
            m_Value[i] = string.IsNullOrEmpty(temp[i]) ? default : float.Parse(temp[i]);
        }
    }
}
