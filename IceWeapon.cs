using UnityEngine;

public class IceWeapon : Weapon
{
    public GameObject iceGroundPrefab;
    public float groundDamage;
    public override void InitWeapon(){
        delay = 5;
    }
    public override void UpdateWeapon(){
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
        bullet.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(GetComponentInParent<Player>(),damage,out bool isCritical), count, dir, knockBackPower, isCritical);
        bullet.GetComponent<IceBullet>().projectilePrefab = iceGroundPrefab;
        bullet.GetComponent<IceBullet>().groundDamage = groundDamage;
    }

}