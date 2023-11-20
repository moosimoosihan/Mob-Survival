using UnityEngine;

public class HPzenPassive : Weapon
{
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
        Player me = gameObject.GetComponentInParent<Player>();

        if (gameObject.activeSelf && me.CurHpRezen > 0 && !me.playerDead)
        {
            me.GetDamage(-me.CurHpRezen, false, null, true);

            // 체력 회복 이펙트 생성
            Transform healEffect = poolBullet.Get().transform;
            healEffect.parent = GameManager.instance.pool.transform;
            Bullet healScript = healEffect.GetComponent<Bullet>();
            healScript.Fire(0, -1, Vector3.zero, 0, 0.5f, false, true, false);
            healEffect.position = me.transform.position;
        }
    }
}
