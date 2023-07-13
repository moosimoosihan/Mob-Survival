using UnityEngine;
using System.Collections;

public class IceActiveSkill : ActiveSkill
{
    public float damage;
    public float skillDuration;
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
        skillArea = transform.GetChild(0).gameObject;
        player = GetComponentInParent<Player>();
    }
    public override void AreaOff()
    {
        areaOn = false;
        skillArea.SetActive(areaOn);
    }
    public override void AreaUpdate()
    {
        if(!isActive && GameManager.instance.inputManager.GetAction("ReadyActiveSkill").IsPressed())
        {
            areaOn = true;
        } else if(areaOn && GameManager.instance.inputManager.GetAction("CancelActiveSkill").IsPressed())
        {
            areaOn = false;
        }

        skillArea.SetActive(areaOn);
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        skillArea.transform.position = mousePos;
    }
    public override void ActiveSkillUpdate(){
        if(areaOn){
            Debug.Log("사제 스킬 시전");
            StartCoroutine(SkillDelay());
            Skill(skillArea.transform.position);
        }
    }
    IEnumerator SkillDelay(){
        timer = 0;
        areaOn = false;
        isActive = true;
        yield return new WaitForSeconds(delay);
        isActive = false;
    }
    void Skill(Vector2 vec){
        // 해당 마우스 지점에 블리자드 생성
        Transform bullet = poolBullet.Get().transform;
        bullet.position = vec;
        bullet.GetComponent<Bullet>().Init(damage, -1, 0, skillDuration, false, true);
    }
}
