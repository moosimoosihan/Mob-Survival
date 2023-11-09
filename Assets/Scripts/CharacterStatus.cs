using UnityEngine;
using System.Collections;
using System;
using olimsko;

public class CharacterStatus : MonoBehaviour
{
    public string character;
    private float maxHP;
    private float curHP = 0;
    private float maxShield;
    private float curShield;
    public bool isShield;
    public float shieldTime;
    public float shieldCurTime;
    public float speed;
    [SerializeField]
    private float critRate;
    public float CritRate
    {
        get
        {
            return critRate;
        }
        set
        {
            critRate = value;
        }
    }
    private float curCritRate;
    public float CurCritRate
    {
        get
        {
            curCritRate = critRate;
            
            // 궁수 8스킬 크리티컬 확률 및 데미지 증가
            if(GetComponent<Player>() && character.Equals("궁수")){
                curCritRate += GameManager.instance.skillContext.ArcherSkill8()[0];
            }

            return curCritRate;
        }
        set
        {
            curCritRate = value;
        }
    }
    [SerializeField]
    private float critDamage;
    public float CritDamage
    {
        get
        {
            return critDamage;
        }
        set
        {
            critDamage = value;
        }
    }
    private float curCritDamage;
    public float CurCritDamage
    {
        get
        {
            curCritDamage = critDamage;

            // 궁수 8스킬 크리티컬 확률 및 데미지 증가
            if(GetComponent<Player>() && character.Equals("궁수")){
                curCritDamage += GameManager.instance.skillContext.ArcherSkill8()[1];
            }

            return curCritDamage;
        }
        set
        {
            curCritDamage = value;
        }
    }
    public float def;
    public float evasion;
    public float heal;

    private float attackDamage;
    private float curAttackDamage;
    private float activeSkillDamage;
    private float curActiveSkillDamage;
    private float attackSpeed = 1;
    private float curAttackSpeed;
    private float activeDelay = 1;
    private float curActiveDelay;
    public float ActiveDelay
    {
        get
        {
            return activeDelay;
        }
        set
        {
            activeDelay = value;
        }
    }
    public float CurActiveDelay
    {
        get
        {
            curActiveDelay = activeDelay;

            if(GetComponent<Player>())
            {
                // 궁수 2번 스킬 액티브 스킬 딜레이 15% 감소
                curActiveDelay += GameManager.instance.skillContext.ArcherSkill2(activeDelay);
            }

            return curActiveDelay;
        }
        set
        {
            curActiveDelay = value;
        }
    }
    public float CurAttackSpeed
    {
        get
        {
            curAttackSpeed = attackSpeed;

            //플레이어 여부 검사
            if(GetComponent<Player>() && character.Equals("궁수"))
                curAttackSpeed += GameManager.instance.skillContext.ArcherSkill1(attackSpeed);
            
            if(GetComponent<Player>()){
                curAttackSpeed += GameManager.instance.skillContext.ArcherSkill2(attackSpeed);
            }

            return curAttackSpeed;
        }
        set
        {
            curAttackSpeed = value;
        }
    }
    public float AttackSpeed
    {
        get
        {
            return attackSpeed;
        }
        set
        {
            attackSpeed = value;
        }
    }
    public float CurActiveSkillDamage
    {
        get
        {
            curActiveSkillDamage = activeSkillDamage;

            //플레이어 여부 검사
            if(GetComponent<Player>())
                // 용사 2번 스킬 파티버프 전체 데미지 15% 증가
                curActiveSkillDamage += GameManager.instance.skillContext.WarriorSkill2(activeSkillDamage);

            if(GetComponent<Player>() && character.Equals("용사")){
                // 용사 15번 스킬 액티브 스킬 15% 증가
                curActiveSkillDamage += GameManager.instance.skillContext.WarriorSkill15(activeSkillDamage);
            }
            
            return curActiveSkillDamage;
        }
        set
        {
            curActiveSkillDamage = value;
        }
    }
    public float ActiveSkillDamage
    {
        get
        {
            return activeSkillDamage;
        }
        set
        {
            activeSkillDamage = value;
        }
    }
    public float AttackDamage
    {
        get
        {
            return attackDamage;
        }
        set
        {
            attackDamage = value;
        }
    }
    public float CurAttackDamage
    {
        get
        {
            curAttackDamage = attackDamage;

            if(GetComponent<Player>())
                // 용사 2번 스킬 파티버프 전체 데미지 15% 증가
                curAttackDamage += GameManager.instance.skillContext.WarriorSkill2(attackDamage);
            
            return curAttackDamage;
        }
        set
        {
            curAttackDamage = value;
        }
    }
    public float MaxHP
    {
        get
        {
            return maxHP;
        }
        set
        {
            OnHpChanged?.Invoke();
            maxHP = value;
        }
    }

    public float CurHP
    {
        get
        {
            return curHP;
        }
        set
        {
            OnHpChanged?.Invoke();
            curHP = value;
        }
    }
    public float MaxShield
    {
        get
        {
            return maxShield;
        }
        set
        {
            OnShieldChanged?.Invoke();
            maxShield = value;
        }
    }
    public float CurShield
    {
        get
        {
            return curShield;
        }
        set
        {
            OnShieldChanged?.Invoke();
            curShield = value;
        }
    }


    public Action OnHpChanged;
    public Action OnShieldChanged;
    //public Action OnAttackDamageChanged;

    // [SerializeField]
    // bool createFollowingHpBar;

    // 속도 저항
    public float resistance = 1;

    // 버프 이펙트
    public GameObject shielSfxPlayer;
    // public void CreateFollowingHpBar()
    // {
    //     if (createFollowingHpBar)
    //     {
    //         HealthFollow followingHpBar = UISingletonManager.Instance.followingBarManager.CreateFollowingHpBar().GetComponent<HealthFollow>();
    //         followingHpBar.Init(this.gameObject);
    //     }
    // }
    public IEnumerator ShieldOn()
    {
        isShield = true;
        // 이펙트 생성해야 함
        // HealthFollow followingSdBar = UISingletonManager.Instance.followingBarManager.CreateFollowingSdBar().GetComponent<HealthFollow>();
        // followingSdBar.Init(this.gameObject);

        shielSfxPlayer = AudioManager.Instance.LoopSfxPlay(AudioManager.LoopSfx.Shield);

        shieldCurTime = shieldTime;
        while (shieldCurTime > 0)
        {
            shieldCurTime -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        shieldCurTime = 0;
        curShield = 0;

        shielSfxPlayer.GetComponent<LoopSFXPlayer>().Stop();
        //이펙트 제거해야 함
        isShield = false;
    }
    public IEnumerator Speedresistance(float _resistance, float _time)
    {
        resistance = _resistance;
        yield return new WaitForSeconds(_time);
        resistance = 1;
    }
}
