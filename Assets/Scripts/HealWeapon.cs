using UnityEngine;

public class HealWeapon : Weapon
{
    Player minHealthPlayer;
    public override void InitWeapon()
    {
        minHealthPlayer = player;
    }
    public override void UpdateWeapon()
    {
        timer += Time.deltaTime;
        if(timer > curDelay){
            Fire();
            timer = 0f;
        }
    }
    void Fire(){
        Player[] playersList =  GameManager.instance.players;
        float minHeath = 1.1f;

        // 가장 체력이 낮은 아군을 탐지
        for (int i = 0; i < GameManager.instance.players.Length; i++)
        {
            if (!playersList[i].playerDead && playersList[i].curHP/playersList[i].maxHP<minHeath)
            {
                minHealthPlayer = GameManager.instance.players[i];
                minHeath = playersList[i].curHP/playersList[i].maxHP;
            }
        }

        if (minHealthPlayer.gameObject.activeSelf)
        {
            minHealthPlayer.GetDamage(-damage, false);

            // 체력이 넘칠 경우 최대 체력으로 설정
            if (minHealthPlayer.curHP > minHealthPlayer.maxHP)
            {
                minHealthPlayer.curHP = minHealthPlayer.maxHP;
            }

            // 체력 회복 이펙트 생성
            Transform healEffect = poolBullet.Get().transform;
            Bullet healScript = healEffect.GetComponent<Bullet>();
            healScript.Fire(0, -1, Vector3.zero, 0, duration, false, true, false);
            healEffect.position = minHealthPlayer.transform.position;
        }
    }
}
