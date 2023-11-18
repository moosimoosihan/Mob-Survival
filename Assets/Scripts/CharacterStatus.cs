using UnityEngine;
using System.Collections;
using System;
using olimsko;

public class CharacterStatus : MonoBehaviour
{
    public bool archerSkill9 = false;
    public bool archerActiveSkill = false;
    public bool archerSkill0 = false;
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
            
            if(GetComponent<Player>()){
                if(archerSkill9 && GameManager.instance.skillContext.ArcherSkill9()!=0){
                    curSpeed += Speed * GameManager.instance.skillContext.ArcherSkill9();
                }

                if(character.Equals("궁수") && archerSkill0){
                    curSpeed += Speed * GameManager.instance.skillContext.ArcherSkill0()[1];
                }

                // 아이템 장착 이속 증가
                // 고서
                curSpeed += Speed * (GameManager.instance.skillContext.GetItemValues(3)[1]/100);
                // 깃털
                curSpeed += Speed * (GameManager.instance.skillContext.GetItemValues(24)[0]/100);
                curSpeed += Speed * (GameManager.instance.skillContext.GetItemValues(25)[0]/100);
                curSpeed += Speed * (GameManager.instance.skillContext.GetItemValues(26)[0]/100);
                curSpeed += Speed * (GameManager.instance.skillContext.GetItemValues(27)[0]/100);

                // 아이템 습득 후 버프
                if(speedPotion){
                    curSpeed += Speed * 0.5f;
                }
            }

            if(slowDeBuff && !stunDeBuff){
                if(GetComponent<Enemy>()){
                    if(!GetComponent<Enemy>().isBoss && GameManager.instance.skillContext.WizardSkill14()!=0){
                        // 현자 14스킬 슬로우 추가
                        float addSlow = GameManager.instance.skillContext.WizardSkill14();
                        curSpeed -= Speed * (slowDeBuffCount * (0.1f + addSlow));
                    }
                } else {
                    curSpeed -= Speed * (slowDeBuffCount * 0.1f);
                }
            }
            
