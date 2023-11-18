using System.Collections.Generic;
using UnityEngine;

public class FireSword : EffectBullet
{
    public bool warriorFire;
    public float warriorFireDamge;
    public float warriorFireTime;

    public bool fireBurn;

    public override List<Enemy> GetEnemies(){
        return base.GetEnemies();
    }

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
            if(!enemyList[i].gameObject.activeSelf)
                return;
            
            enemyList[i].GetDamage(damage, knockBackPower, isCritical, player);
                                    
            if(warriorFire && enemyList[i].gameObject.activeSelf){
                if(!enemyList[i].isFire){
                    enemyList[i].StartCoroutine(enemyList[i].WarriorFireOn(warriorFireDamge, warriorFireTime, fireBurn));
                } else {
                    enemyList[i].FireInit(warriorFireDamge, warriorFireTime);
                }
            }
        }

        if(_deActivate)
        {
            DeActivate(_duration);
        }
    }
}
