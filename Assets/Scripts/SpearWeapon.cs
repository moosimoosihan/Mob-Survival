using UnityEngine;

public class SpearWeapon : Weapon
{
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
        bullet.position = transform.position + dir;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().player = player;
        bullet.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(GetComponentInParent<Player>(),Damage,out bool isCritical), CurCount, dir, knockBackPower, duration, isCritical);
    }
}
