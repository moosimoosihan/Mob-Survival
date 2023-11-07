using UnityEngine;

public class MaceBullet : EffectBullet
{
    public Player player;
    Player minHealthPlayer;
    public float shiledAmount;
    public float shiledTime;

    public override void Fire(float _damage, int _per, Vector3 _dir, float _knockBackPower, float _duration, bool _isCritical, bool _deActivate = true, bool _hitOnlyOnce = true)
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
            if (!player.isShield && player.CurShield <= 0)
            {
                player.MaxShield = shiledAmount;
                player.CurShield = player.MaxShield;
                player.shieldTime = shiledTime;
                player.StartCoroutine(player.ShieldOn());
            }
            else
            {
                if (player.MaxShield < (shiledAmount + player.CurShield))
                {
                    player.CurShield = player.MaxShield;
                }
                else
                {
                    player.CurShield += shiledAmount;
                }
                player.shieldTime = shiledTime;
            }

            // 죽지않았고 사제가 아니면서 가장 체력이 낮은 아군을 탐지
            Player[] playersList = GameManager.instance.players;
            float minHeath = 1.1f;
            minHealthPlayer = player;

            // 가장 체력이 낮은 아군을 탐지
            for (int y = 0; y < GameManager.instance.players.Length; y++)
            {
                if (!playersList[y].playerDead && playersList[y] != player && playersList[y].CurHP / playersList[y].MaxHP < minHeath)
                {
                    minHealthPlayer = GameManager.instance.players[y];
                    minHeath = playersList[y].CurHP / playersList[y].MaxHP;
                }
            }

            // 체력이 낮은 아군에게 보호막
            if (!minHealthPlayer.isShield && minHealthPlayer.CurShield <= 0)
            {
                minHealthPlayer.MaxShield = shiledAmount;
                minHealthPlayer.CurShield = minHealthPlayer.MaxShield;
                minHealthPlayer.shieldTime = shiledTime;
                minHealthPlayer.StartCoroutine(minHealthPlayer.ShieldOn());
            }
            else
            {
                if (minHealthPlayer.MaxShield < (shiledAmount + minHealthPlayer.CurShield))
                {
                    minHealthPlayer.CurShield = minHealthPlayer.MaxShield;
                }
                else
                {
                    minHealthPlayer.CurShield += shiledAmount;
                }
                minHealthPlayer.shieldTime = shiledTime;
            }
        }

        if (_deActivate)
        {
            DeActivate(_duration);
        }
    }
}