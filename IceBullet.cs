using UnityEngine;

public class IceBullet : Bullet
{
    public GameObject projectilePrefab;
    public float groundDamage;

    public virtual void OnTriggerEnter2D(Collider2D collision){
         if (!collision.CompareTag("Enemy") || per == -1)
            return;

        bool tempIsHit = false;
        if (collision != null)
        {
            Enemy detectedEnemy = collision.GetComponent<Enemy>();

            if (hitOnlyOnce)
            {
                //처음 닿은 대상인 경우 데미지 주고 데미지 입힌 리스트에 보관
                if (detectedEnemyList.Contains(detectedEnemy) == false)
                {
                    detectedEnemyList.Add(detectedEnemy);
                    if (detectedEnemy.gameObject.activeSelf)
                        tempIsHit = detectedEnemy.GetDamage(damage, knockBackPower, isCritical);
                }
            }
            else
            {
                tempIsHit = detectedEnemy.GetDamage(damage, knockBackPower, isCritical);
            }
            // 얼음 장판 소환
            Transform bullet = GameManager.instance.pool.Get(projectilePrefab).transform;
            bullet.position = transform.position;
            bullet.GetComponent<Bullet>().Init(groundDamage, -1, 0, false, true);
        }

        //이미 맞아서 죽어야되는애가 뒤에 오는 총알 맞았을때는 총알이 그냥 지나가게하기
        if(tempIsHit)
            per--;

        if (per == -1)
        {
            rigid.velocity = Vector2.zero;
            DeActivate(0);
        }
    }
}
