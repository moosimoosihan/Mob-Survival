using UnityEngine;

public class IceWeapon : Weapon
{
    public GameObject iceGroundPrefab;
    public float groundDamage;
    public float groundDuration;
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
        bullet.rotation = Quaternion.FromToRotation(Vector3.left, dir);
        bullet.GetComponent<IceBullet>().projectilePrefab = iceGroundPrefab;
        bullet.GetComponent<IceBullet>().groundDamage = groundDamage;
        bullet.GetComponent<IceBullet>().groundDuration = groundDuration;
        bullet.GetComponent<IceBullet>().speed = bulletSpeed;
        bullet.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(GetComponentInParent<Player>(),damage,out bool isCritical), count, dir, knockBackPower, duration, isCritical);
        
        AudioManager.Instance.SfxPlay(AudioManager.Sfx.Wizard_Attack);
    }

}