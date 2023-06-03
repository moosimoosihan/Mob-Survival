using System.Collections;
using UnityEngine;

public class ArcherActiveSkill : ActiveSkill
{
    public float skillDuration;
    public override void ActiveSkillInit(){
        skillArea = null;
        player = GetComponentInParent<Player>();
    }
    public override void AreaOff(){}
    public override void AreaUpdate(){}
    public override void ActiveSkillUpdate(){
        Debug.Log("궁수 액티브 스킬 시전");
        StartCoroutine(SkillDelay());
        StartCoroutine(Skill());
    }
    IEnumerator SkillDelay(){
        timer = 0;
        isActive = true;
        yield return new WaitForSeconds(delay);
        isActive = false;
    }
    IEnumerator Skill(){
        player.critRate += 0.3f;
        yield return new WaitForSeconds(skillDuration);
        player.critRate -= 0.3f;
    }
}
