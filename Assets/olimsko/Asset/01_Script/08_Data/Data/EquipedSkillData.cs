using UnityEngine;
using System.Collections.Generic;
using olimsko;
using System.Linq;

public class EquipedSkillData
{
    private int playerID;
    private Dictionary<int, EquipedSkill> m_DicEquipedSkill = new Dictionary<int, EquipedSkill>();

    private SkillTableSO SkillTableSO => OSManager.GetService<DataManager>().GetData<SkillTableSO>();

    public EquipedSkillData(int playerID)
    {
        this.playerID = playerID;
        m_DicEquipedSkill = new Dictionary<int, EquipedSkill>();
    }

    public int PlayerID { get => playerID; set => playerID = value; }
    public Dictionary<int, EquipedSkill> DicEquipedSkill { get => m_DicEquipedSkill; set => m_DicEquipedSkill = value; }

    public void AddEquipedSkill(int idx)
    {
        if (m_DicEquipedSkill.ContainsKey(idx))
        {
            m_DicEquipedSkill[idx].Level += 1;
            return;
        }
        m_DicEquipedSkill.Add(idx, new EquipedSkill(idx, 1));
    }

    public SkillType GetNeedSkillType()
    {
        SkillType skillType = SkillType.Default;

        if (m_DicEquipedSkill.Count <= 7) skillType = SkillType.BasicSkill;
        else if (m_DicEquipedSkill.Count == 8) skillType = SkillType.UniqueSkill;
        else if (m_DicEquipedSkill.Count >= 9) skillType = SkillType.LevelSkill;

        return skillType;
    }

    public int GetSkillLevel(int idx)
    {
        if (m_DicEquipedSkill.ContainsKey(idx))
            return m_DicEquipedSkill[idx].Level;
        return 0;
    }

    public bool IsNeedSkill()
    {
        if (m_DicEquipedSkill.Count >= 13 && m_DicEquipedSkill.Sum(x => x.Value.Level) >= 49) return false;
        return true;
    }
}

public class EquipedSkill
{
    private int idx;
    private int level;

    public EquipedSkill(int idx, int level)
    {
        this.idx = idx;
        this.level = level;
    }

    public EquipedSkill()
    {

    }

    public int Idx { get => idx; set => idx = value; }
    public int Level { get => level; set => level = value; }
}