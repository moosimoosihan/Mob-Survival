using UnityEngine;

public class MaceWeapon : MeleeWeapon
{
    public float shiledAmount;
    public float shiledTime;
    private void Awake() {
        player = GetComponentInParent<Player>();
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

        Transform bullet = GameManager.instance.pool.Get(projectilePrefab).transform;

        bullet.position = transform.position + dir * spawnDistance;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(GetComponentInParent<Player>(), damage, out bool isCritical), count, Vector3.zero, knockBackPower, duration, isCritical);
        bullet.GetComponent<EffectBullet>().detectionAngle = curDetectionAngle;

        // 메이스에 맞은 적이 있다면 보호막이 생기지만 임시로 공격시 보호막을 생성해보기
        if(!player.isShield && player.curShield <= 0){
            player.maxShield = shiledAmount;
            player.curShield = player.maxShield;
            player.shieldTime = shiledTime;
            StartCoroutine(player.ShieldOn());
        } else {
            player.curShield = shiledAmount;
            player.shieldTime = shiledTime;
        }
    }
}
