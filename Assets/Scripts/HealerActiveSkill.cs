using System.Collections;
using UnityEngine;

public class HealerActiveSkill : ActiveSkill
{
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
    protected override void Update()
    {
        base.Update();
    }
    public override void ActiveSkillInit()
    {
        skillArea = null;
        player = GetComponentInParent<Player>();
    }
    public override void AreaOff() { }
    public override void AreaUpdate() { }
    public override void ActiveSkillUpdate()
    {
        if(timer >= CurDelay){
            Debug.Log("사제 액티브 스킬 시전");
            StartCoroutine(SkillDelay());
            Skill();
        }
    }
    IEnumerator SkillDelay()
    {
        timer = 0;
        isActive = true;
        yield return new WaitForSeconds(CurDelay);
        isActive = false;
    }
    void Skill()
    {
        // 사제 11스킬 자신은 회복 나머지 모든 플레이어는 부활 및 쿨타임 초기화가 적용됨
        if(GameManager.instance.skillContext.PriestSkill11()){
            foreach(Player pl in GameManager.instance.players){
                if(pl.character.Equals("사제")){
                    pl.GetDamage(pl.CurHP - pl.CurMaxHP, false, null, true);
                } else {
                    if(pl.playerDead){
                        pl.Revival();
                        if (pl.GetComponentInChildren<ActiveSkill>().isActive)
                        {
                            pl.GetComponentInChildren<ActiveSkill>().StopAllCoroutines();
                            pl.GetComponentInChildren<ActiveSkill>().isActive = false;
                            pl.GetComponentInChildren<ActiveSkill>().timer = pl.GetComponentInChildren<ActiveSkill>().CurDelay;
                        }
                        if (Vector3.Distance(GameManager.instance.playerControl.mainCharacter.transform.position, pl.transform.position) > pl.gameObject.GetComponent<CharacterAI>().distWithinMainCharacter)
                        {
                            // 멀리서 부활했을 경우 플레이어 이동
                            pl.transform.position = GameManager.instance.playerControl.mainCharacter.transform.position;
                        }
                        // 회생 이펙트 생성
                        Transform revivalEffect = poolBuffEffect.Get().transform;
                        Bullet revivalScript = revivalEffect.GetComponent<Bullet>();
                        revivalScript.Fire(0, -1, Vector3.zero, 0, 1, false, true, false);
                        revivalEffect.position = pl.transform.position;

                        // 사제 6스킬 해당 플레이어 5초간 무적
                        if(GameManager.instance.skillContext.PriestSkill6() > 0)
                            StartCoroutine(pl.InvincibleBuff(GameManager.instance.skillContext.PriestSkill6()));
                        
                        // 사제 7스킬 해당 플레이어 10초간 데미지와 공속 증가
                        if(GameManager.instance.skillContext.PriestSkill7(1) > 0)
                            StartCoroutine(pl.PriestSkill7Buff(10f));
                    } else {
                        pl.GetDamage(pl.CurHP - pl.CurMaxHP, false, null, true);
                        if(pl.GetComponentInChildren<ActiveSkill>().isActive && pl != player){
                            pl.GetComponentInChildren<ActiveSkill>().StopCoroutine(SkillDelay());
                            pl.GetComponentInChildren<ActiveSkill>().isActive = false;
                            pl.GetComponentInChildren<ActiveSkill>().timer = pl.GetComponentInChildren<ActiveSkill>().CurDelay;
                        }
                        // 사제 6스킬 해당 플레이어 5초간 무적
                        if(GameManager.instance.skillContext.PriestSkill6() > 0)
                            StartCoroutine(pl.InvincibleBuff(GameManager.instance.skillContext.PriestSkill6()));
                        
                        // 사제 7스킬 해당 플레이어 10초간 데미지와 공속 증가
                        if(GameManager.instance.skillContext.PriestSkill7(1) > 0)
                            StartCoroutine(pl.PriestSkill7Buff(10f));
                    }
                }
            }
        } else {
            Player minHealthPlayer = GameManager.instance.players[0];
            for (int i = 0; i < GameManager.instance.players.Length; i++)
            {
                if (GameManager.instance.players[i].playerDead)
                {
                    // 죽은 캐릭터일 경우 전부 회복 및 부활 및 스킬 쿨타임 초기화
                    minHealthPlayer = GameManager.instance.players[i];
                    minHealthPlayer.Revival();
                    if (minHealthPlayer.GetComponentInChildren<ActiveSkill>().isActive)
                    {
                        minHealthPlayer.GetComponentInChildren<ActiveSkill>().StopAllCoroutines();
                        minHealthPlayer.GetComponentInChildren<ActiveSkill>().isActive = false;
                        minHealthPlayer.GetComponentInChildren<ActiveSkill>().timer = minHealthPlayer.GetComponentInChildren<ActiveSkill>().CurDelay;
                    }
                    if (Vector3.Distance(GameManager.instance.playerControl.mainCharacter.transform.position, minHealthPlayer.transform.position) > minHealthPlayer.gameObject.GetComponent<CharacterAI>().distWithinMainCharacter)
                    {
                        // 멀리서 부활했을 경우 플레이어 이동
                        minHealthPlayer.transform.position = GameManager.instance.playerControl.mainCharacter.transform.position;
                    }
                    // 회생 이펙트 생성
                    Transform revivalEffect = poolBuffEffect.Get().transform;
                    Bullet revivalScript = revivalEffect.GetComponent<Bullet>();
                    revivalScript.Fire(0, -1, Vector3.zero, 0, 1, false, true, false);
                    revivalEffect.position = minHealthPlayer.transform.position;

                    // 사제 6스킬 해당 플레이어 5초간 무적
                    if(GameManager.instance.skillContext.PriestSkill6() > 0)
                        StartCoroutine(minHealthPlayer.InvincibleBuff(GameManager.instance.skillContext.PriestSkill6()));

                    // 사제 7스킬 해당 플레이어 10초간 데미지와 공속 증가
                    if(GameManager.instance.skillContext.PriestSkill7(1) > 0)
                        StartCoroutine(minHealthPlayer.PriestSkill7Buff(10f));
                    return;
                }
                else if (GameManager.instance.players[i].CurHP / GameManager.instance.players[i].CurMaxHP < minHealthPlayer.CurHP / minHealthPlayer.CurMaxHP)
                {
                    minHealthPlayer = GameManager.instance.players[i];
                }
            }
            // 전부 회복 및 스킬 쿨타임 초기화
            minHealthPlayer.GetDamage(minHealthPlayer.CurHP - minHealthPlayer.CurMaxHP, false, null, true);

            // minHealthPlayer.CreateFollowingHpBar();
            if (minHealthPlayer.GetComponentInChildren<ActiveSkill>().isActive && minHealthPlayer != player)
            {
                minHealthPlayer.GetComponentInChildren<ActiveSkill>().StopCoroutine(SkillDelay());
                minHealthPlayer.GetComponentInChildren<ActiveSkill>().isActive = false;
                minHealthPlayer.GetComponentInChildren<ActiveSkill>().timer = minHealthPlayer.GetComponentInChildren<ActiveSkill>().CurDelay;
            }
            // 사제 6스킬 해당 플레이어 5초간 무적
            if(GameManager.instance.skillContext.PriestSkill6() > 0)
                StartCoroutine(minHealthPlayer.InvincibleBuff(GameManager.instance.skillContext.PriestSkill6()));

            // 사제 7스킬 해당 플레이어 10초간 데미지와 공속 증가
            if(GameManager.instance.skillContext.PriestSkill7(1) > 0)
                StartCoroutine(minHealthPlayer.PriestSkill7Buff(10f));
        }
    }
}
