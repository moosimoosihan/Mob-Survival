using UnityEngine;

public class IceBullet : Bullet
{
    public GameObject projectilePrefab;
    public float groundDamage;
    public float groundDuration;

    public override void CreateBullet()
    {
        // 얼음 장판 소환
        Transform bullet = GameManager.instance.pool.Get(projectilePrefab).transform;
        bullet.position = transform.position;
        bullet.GetComponent<Bullet>().Init(groundDamage, -1, 0, groundDuration, false, true);
    }
}
