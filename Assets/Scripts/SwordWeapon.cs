using UnityEngine;

public class SwordWeapon : MeleeWeapon
{
    private void Awake() {
        curDetectionAngle = detectionAngle;
    }
    public override void InitWeapon()
    {

    }
    public override void Fire(Transform _targetTransform)
    {
        Vector3 targetPos = _targetTransform.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        Transform bullet = poolBullet.Get().transform;

        bullet.parent = GameManager.instance.pool.transform;
        bullet.position = transform.position + dir * spawnDistance;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(GetComponentInParent<Player>(), damage, out bool isCritical), count, Vector3.zero, knockBackPower, duration, isCritical);
        bullet.GetComponent<EffectBullet>().detectionAngle = curDetectionAngle;

        if(warriorFire){
            bullet.GetComponent<FireSword>().warriorFire = true;
            bullet.GetComponent<FireSword>().warriorFireDamge = warriorFireDamge;
            bullet.GetComponent<FireSword>().warriorFireTime = warriorFireTime;
        }
    }
}
