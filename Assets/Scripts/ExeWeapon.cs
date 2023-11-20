using UnityEngine;

public class ExeWeapon : Weapon
{
    [SerializeField]
    float distX;
    [SerializeField]
    float distY;

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
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        Vector3 ranVec = new Vector3(Random.Range(-distX, distX), distY, 0);
        bullet.GetComponent<Bullet>().player = player;
        bullet.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(GetComponentInParent<Player>(),Damage, out bool isCritical), CurCount, ranVec, knockBackPower, CurDuration, isCritical);
        if(isCritical || this.isCritical) {this.isCritical=true;} else {this.isCritical=false;}

        // 갯수가 늘어나면 여러개로 나눠서 발사하는거 구현해야 함 (아직 사용하지 않아서 구현x)
    }
}
