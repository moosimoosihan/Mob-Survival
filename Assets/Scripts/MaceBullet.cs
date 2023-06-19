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
                player.StartCoroutine(player.ShieldOn());
            } else {
                if(player.maxShield<(shiledAmount + player.curShield)){
                    player.curShield = player.maxShield;
                } else {
                    player.curShield += shiledAmount;
                }
                player.shieldTime = shiledTime;
            }

            Player minHealthPlayer = GameManager.instance.players[0];

            // 가장 체력이 낮은 아군을 탐지
            for (int y = 0; y < GameManager.instance.players.Length; y++)
            {
                if (GameManager.instance.players[y].curHP > 0 && GameManager.instance.players[i].curHP/GameManager.instance.players[y].maxHP < minHealthPlayer.curHP/minHealthPlayer.maxHP)
                {
                    minHealthPlayer = GameManager.instance.players[y];
                }
            }

            // 체력이 낮은 아군에게 보호막
            if(!minHealthPlayer.isShield && minHealthPlayer.curShield <= 0){
                minHealthPlayer.maxShield = shiledAmount;
                minHealthPlayer.curShield = minHealthPlayer.maxShield;
                minHealthPlayer.shieldTime = shiledTime;
                minHealthPlayer.StartCoroutine(minHealthPlayer.ShieldOn());
            } else {
                if(minHealthPlayer.maxShield<(shiledAmount + minHealthPlayer.curShield)){
                    minHealthPlayer.curShield = minHealthPlayer.maxShield;
                } else {
                    minHealthPlayer.curShield += shiledAmount;
                }
                minHealthPlayer.shieldTime = shiledTime;
            }
        }

        if(_deActivate)
        {
            DeActivate(_duration);
        }
    }
}