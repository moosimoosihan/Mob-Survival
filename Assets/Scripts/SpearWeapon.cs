using UnityEngine;

public class SpearWeapon : Weapon
{
    public override void InitWeapon()
    {
        delay = 1;
    }

    public override void UpdateWeapon()
    {
        timer += Time.deltaTime;
        if (timer > delay)
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
        
        Transform bullet = GameManager.instance.pool.Get(projectilePrefab).transform;

        bullet.position = transform.position + dir;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(GetComponentInParent<Player>(),damage,out bool isCritical), count, dir, knockBackPower, isCritical);
    }
}
