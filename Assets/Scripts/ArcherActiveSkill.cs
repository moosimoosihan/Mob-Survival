using System.Collections;
using UnityEngine;

public class ArcherActiveSkill : ActiveSkill
{
    public float skillDuration;
    bool isActivate;
    public override void ActiveSkillInit(){
        skillArea = null;
        player = GetComponentInParent<Player>();
    }
    public override void AreaOff(){}
    public override void AreaUpdate(){}
    public override void ActiveSkillUpdate(){
        Debug.Log("궁수 액티브 스킬 시전");
        StartCoroutine(SkillDelay());
        if(!isActivate){
            StartCoroutine(Skill());
        }
    }
    IEnumerator SkillDelay(){
        timer = 0;
        isActive = true;
        yield return new WaitForSeconds(delay);
        isActive = false;
    }
    IEnumerator Skill(){
        isActivate = true;
        player.critRate += 0.3f;
        yield return new WaitForSeconds(skillDuration);
        player.critRate -= 0.3f;
        isActivate = false;
    }
}
