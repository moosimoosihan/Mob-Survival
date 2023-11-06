using UnityEngine;

public class HealWeapon : Weapon
{
    Player minHealthPlayer;
    protected override void Awake()
    {
        base.Awake();
        minHealthPlayer = player;
    }
    protected override void Update()
    {
        base.Update();
    }
    protected override void Fire()
    {
        Player[] playersList = GameManager.instance.players;
        float minHeath = 1.1f;

        // 가장 체력이 낮은 아군을 탐지
        for (int i = 0; i < GameManager.instance.players.Length; i++)
        {
            if (!playersList[i].playerDead && playersList[i].CurHP / playersList[i].MaxHP < minHeath)
            {
                minHealthPlayer = GameManager.instance.players[i];
                minHeath = playersList[i].CurHP / playersList[i].MaxHP;
            }
        }

        if (minHealthPlayer.gameObject.activeSelf)
        {
            minHealthPlayer.GetDamage(-damage, false);

            // 체력이 넘칠 경우 최대 체력으로 설정
            if (minHealthPlayer.CurHP > minHealthPlayer.MaxHP)
            {
                minHealthPlayer.CurHP = minHealthPlayer.MaxHP;
            }

            // 체력 회복 이펙트 생성
            Transform healEffect = poolBullet.Get().transform;
            Bullet healScript = healEffect.GetComponent<Bullet>();
            healScript.Fire(0, -1, Vector3.zero, 0, duration, false, true, false);
            healEffect.position = minHealthPlayer.transform.position;
        }
    }
}
