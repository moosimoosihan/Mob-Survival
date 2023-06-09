using System.Collections;
using UnityEngine;

public class ArcherPassive : Weapon
{
    BowWeapon bowWeapon;
    public bool isActivate;
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
        }
    }
    IEnumerator BuffDelay(){
        isActivate = true;
        bowWeapon.curDelay -=  bowWeapon.delay * damage;
        yield return new WaitForSeconds(curDelay);
        bowWeapon.curDelay +=  bowWeapon.delay * damage;
        isActivate = false;
    }
}
