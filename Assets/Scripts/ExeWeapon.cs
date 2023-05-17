using UnityEngine;

public class ExeWeapon : Weapon
{
    [SerializeField]
    float distX;
    [SerializeField]
    float distY;

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

        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        Vector3 ranVec = new Vector3(Random.Range(-distX, distX), distY, 0);
        bullet.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(GetComponentInParent<Player>(),damage, out bool isCritical), count, ranVec, knockBackPower, isCritical);
    }

}