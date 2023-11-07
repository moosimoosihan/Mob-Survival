using System.Collections;
using UnityEngine;

public class ArcherPassive : Weapon
{
    public BowWeapon bowWeapon;
    public bool isActivate;
    public float buffTime;

    protected override void Awake()
    {
        base.Awake();
        bowWeapon = player.GetComponentInChildren<BowWeapon>();
    }

    protected override void Update()
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
        BuffEffect effect = poolBuffEffect.Get();
        effect.transform.parent = GameManager.instance.pool.transform;
        effect.target = transform.parent;
        effect.transform.position = transform.position;

        
        bowWeapon.curDelay -=  bowWeapon.delay * value;

        GameObject buffSfxPlayer = AudioManager.Instance.LoopSfxPlay(AudioManager.LoopSfx.Archer_Buff2);
        
        buffTime = curDelay;
        while(buffTime>0){
            buffTime -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        buffTime = 0;
        
        bowWeapon.curDelay +=  bowWeapon.delay * value;

        buffSfxPlayer.GetComponent<LoopSFXPlayer>().Stop();

        effect.DestroyBuffEffect();
        isActivate = false;
    }

    protected override void Fire()
    {
    }
}
