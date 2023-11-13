using UnityEngine;
using UnityEngine.Pool;
public class IceBullet : Bullet
{
    public GameObject projectilePrefab;
    public float groundDamage;
    private float groundDuration;
    public float GroundDuration
    {
        get
        {
            return groundDuration;
        }
        set
        {
            groundDuration = value;
        }
    }
    private float curGroundDuration;
    public float CurGroundDuration
    {
        get
        {
            curGroundDuration = groundDuration;
            // 현자 5스킬 얼음 장판 지속시간 증가
            curGroundDuration += GameManager.instance.skillContext.WizardSkill5();

            // 현자 10스킬 얼음 장판 지속시간 증가
            curGroundDuration += GameManager.instance.skillContext.WizardSkill10()[1];
            
            return curGroundDuration;
        }
        set
        {

        }
    }

    public IObjectPool<Bullet> bulletPool;

    protected override void Awake()
    {
        base.Awake();
        bulletPool = new ObjectPool<Bullet>(CreateBullet, OnGetBullet, OnReleaseBullet, OnDestroyBullet);
    }
    public override void OnCreateBullet()
    {
        // 얼음 장판 소환
        Transform bullet = bulletPool.Get().transform;

        Vector2 size = new Vector2(1,1);

        // 현자 4스킬 아이스그라운드 사이즈 증가
        if(GameManager.instance.skillContext.WizardSkill4()!=0){
            float val = GameManager.instance.skillContext.WizardSkill4();
            size = new Vector2(val, val);
        }
        // 현자 10스킬 아이스필드의 사이즈를 증가
        if(GameManager.instance.skillContext.WizardSkill10()[0]!=0){
            float val = GameManager.instance.skillContext.WizardSkill10()[0];
            size = new Vector2(val, val);
        }

        // 현자 12스킬 아이스필드의 사이즈를 증가
        if(GameManager.instance.skillContext.WizardSkill12(1)!=0){
            float val = GameManager.instance.skillContext.WizardSkill12(1);
            size = new Vector3(size.x+val, size.y+val);
        }

        // 현자 13스킬 아이스필드의 사이즈를 증가
        if(GameManager.instance.skillContext.WizardSkill13()!=0){
            float val = GameManager.instance.skillContext.WizardSkill13();
            size = new Vector3(size.x*val, size.y*val);
        }

        bullet.transform.localScale = size;
        bullet.parent = GameManager.instance.pool.transform;
        bullet.position = transform.position;
        bullet.GetComponent<Bullet>().Init(groundDamage, -1, 0, CurGroundDuration, false, true);
    }
    Bullet CreateBullet()
    {
        Bullet bullet = Instantiate(projectilePrefab).GetComponent<Bullet>();
        bullet.SetManagedPool(bulletPool);
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
