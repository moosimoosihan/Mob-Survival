using UnityEngine;

public class FireStrike : Bullet
{
    public bool warriorFire;
    public float warriorFireDamge;
    public float warriorFireTime;
    public override void OnTriggerEnter2DUpdate(Collider2D collision)
    {
        if(per == -1)
            return;

        if (!collision.CompareTag("Enemy")){
            if(throwBullet)
                return;
            
            if(collision.CompareTag("Wall")){
                DeActivate(0);
            }
            return;
        }

        bool tempIsHit = false;
        if (collision != null)
        {
            Enemy detectedEnemy = collision.GetComponent<Enemy>();

            //피격 이펙트
            if (hitEffectPrefab != null)
            {
                HitEffect bulletHitEffect = hitEffectPool.Get();
                bulletHitEffect.transform.position = transform.position;
            }

            if (hitOnlyOnce)
            {
                //처음 닿은 대상인 경우 데미지 주고 데미지 입힌 리스트에 보관
                if (detectedEnemyList.Contains(detectedEnemy) == false)
                {
                    detectedEnemyList.Add(detectedEnemy);
                    
                    tempIsHit = detectedEnemy.GetDamage(damage, knockBackPower, isCritical);
                    
                    if (detectedEnemy.gameObject.activeSelf){                        
                        if(warriorFire){
                            if(!detectedEnemy.isFire){
                                detectedEnemy.StartCoroutine(detectedEnemy.WarriorFireOn(warriorFireDamge, warriorFireTime, LevelUpSkills.WorriorSkill10()));
                            } else {
                                detectedEnemy.FireInit(warriorFireDamge, warriorFireTime);
                            }
                        }
                    }
                }
            }
            else
            {
                tempIsHit = detectedEnemy.GetDamage(damage, knockBackPower, isCritical);
                if(warriorFire){
                    if(!detectedEnemy.isFire){
                        StartCoroutine(detectedEnemy.WarriorFireOn(warriorFireDamge, warriorFireTime, LevelUpSkills.WorriorSkill10()));
                    } else {
                        detectedEnemy.FireInit(warriorFireDamge, warriorFireTime);
                    }
                }
            }
        }
    }
}
