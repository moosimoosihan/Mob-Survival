using UnityEngine;

public class MaceWeapon : MeleeWeapon
{
    public float shiledAmount;
    public float shiledTime;
    protected override void Awake()
    {
        base.Awake();
        player = GetComponentInParent<Player>();
        curDetectionAngle = detectionAngle;
    }
    protected override void Update()
    {
        base.Update();
    }

    protected override void OnFire(Transform _targetTransform)
    {
        Vector3 targetPos = _targetTransform.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        Transform bullet = poolBullet.Get().transform;

        bullet.transform.parent = GameManager.instance.pool.transform;
        bullet.position = transform.position + dir * spawnDistance;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<MaceBullet>().player = player;
        bullet.GetComponent<MaceBullet>().shiledAmount = shiledAmount;
        bullet.GetComponent<MaceBullet>().shiledTime = shiledTime;

        bullet.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(GetComponentInParent<Player>(), damage, out bool isCritical), count, Vector3.zero, knockBackPower, duration, isCritical);
        bullet.GetComponent<EffectBullet>().detectionAngle = curDetectionAngle;

    }
}
