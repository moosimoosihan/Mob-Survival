using System.Collections;
using UnityEngine;

public class ArcherActiveSkill : ActiveSkill
{
    public float skillDuration;
    bool isActivate;
    public float buffTime;
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
        Debug.Log("궁수 액티브 스킬 시전");
        StartCoroutine(SkillDelay());
        if(!isActivate){
            StartCoroutine(Skill());
        } else {
            buffTime = skillDuration;
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
        BuffEffect effect = poolBuffEffect.Get();
        effect.transform.parent = GameManager.instance.pool.transform;
        effect.target = transform.parent;
        effect.transform.position = transform.position;

        player.critRate += 0.3f;

        GameObject buffSfxPlayer = AudioManager.Instance.LoopSfxPlay(AudioManager.LoopSfx.Archer_Buff1);

        buffTime = skillDuration;
        while(buffTime>0){
            buffTime -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        buffTime = 0;
        player.critRate -= 0.3f;

        buffSfxPlayer.GetComponent<LoopSFXPlayer>().Stop();

        effect.DestroyBuffEffect();
        isActivate = false;
    }
}
