using System.Collections;
using UnityEngine;

public class HealerActiveSkill : ActiveSkill
{
    protected override void Awake(){
        base.Awake();
    }
    protected override void Start(){
        base.Start();
    }
    protected override void OnDestroy(){
        base.OnDestroy();
    }
    protected override void Update(){
        base.Update();
    }
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
                minHealthPlayer = GameManager.instance.players[i];
                minHealthPlayer.Revival();
                if(minHealthPlayer.GetComponentInChildren<ActiveSkill>().isActive){
                    minHealthPlayer.GetComponentInChildren<ActiveSkill>().StopAllCoroutines();
                    minHealthPlayer.GetComponentInChildren<ActiveSkill>().isActive = false;
                    minHealthPlayer.GetComponentInChildren<ActiveSkill>().timer = minHealthPlayer.GetComponentInChildren<ActiveSkill>().delay;
                }
                if(Vector3.Distance(GameManager.instance.playerControl.mainCharacter.transform.position, minHealthPlayer.transform.position)>minHealthPlayer.gameObject.GetComponent<CharacterAI>().distWithinMainCharacter){
                    // 멀리서 부활했을 경우 플레이어 이동
                    minHealthPlayer.transform.position = GameManager.instance.playerControl.mainCharacter.transform.position;
                }
                // 회생 이펙트 생성
                Transform revivalEffect = poolBuffEffect.Get().transform;
                Bullet revivalScript = revivalEffect.GetComponent<Bullet>();
                revivalScript.Fire(0, -1, Vector3.zero, 0, 1, false, true, false);
                revivalEffect.position = minHealthPlayer.transform.position;
                return;
            } else if (GameManager.instance.players[i].curHP/GameManager.instance.players[i].maxHP<minHealthPlayer.curHP/minHealthPlayer.maxHP){
                minHealthPlayer = GameManager.instance.players[i];
            }
        }
        // 전부 회복 및 스킬 쿨타임 초기화
        minHealthPlayer.curHP = minHealthPlayer.maxHP;
        minHealthPlayer.playerDead = false;
        minHealthPlayer.collider2D.enabled = true;
        minHealthPlayer.CreateFollowingHpBar();
        if(minHealthPlayer.GetComponentInChildren<ActiveSkill>().isActive && minHealthPlayer != player){
            minHealthPlayer.GetComponentInChildren<ActiveSkill>().StopCoroutine(SkillDelay());
            minHealthPlayer.GetComponentInChildren<ActiveSkill>().isActive = false;
            minHealthPlayer.GetComponentInChildren<ActiveSkill>().timer = minHealthPlayer.GetComponentInChildren<ActiveSkill>().delay;
        }
    }
}
