using System.Collections;
using UnityEngine;

public class HealerActiveSkill : ActiveSkill
{
    public override void ActiveSkillInit(){
        skillArea = null;
        player = GetComponentInParent<Player>();
    }
    public override void AreaOff(){}
    public override void AreaUpdate(){}
    public override void ActiveSkillUpdate(){    
        Debug.Log("사제 액티브 스킬 시전");
        StartCoroutine(SkillDelay());
        Skill();
    }
    IEnumerator SkillDelay(){
        timer = 0;
        isActive = true;
        yield return new WaitForSeconds(delay);
        isActive = false;
    }
    void Skill(){
        Player minHealthPlayer = GameManager.instance.players[0];
        for(int i=0;i<GameManager.instance.players.Length;i++){
            if(GameManager.instance.players[i].playerDead){
                // 죽은 캐릭터일 경우 전부 회복 및 부활 및 스킬 쿨타임 초기화
                GameManager.instance.players[i].curHP = GameManager.instance.players[i].maxHP;
                GameManager.instance.players[i].playerDead = false;
                GameManager.instance.players[i].collider2D.enabled = true;
                GameManager.instance.players[i].CreateFollowingHpBar();
                return;
            }
            if(GameManager.instance.players[i].curHP/GameManager.instance.players[i].maxHP<minHealthPlayer.curHP/minHealthPlayer.maxHP){
                minHealthPlayer = GameManager.instance.players[i];
            }
        }
        // 체력이 제일 적은 캐릭터 전부 회복 및 스킬 쿨타임 초기화
        minHealthPlayer.curHP = minHealthPlayer.maxHP;

        if(minHealthPlayer.GetComponentInChildren<ActiveSkill>().isActive && minHealthPlayer != player){
            minHealthPlayer.GetComponentInChildren<ActiveSkill>().StopAllCoroutines();
            minHealthPlayer.GetComponentInChildren<ActiveSkill>().isActive = false;
            minHealthPlayer.GetComponentInChildren<ActiveSkill>().timer = minHealthPlayer.GetComponentInChildren<ActiveSkill>().delay;
        }
    }
}
