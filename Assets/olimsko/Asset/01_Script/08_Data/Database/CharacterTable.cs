using System;
using UnityEngine;
using olimsko;

[Serializable]
public class CharacterTable : ITableData<int>
{
    [SerializeField] private int m_Idx;
    [SerializeField] private string m_Name;
    [SerializeField] private string m_Desc;
    [SerializeField] private float m_Damage;
    [SerializeField] private float m_CritRate;
    [SerializeField] private float m_CritDamage;
    [SerializeField] private float m_AttackSpeed;
    [SerializeField] private float m_AttackRange;
    [SerializeField] private float m_Heal;
    [SerializeField] private float m_HP;
    [SerializeField] private int m_Def;
    [SerializeField] private int m_HPRegen;
    [SerializeField] private float m_Evasion;
    [SerializeField] private float m_Vamp;
    [SerializeField] private float m_MoveSpeed;
    [SerializeField] private float m_DamageReduction;

    public CharacterTable() { }

    public CharacterTable(int idx, string name, string desc, float damage, float critrate, float critdamage, float attackspeed, float attackrange, float heal, float hp, int def, int hpregen, float evasion, float vamp, float movespeed, float damagereduction)
    {
        m_Idx = idx;
        m_Name = name;
        m_Desc = desc;
        m_Damage = damage;
        m_CritRate = critrate;
        m_CritDamage = critdamage;
        m_AttackSpeed = attackspeed;
        m_AttackRange = attackrange;
        m_Heal = heal;
        m_HP = hp;
        m_Def = def;
        m_HPRegen = hpregen;
        m_Evasion = evasion;
        m_Vamp = vamp;
        m_MoveSpeed = movespeed;
        m_DamageReduction = damagereduction;
    }

    public int Idx { get => m_Idx; set => m_Idx = value; }
    public string Name { get => m_Name; set => m_Name = value; }
    public string Desc { get => m_Desc; set => m_Desc = value; }
    public float Damage { get => m_Damage; set => m_Damage = value; }
    public float CritRate { get => m_CritRate; set => m_CritRate = value; }
    public float CritDamage { get => m_CritDamage; set => m_CritDamage = value; }
    public float AttackSpeed { get => m_AttackSpeed; set => m_AttackSpeed = value; }
    public float AttackRange { get => m_AttackRange; set => m_AttackRange = value; }
    public float Heal { get => m_Heal; set => m_Heal = value; }
    public float HP { get => m_HP; set => m_HP = value; }
    public int Def { get => m_Def; set => m_Def = value; }
    public int HPRegen { get => m_HPRegen; set => m_HPRegen = value; }
    public float Evasion { get => m_Evasion; set => m_Evasion = value; }
    public float Vamp { get => m_Vamp; set => m_Vamp = value; }
    public float MoveSpeed { get => m_MoveSpeed; set => m_MoveSpeed = value; }
    public float DamageReduction { get => m_DamageReduction; set => m_DamageReduction = value; }

    public int GetKey()
    {
        return Idx;
    }

    public void SetDataFromRow(string[,] data, int row)
    {
        m_Idx = int.Parse(data[row, 0]);
        m_Name = data[row, 1];
        m_Desc = data[row, 2];
        m_Damage = float.Parse(data[row, 3]);
        m_CritRate = float.Parse(data[row, 4]);
        m_CritDamage = float.Parse(data[row, 5]);
        m_AttackSpeed = float.Parse(data[row, 6]);
        m_AttackRange = float.Parse(data[row, 7]);
        m_Heal = float.Parse(data[row, 8]);
        m_HP = float.Parse(data[row, 9]);
        m_Def = int.Parse(data[row, 10]);
        m_HPRegen = int.Parse(data[row, 11]);
        m_Evasion = float.Parse(data[row, 12]);
        m_Vamp = float.Parse(data[row, 13]);
        m_MoveSpeed = float.Parse(data[row, 14]);
        m_DamageReduction = float.Parse(data[row, 15]);
    }
}
