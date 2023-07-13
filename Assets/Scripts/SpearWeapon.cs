using UnityEngine;

public class SpearWeapon : Weapon
{
    public override void InitWeapon()
    {
        
    }

    public override void UpdateWeapon()
    {
        timer += Time.deltaTime;
        if (timer > curDelay)
        {
            timer = 0f;
            Fire();
        }
    }

    void Fire()
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
        bullet.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(GetComponentInParent<Player>(),damage,out bool isCritical), count, dir, knockBackPower, duration, isCritical);
    }
}
