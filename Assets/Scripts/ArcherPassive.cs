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

            // 궁수 5스킬 치명타 발생시 액티브 스킬 쿨타임 1초 감소
            if(GameManager.instance.skillContext.ArcherSkill5()!=0){
                ArcherActiveSkill archerActiveSkill = player.GetComponentInChildren<ArcherActiveSkill>();
                if(archerActiveSkill.timer<archerActiveSkill.CurDelay){
                    archerActiveSkill.timer+=GameManager.instance.skillContext.ArcherSkill5();
                }
            }
            
            Activate();
        }
    }
    void Activate(){
        if(!isActivate){
            StartCoroutine(BuffDelay());
        } else if(isActivate) {
            buffTime = weaponValue;
        }
    }
    IEnumerator BuffDelay(){
        isActivate = true;
        BuffEffect effect = poolBuffEffect.Get();
        effect.transform.parent = GameManager.instance.pool.transform;
        effect.target = transform.parent;
        effect.transform.position = transform.position;

        
        bowWeapon.CurDelay -=  bowWeapon.Delay * weaponValue;

        GameObject buffSfxPlayer = AudioManager.Instance.LoopSfxPlay(AudioManager.LoopSfx.Archer_Buff2);
        
        buffTime = Delay;
        while(buffTime>0){
            buffTime -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        buffTime = 0;
        
        bowWeapon.CurDelay +=  bowWeapon.Delay * weaponValue;

        buffSfxPlayer.GetComponent<LoopSFXPlayer>().Stop();

        effect.DestroyBuffEffect();
        isActivate = false;
    }

    protected override void Fire()
    {
    }
}
