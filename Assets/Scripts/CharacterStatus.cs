using UnityEngine;
using System.Collections;
using System;
using olimsko;

public class CharacterStatus : MonoBehaviour
{
    private bool archerSkill9 = false;
    public bool archerSkill7 = false;
    public string character;
    private float maxHP;
    private float curHP = 0;
    private float maxShield;
    private float curShield;
    public bool isShield;
    public float shieldTime;
    public float shieldCurTime;
    [SerializeField]
    private float speed;
    public float Speed
    {
        get
        {
            return speed;
        }
        set
        {
            speed = value;
        }
    }
    [SerializeField]
    private float curSpeed;
    public float CurSpeed
    {
        get
        {
            curSpeed = speed;
            
            if(GetComponent<Player>() && archerSkill9 && GameManager.instance.skillContext.ArcherSkill9()!=0){
                curSpeed += Speed * GameManager.instance.skillContext.ArcherSkill9();
            }

            return curSpeed;
        }
        set
        {
            curSpeed = value;
        }
    }
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
    [SerializeField]
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
            
            // 궁수 액티브 스킬
            if(GetComponent<Player>() && archerSkill7){
                curCritRate += 0.3f;
                
                // 궁수 15스킬 액티브의 크리티컬 확률 레벨당 1% 증가
                curCritRate += GameManager.instance.skillContext.ArcherSkill15();
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
    [SerializeField]
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
            
            // 궁수 7스킬 크리티컬 확률 및 데미지 증가
            if(GetComponent<Player>() && archerSkill7 && character.Equals("궁수") && GameManager.instance.skillContext.ArcherSkill7()!=0){
                curCritDamage += GameManager.instance.skillContext.ArcherSkill7();
            }

            return curCritDamage;
        }
        set
        {
            curCritDamage = value;
        }
    }
    public float def;
    private float evasion;
    private float curEvasion;
    public float Evasion
    {
        get
        {
            return evasion;
        }
        set
        {
            evasion = value;
        }
    }
    public float CurEvasion
    {
        get
        {
            curEvasion = evasion;

            // 궁수 13번 스킬 회피율 1% 증가
            if(GetComponent<Player>() && character.Equals("궁수")){
                curEvasion += GameManager.instance.skillContext.ArcherSkill13();
            }

            return curEvasion;
        }
        set
        {
            curEvasion = value;
        }
    }
    public float heal;

    private float attackDamage;
    private float curAttackDamage;
    private float activeSkillDamage;
    private float curActiveSkillDamage;
    private float attackSpeed = 1;
    private float curAttackSpeed;
    private float curActiveDelay;
    public float CurActiveDelay
    {
        get
        {
            curActiveDelay = 1;

            if(GetComponent<Player>())
            {
                // 궁수 2번 스킬 액티브 스킬 딜레이 15% 감소
                curActiveDelay += GameManager.instance.skillContext.ArcherSkill2();
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
                // 궁수 1스킬 공속 증가
                curAttackSpeed += GameManager.instance.skillContext.ArcherSkill1();
                // 궁수 12스킬 공속 증가
                curAttackSpeed += GameManager.instance.skillContext.ArcherSkill12(attackSpeed);
            
            if(GetComponent<Player>()){
                // 궁수 2스킬 파티원 전체 공속 증가
                curAttackSpeed += GameManager.instance.skillContext.ArcherSkill2();
            }

            if(GetComponent<Player>() && archerSkill9 && GameManager.instance.skillContext.ArcherSkill9()!=0){
                // 궁수 9스킬 30초마다 10초간 파티원 전체 공속 증가
                curAttackSpeed += GameManager.instance.skillContext.ArcherSkill9();
            }

            if(GetComponent<Player>() && character.Equals("사제")){
                // 사제 1스킬 공속 증가
                curAttackSpeed += GameManager.instance.skillContext.PriestSkill1();
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
            
            if(GetComponent<Player>() && character.Equals("궁수")){
                // 궁수 12스킬 공격력 5% 증가
                curAttackDamage += GameManager.instance.skillContext.ArcherSkill12(attackDamage);
            }

            if(GetComponent<Player>() && character.Equals("사제")){
                // 사제 0스킬 데미지 50% 증가
                curAttackDamage += GameManager.instance.skillContext.PriestSkill0(attackDamage);
            }

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

    public IEnumerator ArcherSkill9Buff()
    {
        archerSkill9 = true;
        yield return new WaitForSeconds(10f);
        archerSkill9 = false;
    }
    float archerSkill9Count = 0;
    void Update()
    {
        if(GameManager.instance.skillContext.ArcherSkill9()!=0){
            archerSkill9Count -= Time.deltaTime;
            if(archerSkill9Count<=0){
                archerSkill9Count = 40;
                StartCoroutine(ArcherSkill9Buff());
            }
        }
    }
}
