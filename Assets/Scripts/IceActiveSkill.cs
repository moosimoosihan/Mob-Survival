using UnityEngine;
using System.Collections;

public class IceActiveSkill : ActiveSkill
{
    [SerializeField]
    private float skillDuration;
    [SerializeField]
    private float curSkillDuration;
    public float CurSkillDuration
    {
        get
        {
            curSkillDuration = skillDuration;
            
            // 현자 7스킬 얼음 장판 지속시간 증가
            curSkillDuration += GameManager.instance.skillContext.WizardSkill7()[0];

            // 현자 11스킬 얼음 장판 지속시간 감소
            curSkillDuration -= GameManager.instance.skillContext.WizardSkill11()[0];
            
            return curSkillDuration;
        }
        set
        {
            curSkillDuration = value;
        }
    }

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

        // 현자 11스킬 장판 맵 전체 적용
        if(GameManager.instance.skillContext.WizardSkill11()[0]!=0){
            skillArea.transform.localScale = new Vector3(200, 100, 0);
            skillArea.transform.position = new Vector3(0, 0, 0);
            return;
        }

        // 현자 6스킬 아이스그라운드 사이즈 증가로인한 스킬영역 사이즈 증가
        if(GameManager.instance.skillContext.WizardSkill6()!=0){
            float val = GameManager.instance.skillContext.WizardSkill6();
            transform.localScale = new Vector3(val, val, val);
        }
        
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
        yield return new WaitForSeconds(CurDelay);
        isActive = false;
    }
    void Skill(Vector2 vec){
        // 해당 마우스 지점에 블리자드 생성
        Transform bullet = poolBullet.Get().transform;

        // 현자 6스킬 아이스그라운드 사이즈 증가
        if(GameManager.instance.skillContext.WizardSkill6()!=0){
            float val = GameManager.instance.skillContext.WizardSkill6();
            bullet.localScale = new Vector3(val, val, val);
        }

        

        bullet.parent = GameManager.instance.pool.transform;
        
        // 현자 11스킬 아이스그라운드 맵 전체 적용
        if(GameManager.instance.skillContext.WizardSkill11()[0]!=0){
            bullet.localScale = new Vector3(20, 25, 0);
            bullet.position = Vector2.zero;
        } else {
            bullet.position = vec;
        }

        bullet.GetComponent<Bullet>().Init(Damage, -1, 0, skillDuration, false, true);
    }
}
