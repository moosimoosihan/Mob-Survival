using UnityEngine;
using UnityEngine.Pool;

public abstract class ActiveSkill : MonoBehaviour
{
    private float damage;
    public float Damage
    {
        get
        {
            if(player.CurActiveSkillDamage != 0){
                damage = player.CurActiveSkillDamage;
            }
            return damage;
        }
        set
        {
            damage = value;
        }
    }
    public bool isActive;
    public bool areaOn;
    public GameObject skillArea;
    public float delay;
    private float curDelay;
    public float CurDelay
    {
        get
        {
            curDelay = delay;
            if(player!=null && player.CurActiveDelay != 0){
                curDelay /= player.CurActiveDelay;
            }
            return curDelay;
        }
        set
        {
            curDelay = value;
        }
    }
    public Player player;
    public float timer;
    [SerializeField]
    protected GameObject projectilePrefab;

    public IObjectPool<BuffEffect> poolBuffEffect;
    public IObjectPool<Bullet> poolBullet;

    protected virtual void Start()
    {
        
        timer = CurDelay;
        
        ActiveSkillInit();

        GameManager.instance.inputManager.GetAction("ReadyActiveSkill").Enable();
        GameManager.instance.inputManager.GetAction("ConfirmActiveSkill").Enable();
        GameManager.instance.inputManager.GetAction("CancelActiveSkill").Enable();
    }
    protected virtual void Awake()
    {
        poolBuffEffect = new ObjectPool<BuffEffect>(CreateEffect, OnGetEffect, OnReleaseEffect, OnDestroyEffect);
        poolBullet = new ObjectPool<Bullet>(CreateBullet, OnGetBullet, OnReleaseBullet, OnDestroyBullet);
    }
    protected virtual void OnDestroy()
    {
        GameManager.instance.inputManager.GetAction("ReadyActiveSkill").Disable();
        GameManager.instance.inputManager.GetAction("ConfirmActiveSkill").Disable();
        GameManager.instance.inputManager.GetAction("CancelActiveSkill").Disable();
    }
    protected virtual void Update()
    {
        if(isActive && timer < CurDelay){
            timer += Time.deltaTime;
        }
        if(!player.inputEnabled || !GameManager.instance.isPlay){
            AreaOff();
            return;
        }
        if(KeyDown(areaOn)){
            ActiveSkillUpdate();
        }
        AreaUpdate();
    }
    public abstract void ActiveSkillUpdate();
    public abstract void ActiveSkillInit();
    public abstract void AreaUpdate();
    public abstract void AreaOff();
    public bool KeyDown(bool _areaOn){
        if(!isActive && _areaOn? GameManager.instance.inputManager.GetAction("ConfirmActiveSkill").IsPressed() : GameManager.instance.inputManager.GetAction("ReadyActiveSkill").IsPressed())
        {
            return true;
        } else {
            return false;
        }
    }
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
