using System;
using UnityEngine;
using olimsko;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

[Serializable]
public class CharacterTable : ITableData<int>
{
    [SerializeField] private int m_Idx;
    [SerializeField] private string m_Name;
    [SerializeField] private string m_Desc;
    [SerializeField] private string m_NameKey;
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
    [SerializeField] private Sprite m_SpriteLD;
    [SerializeField] private Sprite m_SpriteSD;
    [SerializeField] private Sprite m_SpriteProfile;

    public CharacterTable() { }

    public CharacterTable(int idx, string name, string desc, string namekey, float damage, float critrate, float critdamage, float attackspeed, float attackrange, float heal, float hp, int def, int hpregen, float evasion, float vamp, float movespeed, float damagereduction)
    {
        m_Idx = idx;
        m_Name = name;
        m_Desc = desc;
        m_NameKey = namekey;
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
    public string NameKey { get => m_NameKey; set => m_NameKey = value; }
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

    public async UniTask<Sprite> GetLDSprite()
    {
        if (m_SpriteLD == null)
        {
            m_SpriteLD = await Addressables.LoadAssetAsync<Sprite>($"PlayerLD/{m_NameKey}LD.png");
        }

        return m_SpriteLD;
    }

    public async UniTask<Sprite> GetSDSprite()
    {
        if (m_SpriteSD == null)
        {
            m_SpriteSD = await Addressables.LoadAssetAsync<Sprite>($"PlayerSD/{m_NameKey}SD.png");
        }

        return m_SpriteSD;
    }

    public async UniTask<Sprite> GetProfileSprite()
    {
        if (m_SpriteProfile == null)
        {
            m_SpriteProfile = await Addressables.LoadAssetAsync<Sprite>($"PlayerProfile/{m_NameKey}Profile.png");
        }

        return m_SpriteProfile;
    }

    public int GetKey()
    {
        return Idx;
    }

    public void SetDataFromRow(string[,] data, int row)
    {
        m_Idx = string.IsNullOrEmpty(data[row, 0]) ? default : int.Parse(data[row, 0]);
        m_Name = data[row, 1];
        m_Desc = data[row, 2];
        m_NameKey = data[row, 3];
        m_Damage = string.IsNullOrEmpty(data[row, 4]) ? default : float.Parse(data[row, 4]);
        m_CritRate = string.IsNullOrEmpty(data[row, 5]) ? default : float.Parse(data[row, 5]);
        m_CritDamage = string.IsNullOrEmpty(data[row, 6]) ? default : float.Parse(data[row, 6]);
        m_AttackSpeed = string.IsNullOrEmpty(data[row, 7]) ? default : float.Parse(data[row, 7]);
        m_AttackRange = string.IsNullOrEmpty(data[row, 8]) ? default : float.Parse(data[row, 8]);
        m_Heal = string.IsNullOrEmpty(data[row, 9]) ? default : float.Parse(data[row, 9]);
        m_HP = string.IsNullOrEmpty(data[row, 10]) ? default : float.Parse(data[row, 10]);
        m_Def = string.IsNullOrEmpty(data[row, 11]) ? default : int.Parse(data[row, 11]);
        m_HPRegen = string.IsNullOrEmpty(data[row, 12]) ? default : int.Parse(data[row, 12]);
        m_Evasion = string.IsNullOrEmpty(data[row, 13]) ? default : float.Parse(data[row, 13]);
        m_Vamp = string.IsNullOrEmpty(data[row, 14]) ? default : float.Parse(data[row, 14]);
        m_MoveSpeed = string.IsNullOrEmpty(data[row, 15]) ? default : float.Parse(data[row, 15]);
        m_DamageReduction = string.IsNullOrEmpty(data[row, 16]) ? default : float.Parse(data[row, 16]);
    }
}
