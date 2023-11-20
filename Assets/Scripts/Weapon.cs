using UnityEngine;
using UnityEngine.Pool;

public abstract class Weapon : MonoBehaviour
{
    [Header("무기 정보")]
    private float damage;
    
    // 회복 및 쿨타임 적용시 사용할 값
    public float weaponValue;
    [SerializeField]
    private int count;
    [SerializeField]
    private int curCount;
    public int CurCount
    {
        get
        {
            curCount = count;
            if(GetComponent<BowWeapon>() && player.character.Equals("궁수")){
                // 궁수 4스킬 50마리 처치시 관통 1 증가
                curCount += GameManager.instance.skillContext.ArcherSkill4(GameManager.instance.kill);

                // 궁수 10스킬 투사체 관통 1 증가
                if(GameManager.instance.skillContext.ArcherSkill10()){
                    curCount += 1;
                }
            }

            // 아이템 장착시 관통 증가
            // 반지
            curCount += GameManager.instance.skillContext.GetItemValues(18)[0];
            curCount += GameManager.instance.skillContext.GetItemValues(19)[0];
            curCount += GameManager.instance.skillContext.GetItemValues(20)[0];
            curCount += GameManager.instance.skillContext.GetItemValues(21)[0];
            
            return curCount;
        }
        set
        {
            curCount = value;
        }
    }
    [SerializeField]
    private float delay;
    private float curDelay;
    public float Delay
    {
        get
        {
            return delay;
        }
        set
        {
            delay = value;
        }
    }
    public float CurDelay
    {
        get
        {
            curDelay = delay;

            // 현자 8스킬 쿨타임 5초 감소
            if(GetComponent<IceWeapon>()){
                curDelay -= GameManager.instance.skillContext.WizardSkill8()[1];
            }

            // 플레이어의 공속을 가져와 적용
            curDelay /= player.CurAttackSpeed;
            
            return curDelay;
        }
        set
        {
            curDelay = value;
        }
    }
    public float bulletSpeed;
    public float knockBackPower;
    [SerializeField]
    private float duration;
    [SerializeField]
    private float curDuration;
    public float Duration
    {
        get
        {
            return duration;
        }
        set
        {
            duration = value;
        }
    }
    public float CurDuration
    {
        get
        {
            curDuration = duration;

            // 아이템 장착에 의한 지속시간 증가
            if(!GetComponent<MeleeWeapon>()){ // 근접무기가 아닐경우
                // 깃털
                curDuration += duration * GameManager.instance.skillContext.GetItemValues(27)[2]/100;
            }
            
            return curDuration;
        }
        set
        {
            curDuration = value;
        }
    }

    public bool isCritical;

    protected float timer;
    protected Player player;

    public GameObject projectilePrefab;
    public IObjectPool<BuffEffect> poolBuffEffect;
    public IObjectPool<Bullet> poolBullet;
    [SerializeField]
    private int projectilePrefabNum = 1;
    public int ProjectilePrefabNum
    {
        get
        {
            return projectilePrefabNum;
        }
        set
        {
            projectilePrefabNum = value;
        }
    }
    [SerializeField]
    private int curProjectilePrefabNum;
    public int CurProjectilePrefabNum
    {
        get
        {
            curProjectilePrefabNum = projectilePrefabNum;
            
            if(GetComponent<BowWeapon>() && player.GetComponent<Player>().archerActiveSkill){
                // 궁수 11스킬 투사체 2개 추가
                curProjectilePrefabNum += (int)GameManager.instance.skillContext.ArcherSkill11();
            }

            // 아이템 장착시 갯수 증가
            // 반지
            curProjectilePrefabNum += GameManager.instance.skillContext.GetItemValues(18)[1];
            curProjectilePrefabNum += GameManager.instance.skillContext.GetItemValues(19)[1];

            return curProjectilePrefabNum;
        }
        set
        {
            curProjectilePrefabNum = value;
        }
    }

    public float Damage
    {
        get
        {
            if(player.CurAttackDamage != 0){
                damage = player.CurAttackDamage;
            }
            return damage;
        }
        set
        {
            damage = value;
        }
    }

    protected virtual void Awake()
    {
        player = GetComponentInParent<Player>();
        poolBuffEffect = new ObjectPool<BuffEffect>(CreateEffect, OnGetEffect, OnReleaseEffect, OnDestroyEffect);
        poolBullet = new ObjectPool<Bullet>(CreateBullet, OnGetBullet, OnReleaseBullet, OnDestroyBullet);
    }
    protected virtual void Update()
    {
        timer += Time.deltaTime;
        if (timer > CurDelay)
        {
            timer = 0f;
            Fire();
        }
    }
    protected abstract void Fire();
    BuffEffect CreateEffect()
    {
        BuffEffect buffEffect = Instantiate(projectilePrefab).GetComponent<BuffEffect>();
        buffEffect.SetManagedPool(poolBuffEffect);
        return buffEffect;
    }
    void OnGetEffect(BuffEffect buffEffect)
    {
        buffEffect.gameObject.SetActive(true);
    }
    void OnReleaseEffect(BuffEffect buffEffect)
    {
        if (buffEffect.gameObject.activeSelf)
            buffEffect.gameObject.SetActive(false);
    }
    void OnDestroyEffect(BuffEffect buffEffect)
    {
        Destroy(buffEffect.gameObject);
    }
    Bullet CreateBullet()
    {
        Bullet bullet = Instantiate(projectilePrefab).GetComponent<Bullet>();
        bullet.SetManagedPool(poolBullet);
        return bullet;
    }
    void OnGetBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(true);
    }
    void OnReleaseBullet(Bullet bullet)
    {
        if (bullet.gameObject.activeSelf)
            bullet.gameObject.SetActive(false);
    }
    void OnDestroyBullet(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }
}
