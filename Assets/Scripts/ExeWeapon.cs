using UnityEngine;

public class ExeWeapon : Weapon
{
    [SerializeField]
    float distX;
    [SerializeField]
    float distY;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Fire()
    {
        if (!player.scanner.nearestTarget)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        Transform bullet = poolBullet.Get().transform;

        bullet.parent = GameManager.instance.pool.transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        Vector3 ranVec = new Vector3(Random.Range(-distX, distX), distY, 0);
        bullet.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(GetComponentInParent<Player>(),Damage, out bool isCritical), count, ranVec, knockBackPower, duration, isCritical);
    }

}
