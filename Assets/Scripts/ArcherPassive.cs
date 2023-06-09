using System.Collections;
using UnityEngine;

public class ArcherPassive : Weapon
{
    BowWeapon bowWeapon;
    public bool isActivate;
    public float buffTime;
    public override void InitWeapon()
    {
        curDelay = delay;
        bowWeapon = player.GetComponentInChildren<BowWeapon>();
    }

    public override void UpdateWeapon()
    {
        if(bowWeapon.isCritical){
            bowWeapon.isCritical = false;
            Activate();
        }
    }
    public void Activate(){
        if(!isActivate){
            StopCoroutine(BuffDelay());
        } else if(isActivate) {
            buffTime = curDelay;
        }
    }
    IEnumerator BuffDelay(){
        isActivate = true;
        bowWeapon.curDelay -=  bowWeapon.delay * damage;
        buffTime = curDelay;
        while(buffTime>0){
            buffTime -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        bowWeapon.curDelay +=  bowWeapon.delay * damage;
        isActivate = false;
    }
}
