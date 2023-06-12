using System.Collections;
using UnityEngine;

public class ArcherPassive : Weapon
{
    public BowWeapon bowWeapon;
    public bool isActivate;
    public float buffTime;
    public override void InitWeapon()
    {
        bowWeapon = player.GetComponentInChildren<BowWeapon>();
    }

    public override void UpdateWeapon()
    {
        if(bowWeapon.isCritical){
            bowWeapon.isCritical = false;
            Activate();
        }
    }
    void Activate(){
        if(!isActivate){
            StartCoroutine(BuffDelay());
        } else if(isActivate) {
            buffTime = curDelay;
        }
    }
    IEnumerator BuffDelay(){
        isActivate = true;
        GameObject effect = GameManager.instance.pool.Get(projectilePrefab);
        effect.transform.SetParent(transform);
        effect.transform.position = transform.position;

        bowWeapon.curDelay -=  bowWeapon.delay * damage;
        buffTime = curDelay;
        while(buffTime>0){
            buffTime -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        buffTime = 0;
        bowWeapon.curDelay +=  bowWeapon.delay * damage;
        effect.gameObject.SetActive(false);
        isActivate = false;
    }
}
