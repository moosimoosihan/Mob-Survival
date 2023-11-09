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
    private int curCount;
    public int CurCount
    {
        get
        {
            curCount = count;
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
    public float duration;
    public bool isCritical;

    protected float timer;
    protected Player player;

    public GameObject projectilePrefab;
    public IObjectPool<BuffEffect> poolBuffEffect;
    public IObjectPool<Bullet> poolBullet;

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
