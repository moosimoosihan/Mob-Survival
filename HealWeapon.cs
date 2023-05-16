using UnityEngine;

public class HealWeapon : Weapon
{
    public override void InitWeapon()
    {
        delay = 5;
    }
    public override void UpdateWeapon()
    {
        timer += Time.deltaTime;
        if(timer > delay){
            Fire();
            timer = 0f;
        }
    }
    void Fire(){
        int playerNum = 0;
        float minHealth = GameManager.instance.players[playerNum].maxHP;

        // 가장 체력이 적은 아군을 탐지
        for (int i = 0; i < GameManager.instance.players.Length; i++)
        {
            if (GameManager.instance.players[i].curHP > 0 && GameManager.instance.players[i].curHP < minHealth)
            {
                minHealth = GameManager.instance.players[i].curHP;
                playerNum = i;
            }
        }

        if (GameManager.instance.players[playerNum].gameObject.activeSelf)
        {
            GameManager.instance.players[playerNum].GetDamage(-damage, false);

            // 체력이 넘칠 경우 최대 체력으로 설정
            if (GameManager.instance.players[playerNum].curHP > GameManager.instance.players[playerNum].maxHP)
            {
                GameManager.instance.players[playerNum].curHP = GameManager.instance.players[playerNum].maxHP;
            }

            // 체력 회복 이펙트 생성
            Transform healEffect = GameManager.instance.pool.Get(projectilePrefab).transform;
            Bullet healScript = healEffect.GetComponent<Bullet>();
            healScript.Fire(0, -1, Vector3.zero, 0,false, true, false);
            healEffect.position = GameManager.instance.players[playerNum].transform.position;
            //healEffect.SetParent(GameManager.instance.players[playerNum].transform);
            //healEffect.localPosition = Vector3.zero;
            //healEffect.rotation = Quaternion.identity;
        }
    }
}
