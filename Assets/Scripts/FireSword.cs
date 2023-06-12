using UnityEngine;

public class FireSword : EffectBullet
{
    public bool warriorFire;
    public float warriorFireDamge;
    public float warriorFireTime;
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

            if(warriorFire){
                if(!enemyList[i].isFire){
                    StartCoroutine(enemyList[i].WarriorFireOn(warriorFireDamge, warriorFireTime));
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
