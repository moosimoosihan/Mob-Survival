using UnityEngine;

public class MaceWeapon : MeleeWeapon
{
    public float shieldAmount;
    public float shieldTime;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponentInParent<Player>();
        CurDetectionAngle = DetectionAngle;
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
        bullet.GetComponent<MaceBullet>().ShieldAmount = shieldAmount;
        bullet.GetComponent<MaceBullet>().shieldTime = shieldTime;
        bullet.GetComponent<EffectBullet>().DetectionAngle = CurDetectionAngle;
        bullet.GetComponent<EffectBullet>().AttackRadius = CurDetectRadius;
        bullet.GetComponent<EffectBullet>().Fire(DamageManager.Instance.Critical(GetComponentInParent<Player>(), Damage, out bool isCritical), CurCount, Vector3.zero, knockBackPower, duration, isCritical);
        if(isCritical || this.isCritical) {this.isCritical=true;} else {this.isCritical=false;}
    }
}
