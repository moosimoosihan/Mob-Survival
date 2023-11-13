using UnityEngine;

public class MaceBullet : EffectBullet
{
    public Player player;
    Player minHealthPlayer;
    private float shiledAmount;
    private float curShiledAmount;
    public float ShiledAmount
    {
        get
        {
            return shiledAmount;
        }
        set
        {
            shiledAmount = value;
        }
    }
    public float CurShiledAmount
    {
        get
        {
            curShiledAmount = shiledAmount;

            // 사제 4스킬 보호막이 5로 줄어듬
            curShiledAmount -= GameManager.instance.skillContext.PriestSkill4();

            return curShiledAmount;
        }
        set
        {
            curShiledAmount = value;
        }
    }
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

        int removeDebuffCount = 1;

        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].GetDamage(damage, knockBackPower, isCritical);
            // 사제 0스킬 타격시 2초간 스턴
            if(GameManager.instance.skillContext.PriestSkill0()[1]!=0){
                if(!enemyList[i].stunDeBuff){
                    enemyList[i].stunDeBuffTime = 2;
                    enemyList[i].StartCoroutine(enemyList[i].StunDeBuff());
                } else {
                    enemyList[i].stunDeBuffTime = 2;
                }
            }

            // 사제 1스킬 타격시 디버프 1개 제거
            if(GameManager.instance.skillContext.PriestSkill1() && removeDebuffCount>0){
                foreach(Player player in GameManager.instance.players){
                    if(player.slowDeBuff){
                        StopCoroutine(player.SlowDeBuff());
                        player.slowDeBuff = false;
                        player.slowDeBuffTime = 0;
                        removeDebuffCount--;
                        break;
                    } else if(player.stunDeBuff){
                        StopCoroutine(player.StunDeBuff());
                        player.stunDeBuff = false;
                        player.stunDeBuffTime = 0;
                        removeDebuffCount--;
                        break;
                    }
                }
            }

            // 메이스에 맞은 적이 있다면 보호막을 생성
            if (!player.isShield && player.CurShield <= 0)
            {
                player.MaxShield = CurShiledAmount;
                player.CurShield = player.MaxShield;
                player.shieldTime = shiledTime;
                player.StartCoroutine(player.ShieldOn());
            }
            else
            {
                if (player.MaxShield < (CurShiledAmount + player.CurShield))
                {
                    player.CurShield = player.MaxShield;
                }
                else
                {
                    player.CurShield += CurShiledAmount;
                }
                player.shieldTime = shiledTime;
            }

            if(GameManager.instance.skillContext.PriestSkill4()!=0){
                // 사제 4스킬 파티 전체에게 보호막
                // 죽지않고 사제가 아닌 나머지에게만 보호막을 적용 만약 보호막이 있다면 남은 보호막을 체크하여 보호막을 더했을경우 최대보호막을 넘어가면 최대보호막으로 설정
                Player[] playerList = GameManager.instance.players;
                foreach(Player pl in playerList){
                    if(!pl.playerDead && pl != player){
                        if(!pl.isShield && pl.CurShield <= 0){
                            pl.MaxShield = CurShiledAmount;
                            pl.CurShield = pl.MaxShield;
                            pl.shieldTime = shiledTime;
                            pl.StartCoroutine(pl.ShieldOn());
                        } else {
                            if(pl.MaxShield < (CurShiledAmount + pl.CurShield)){
                                pl.CurShield = pl.MaxShield;
                            } else {
                                pl.CurShield += CurShiledAmount;
                            }
                            pl.shieldTime = shiledTime;
                        }
                    }
                }
            } else {
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
                    minHealthPlayer.MaxShield = CurShiledAmount;
                    minHealthPlayer.CurShield = minHealthPlayer.MaxShield;
                    minHealthPlayer.shieldTime = shiledTime;
                    minHealthPlayer.StartCoroutine(minHealthPlayer.ShieldOn());
                }
                else
                {
                    if (minHealthPlayer.MaxShield < (CurShiledAmount + minHealthPlayer.CurShield))
                    {
                        minHealthPlayer.CurShield = minHealthPlayer.MaxShield;
                    }
                    else
                    {
                        minHealthPlayer.CurShield += CurShiledAmount;
                    }
                    minHealthPlayer.shieldTime = shiledTime;
                }
            }
        }

        if (_deActivate)
        {
            DeActivate(_duration);
        }
    }
}