using UnityEngine;

public class MaceBullet : EffectBullet
{
    public Player player;
    public float shiledAmount;
    public float shiledTime;
    public override void Fire(float _damage, int _per, Vector3 _dir,  float _knockBackPower, float _duration, bool _isCritical, bool _deActivate = true, bool _hitOnlyOnce = true)
    {
        if (enemyList.Count > 0)
            enemyList.Clear();

        enemyList = GetEnemies();
        hitOnlyOnce = _hitOnlyOnce;
        damage = _damage;
        knockBackPower = _knockBackPower;
        isCritical = _isCritical;

        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].GetDamage(damage, knockBackPower, isCritical);
            // 메이스에 맞은 적이 있다면 보호막을 생성
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

        if(_deActivate)
        {
            DeActivate(_duration);
        }
    }
}