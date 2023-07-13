using UnityEngine;
using UnityEngine.Pool;

public abstract class Weapon : MonoBehaviour
{    
    [Header("무기 정보")]
    public float damage;
    public int count;
    public float delay;
    public float curDelay;
    public float bulletSpeed;
    public float knockBackPower;
    public float duration;
    public bool isCritical;
    
    protected float timer;
    protected Player player;

    public GameObject projectilePrefab;
    public IObjectPool<BuffEffect> poolBuffEffect;
    public IObjectPool<Bullet> poolBullet;

    void Awake()
    {
        player = GetComponentInParent<Player>();
        poolBuffEffect = new ObjectPool<BuffEffect>(CreateEffect, OnGetEffect, OnReleaseEffect, OnDestroyEffect);
        poolBullet = new ObjectPool<Bullet>(CreateBullet, OnGetBullet, OnReleaseBullet, OnDestroyBullet);
    }
    
    void Start()
    {
        curDelay = delay;
        InitWeapon();
    }

    void Update()
    {
        UpdateWeapon();
    }
    public void LevelUp()
    {

    }

    public abstract void InitWeapon();

    public abstract void UpdateWeapon();

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