            if(stunDeBuff){
                if(GetComponent<Enemy>()){
                    if(!GetComponent<Enemy>().isBoss){
                        curSpeed = 0;
                    }
                } else {
                    curSpeed = 0;
                }
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
            
                // 궁수 액티브 스킬
                if(archerActiveSkill){
                    curCritRate += 0.3f;
                    
                    // 궁수 15스킬 액티브의 크리티컬 확률 레벨당 1% 증가
                    curCritRate += GameManager.instance.skillContext.ArcherSkill15();
                }

                // 아이템 장착 크리티컬 확률 증가
                // 검
                curCritRate += GameManager.instance.skillContext.GetItemValues(5)[1]/100;
                curCritRate += GameManager.instance.skillContext.GetItemValues(6)[1]/100;
                curCritRate += GameManager.instance.skillContext.GetItemValues(7)[1]/100;
                // 부적
                curCritRate += GameManager.instance.skillContext.GetItemValues(20)[0]/100;
                curCritRate += GameManager.instance.skillContext.GetItemValues(21)[0]/100;
                curCritRate += GameManager.instance.skillContext.GetItemValues(22)[0]/100;
                curCritRate += GameManager.instance.skillContext.GetItemValues(23)[0]/100;
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

            if(GetComponent<Player>()){
                if(character.Equals("궁수")){
                    // 궁수 8스킬 크리티컬 확률 및 데미지 증가
                    curCritDamage += GameManager.instance.skillContext.ArcherSkill8()[1];
                    
                    // 궁수 7스킬 크리티컬 확률 및 데미지 증가
                    if(archerActiveSkill){
                        curCritDamage += GameManager.instance.skillContext.ArcherSkill7();
                    }

                    // 아이템 장착 크리티컬 데미지 증가
                    // 부적
                    curCritDamage += GameManager.instance.skillContext.GetItemValues(20)[1]/100;
                    curCritDamage += GameManager.instance.skillContext.GetItemValues(21)[1]/100;
                    curCritDamage += GameManager.instance.skillContext.GetItemValues(22)[1]/100;
                    curCritDamage += GameManager.instance.skillContext.GetItemValues(23)[1]/100;
                }
            }

            return curCritDamage;
        }
        set
        {
            curCritDamage = value;
        }
    }
    private float def;
    private float curDef;
    public float Def
    {
        get
        {
            return def;
        }
        set
        {
            def = value;
        }
    }
    public float CurDef
    {
        get
        {
            curDef = def;

            if(GetComponent<Player>()){
                // 사제 2스킬 파티원 전체 방어력 15% 증가
                curDef += GameManager.instance.skillContext.PriestSkill2(def);

                // 아이템 장착 방어력 증가
                // 갑옷
                curDef += GameManager.instance.skillContext.GetItemValues(36)[0];
                curDef += GameManager.instance.skillContext.GetItemValues(37)[0];
                curDef += GameManager.instance.skillContext.GetItemValues(38)[0];
                curDef += GameManager.instance.skillContext.GetItemValues(39)[0];
            }

            return curDef;
        }
        set
        {
            curDef = value;
        }
    }

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
    private float heal;
    public float Heal
    {
        get
        {
            return heal;
        }
        set
        {
            heal = value;
        }
    }
    private float curHeal;
    public float CurHeal
    {
        get
        {
            curHeal = heal;

            if(GetComponent<Player>()){
                // 아이템 장착 후 회복량 증가
                // 구슬
                curHeal += GameManager.instance.skillContext.GetItemValues(32)[0]/100;
                curHeal += GameManager.instance.skillContext.GetItemValues(33)[0]/100;
                curHeal += GameManager.instance.skillContext.GetItemValues(34)[0]/100;
                curHeal += GameManager.instance.skillContext.GetItemValues(35)[0]/100;
            }

            return curHeal;
        }
        set
        {
            curHeal = value;
        }
    }

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
                // 궁수 2스킬 액티브 스킬 딜레이 15% 감소
                curActiveDelay += GameManager.instance.skillContext.ArcherSkill2();

                // 사제 9스킬 액티브 스킬 딜레이 감소
                curActiveDelay += GameManager.instance.skillContext.PriestSkill9(1);

                if(character.Equals("사제")){
                    // 사제 15스킬 액티브 스킬 딜레이 감소
                    curActiveDelay += GameManager.instance.skillContext.PriestSkill15();
                }

                // 아이템 장착 액티브 스킬 딜레이 감소
                curActiveDelay += GameManager.instance.skillContext.GetItemValues(8)[1]/100;

                curActiveDelay += GameManager.instance.skillContext.GetItemValues(9)[1]/100;

                curActiveDelay += GameManager.instance.skillContext.GetItemValues(10)[1]/100;

                curActiveDelay += GameManager.instance.skillContext.GetItemValues(11)[1]/100;
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
            if(GetComponent<Player>()){
                // 궁수 2스킬 파티원 전체 공속 증가
                curAttackSpeed += GameManager.instance.skillContext.ArcherSkill2();

                
                if(isShield && CurShield > 0){
                    // 사제 5스킬 보호막이 적용된 아군 데미지 증가
                    curAttackSpeed += GameManager.instance.skillContext.PriestSkill5(attackSpeed);

                    // 사제 13스킬 보호막이 적용된 아군 공속 증가
                    curAttackSpeed += GameManager.instance.skillContext.PriestSkill13(attackSpeed);
                }

                //사제 9스킬 공속 증가
                curAttackSpeed += GameManager.instance.skillContext.PriestSkill9(attackSpeed);

                // 사제 7스킬 기적 발동시 공속 증가
                if(priestSkill7){
                    curAttackSpeed += GameManager.instance.skillContext.PriestSkill7(attackSpeed);
                }
                
                if(archerSkill9 && GameManager.instance.skillContext.ArcherSkill9()!=0){
                    // 궁수 9스킬 30초마다 10초간 파티원 전체 공속 증가
                    curAttackSpeed += GameManager.instance.skillContext.ArcherSkill9();
                }
                
                if(character.Equals("궁수")){
                    // 궁수 1스킬 공속 증가
                    curAttackSpeed += GameManager.instance.skillContext.ArcherSkill1();
                    // 궁수 12스킬 공속 증가
                    curAttackSpeed += GameManager.instance.skillContext.ArcherSkill12(attackSpeed);
                }

                if(character.Equals("현자")){
                    // 현자 1스킬 공속 증가
                    curAttackSpeed += GameManager.instance.skillContext.WizardSkill1();
                }

                if(character.Equals("사제")){
                    // 사제 0스킬 공속 증가
                    curAttackSpeed += GameManager.instance.skillContext.PriestSkill0()[0];

                    // 사제 12스킬 공속 증가
                    curAttackSpeed += GameManager.instance.skillContext.PriestSkill12();
                }

                // 아이템 장착 공속 증가
                // 고서
                curAttackSpeed += GameManager.instance.skillContext.GetItemValues(1)[1]/100;
                curAttackSpeed += GameManager.instance.skillContext.GetItemValues(2)[1]/100;
                curAttackSpeed += GameManager.instance.skillContext.GetItemValues(3)[1]/100;
                // 깃털
                curAttackSpeed += GameManager.instance.skillContext.GetItemValues(24)[1]/100;
                curAttackSpeed += GameManager.instance.skillContext.GetItemValues(25)[1]/100;
                curAttackSpeed += GameManager.instance.skillContext.GetItemValues(26)[1]/100;
                curAttackSpeed += GameManager.instance.skillContext.GetItemValues(27)[1]/100;

                // 아이템 습득 후 버프
                if(agilityPotion){
                    curAttackSpeed += 0.5f;
                }
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
            if(GetComponent<Player>()){
                // 용사 2스킬 파티버프 전체 데미지 15% 증가
                curActiveSkillDamage += GameManager.instance.skillContext.WarriorSkill2(activeSkillDamage);

                // 사제 9스킬 액티브 데미지 증가
                curActiveSkillDamage += GameManager.instance.skillContext.PriestSkill9(activeSkillDamage);
                
                if(character.Equals("용사")){
                    // 용사 15스킬 액티브 데미지 증가
                    curActiveSkillDamage += GameManager.instance.skillContext.WarriorSkill15(activeSkillDamage);
                }

                if(character.Equals("현자")){
                    // 현자 15스킬 액티브 데미지 증가
                    curActiveSkillDamage += GameManager.instance.skillContext.WizardSkill15(activeSkillDamage);
                }

                // 아이템 장착 액티브 스킬 데미지 증가
                curActiveSkillDamage += activeSkillDamage * GameManager.instance.skillContext.GetItemValues(8)[0]/100;

                curActiveSkillDamage += activeSkillDamage * GameManager.instance.skillContext.GetItemValues(9)[0]/100;

                curActiveSkillDamage += activeSkillDamage * GameManager.instance.skillContext.GetItemValues(10)[0]/100;

                curActiveSkillDamage += activeSkillDamage * GameManager.instance.skillContext.GetItemValues(11)[0]/100;
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

            if(GetComponent<Player>()){
                // 용사 2번 스킬 파티버프 전체 데미지 15% 증가
                curAttackDamage += GameManager.instance.skillContext.WarriorSkill2(attackDamage);

                // 사제 2스킬 파티원 전체 15% 증가
                curAttackDamage += GameManager.instance.skillContext.PriestSkill2(attackDamage);

                // 사제 9스킬 데미지 증가
                curAttackDamage += GameManager.instance.skillContext.PriestSkill9(attackDamage);

                if(isShield && CurShield > 0){
                    // 사제 5스킬 보호막이 적용된 아군 데미지 증가
                    curAttackDamage += GameManager.instance.skillContext.PriestSkill5(attackDamage);

                    // 사제 13스킬 보호막이 적용된 아군 데미지 증가
                    curAttackDamage += GameManager.instance.skillContext.PriestSkill13(attackDamage);
                }

                // 사제 7스킬 기적 발동시 데미지 증가
                if(priestSkill7){
                    curAttackDamage += GameManager.instance.skillContext.PriestSkill7(attackDamage);
                }
                
                if(character.Equals("용사")){
                    // 용사 12스킬 데미지 5%증가
                    curAttackDamage += GameManager.instance.skillContext.WarriorSkill12(attackDamage);
                }

                if(character.Equals("궁수")){
                    // 궁수 12스킬 공격력 5% 증가
                    curAttackDamage += GameManager.instance.skillContext.ArcherSkill12(attackDamage);
                }

                if(character.Equals("현자")){
                    // 현자 0스킬 데미지 50% 증가
                    curAttackDamage += GameManager.instance.skillContext.WizardSkill0(attackDamage)[0];

                    // 현자 8스킬 데미지 10 증가
                    curAttackDamage += GameManager.instance.skillContext.WizardSkill8()[0];

                    // 현자 12스킬 데미지 증가
                    curAttackDamage += GameManager.instance.skillContext.WizardSkill12(attackDamage);
                }

                // 아이템 장착 데미지 증가
                curAttackDamage += attackDamage * GameManager.instance.skillContext.GetItemValues(4)[0]/100;
                curAttackDamage += attackDamage * GameManager.instance.skillContext.GetItemValues(5)[0]/100;
                curAttackDamage += attackDamage * GameManager.instance.skillContext.GetItemValues(6)[0]/100;
                curAttackDamage += attackDamage * GameManager.instance.skillContext.GetItemValues(7)[0]/100;
                curAttackDamage += attackDamage * GameManager.instance.skillContext.GetItemValues(11)[2]/100;

                // 아이템 습득 후 버프
                if(strongPotion){
                    curAttackDamage += attackDamage * 0.5f;
                }
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
    private float curMaxHP;
    public float CurMaxHP
    {
        get
        {
            curMaxHP = maxHP;

            if(GetComponent<Player>()){
                // 아이템 장착 후 체력 증가
                // 구슬
                curMaxHP += GameManager.instance.skillContext.GetItemValues(35)[2];
            }

            return curMaxHP;
        }
        set
        {
            curMaxHP = value;
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
    private float resistance = 1;
    private float curResistance;
    public float CurResistance
    {
        get
        {
            curResistance = resistance;

            if(slowDeBuff && !stunDeBuff){
                if(GetComponent<Enemy>()){
                    if(!GetComponent<Enemy>().isBoss && GameManager.instance.skillContext.WizardSkill14()!=0){
                        // 현자 14스킬 슬로우 추가
                        float addSlow = GameManager.instance.skillContext.WizardSkill14();
                        curResistance -= resistance * (slowDeBuffCount * (0.1f + addSlow));
                    }
                } else {
                    curResistance -= resistance * (slowDeBuffCount * 0.1f);
                }
            }
            if(stunDeBuff){
                if(GetComponent<Enemy>()){
                    if(!GetComponent<Enemy>().isBoss){
                        curResistance = 0;
                    }
                } else {
                    curResistance = 0;
                }
            }

            return curResistance;
        }
        set
        {
            curResistance = value;
        }
    }

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
        priestSkill3Count = 5;

        shieldCurTime = shieldTime;
        while (shieldCurTime > 0)
        {
            shieldCurTime -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        shieldCurTime = 0;
        curShield = 0;
        priestSkill3Count = 0;

        shielSfxPlayer.GetComponent<LoopSFXPlayer>().Stop();
        //이펙트 제거해야 함
        isShield = false;
    }

    public IEnumerator ArcherSkill9Buff()
    {
        archerSkill9 = true;
        yield return new WaitForSeconds(10f);
        archerSkill9 = false;
    }
    float archerSkill9Count = 0;
    float priestSkill3Count = 0;

    public float priestSkill10Time;
    void Update()
    {
        if(GetComponent<Player>() && !GetComponent<Player>().playerDead){

            // 궁수 9스킬 30초마다 10초간 버프
            if(GameManager.instance.skillContext.ArcherSkill9()!=0){
                archerSkill9Count -= Time.deltaTime;
                if(archerSkill9Count<=0){
                    archerSkill9Count = 40;
                    StartCoroutine(ArcherSkill9Buff());
                }
            }

            // 사제 3스킬 보호막이 활성화되어 있을경우 5초마다 잃은체력의 5%를 회복
            if(isShield && GameManager.instance.skillContext.PriestSkill3()!=0 && GetComponent<Player>()){
                float val = GameManager.instance.skillContext.PriestSkill3();
                priestSkill3Count -= Time.deltaTime;
                if(priestSkill3Count<=0){
                    priestSkill3Count = 5;
                    if(CurHP<CurMaxHP){
                        GetComponent<Player>().GetDamage((CurMaxHP-CurHP)*val, false);
                        if(GetComponent<Player>().CurHP>GetComponent<Player>().CurMaxHP){
                            GetComponent<Player>().CurHP = GetComponent<Player>().CurMaxHP;
                        }
                    } else {
                        GetComponent<Player>().GetDamage(0, false);
                    }
                }
            }
            
            // 일정시간마다 쉴드 적용
            if(GameManager.instance.skillContext.PriestSkill10()!=0){
                priestSkill10Time += Time.deltaTime;
                if(priestSkill10Time>=GameManager.instance.skillContext.PriestSkill10()){
                    priestSkill10Time = 0;
                    MaceBullet maceBullet = null;
                    foreach(Player pl in GameManager.instance.players){
                        if(pl.character.Equals("사제")){
                            maceBullet = pl.GetComponentInChildren<MaceBullet>();
                            break;
                        }
                    }
                    if(maceBullet != null){
                        if(isShield && CurShield <= 0){
                            MaxShield = maceBullet.CurShieldAmount;
                            CurShield = MaxShield;
                            shieldTime = maceBullet.shieldTime;
                            StartCoroutine(ShieldOn());
                        } else {
                            if(MaxShield < (maceBullet.CurShieldAmount + CurShield)){
                                CurShield = MaxShield;
                            } else {
                                CurShield += maceBullet.CurShieldAmount;
                            }
                            shieldTime = maceBullet.shieldTime;
                        }
                    }
                }
            }
        }
    }
    // 궁수 0스킬 피격시 5초동안 이속 증가
    public float archerSkill0BuffTime = 5f;
    public IEnumerator ArcherSkill0Buff()
    {
        archerSkill0 = true;
        archerSkill0BuffTime = 5;
        while(archerSkill0BuffTime>0){
            archerSkill0BuffTime -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        archerSkill0 = false;
    }

    public int slowDeBuffCount = 0;
    public float slowDeBuffTime = 0;
    public bool slowDeBuff = false;
    public IEnumerator SlowDeBuff(){
        slowDeBuff = true;
        while(slowDeBuffTime>0){
            slowDeBuffTime -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        slowDeBuffTime = 0;
        slowDeBuffCount = 0;
        slowDeBuff = false;
    }

    public float stunDeBuffTime = 0;
    public bool stunDeBuff = false;
    public IEnumerator StunDeBuff(){
        stunDeBuff = true;
        resistance = 0;

        while(stunDeBuffTime>0){
            stunDeBuffTime -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        stunDeBuffTime = 0;
        stunDeBuff = false;
        resistance = 1;
    }

    public bool priestSkill7;
    public float priestSkill7Time;
    public IEnumerator PriestSkill7Buff(float time){
        priestSkill7 = true;
        priestSkill7Time = time;
        while(priestSkill7Time>0){
            priestSkill7Time -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        priestSkill7Time = 0;
        priestSkill7 = false;
    }
    [SerializeField]
    private float vamp;
    [SerializeField]
    private float curVamp;
    public float Vamp
    {
        get
        {
            return vamp;
        }
        set
        {
            vamp = value;
        }
    }
    public float CurVamp
    {
        get
        {
            curVamp = vamp;

            if(GetComponent<Player>()){
                // 아이템 장착 흡혈 증가
                curVamp += GameManager.instance.skillContext.GetItemValues(7)[2]/100;
            }

            return curVamp;
        }
        set
        {
            curVamp = value;
        }
    }

    private float hpRezen;
    private float curHpRezen;
    public float HpRezen
    {
        get
        {
            return hpRezen;
        }
        set
        {
            hpRezen = value;
        }
    }
    public float CurHpRezen
    {
        get
        {
            curHpRezen = hpRezen;

            if(GetComponent<Player>()){
                // 아이템 장착 체력 회복량 증가
                // 구슬
                curHpRezen += GameManager.instance.skillContext.GetItemValues(32)[1];
                curHpRezen += GameManager.instance.skillContext.GetItemValues(33)[1];
                curHpRezen += GameManager.instance.skillContext.GetItemValues(34)[1];
                curHpRezen += GameManager.instance.skillContext.GetItemValues(35)[1];

                //갑옷
                curHpRezen += GameManager.instance.skillContext.GetItemValues(37)[1];
                curHpRezen += GameManager.instance.skillContext.GetItemValues(38)[1];
                curHpRezen += GameManager.instance.skillContext.GetItemValues(39)[1];
            }

            return curHpRezen;
        }
        set
        {
            curHpRezen = value;
        }
    }
    private float DamageReduction = 0;
    private float curDamageReduction;
    public float CurDamageReduction
    {
        get
        {
            curDamageReduction = DamageReduction;

            if(GetComponent<Player>()){
                // 아이템 장착 데미지 감소
                // 갑옷
                curDamageReduction += GameManager.instance.skillContext.GetItemValues(39)[2]/100;
            }

            return curDamageReduction;
        }
        set
        {
            curDamageReduction = value;
        }
    }

    // 포션 아이템들 구현
    // 힘의 비약
    public float strongPotionTime;
    public bool strongPotion;
    public IEnumerator StrongPotion(){
        strongPotion = true;
        strongPotionTime = 20;
        while(strongPotionTime>0){
            strongPotionTime -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        strongPotionTime = 0;
        strongPotion = false;
    }
    // 민첩의 비약
    public float speedPotionTime;
    public bool speedPotion;
    public IEnumerator SpeedPotion(){
        speedPotion = true;
        speedPotionTime = 20;
        while(speedPotionTime>0){
            speedPotionTime -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        speedPotionTime = 0;
        speedPotion = false;
    }
    // 마나의 비약
    public float agilityPotionTime;
    public bool agilityPotion;
    public IEnumerator AgilityPotion(){
        agilityPotion = true;
        agilityPotionTime = 20;
        while(agilityPotionTime>0){
            agilityPotionTime -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        agilityPotionTime = 0;
        agilityPotion = false;
    }
}
