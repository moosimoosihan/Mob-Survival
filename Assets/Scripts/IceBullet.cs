using UnityEngine;
using UnityEngine.Pool;
public class IceBullet : Bullet
{
    public GameObject projectilePrefab;
    public float groundDamage;
    public float groundDuration;
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
        bullet.parent = GameManager.instance.pool.transform;
        bullet.position = transform.position;
        bullet.GetComponent<Bullet>().Init(groundDamage, -1, 0, groundDuration, false, true);
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
