using System.Collections;
using UnityEngine;

public class WarriorActiveSkill : ActiveSkill
{
    public int count;
    public float bulletSpeed;
    public float knockBackPower;
    public float bulletDuration;

    public bool warriorFire;
    public float warriorFireDamge;
    public float warriorFireTime;

    [SerializeField]
    private GameObject skillArea2;
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
    public override void AreaUpdate(){
        if(!isActive && GameManager.instance.inputManager.GetAction("ReadyActiveSkill").IsPressed())
        {
            areaOn = true;
        } else if(areaOn && GameManager.instance.inputManager.GetAction("CancelActiveSkill").IsPressed())
        {
            areaOn = false;
        }

        skillArea.SetActive(areaOn);

        // 전사 11스킬 영역 2개
        if(skillArea.activeSelf){
            if(GameManager.instance.skillContext.WarriorSkill11()){
                if(!skillArea2.activeSelf)
                    skillArea2.SetActive(areaOn);
            } else {
                if(skillArea2.activeSelf)
                    skillArea2.SetActive(!areaOn);
            }
        }
        // 마우스를 따라 회전
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = skillArea.transform.position.z; // skillArea의 현재 z 축 값 유지

        Vector3 direction = mousePos - skillArea.transform.position;
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction);
        skillArea.transform.rotation = rotation;
    }
    public override void ActiveSkillUpdate(){        
        if(areaOn){
            Debug.Log("전사 스킬 시전");
            StartCoroutine(SkillDelay());
            Skill(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }
    IEnumerator SkillDelay(){
        timer = 0;
        areaOn = false;
        isActive = true;
        yield return new WaitForSeconds(CurDelay);
        isActive = false;
    }
    void Skill(Vector2 vec, bool clone = false){
        // 해당 마우스 지점에 화염강타 발사
        Vector2 myVec = transform.position;
        Vector2 dir = vec - myVec;
        dir = dir.normalized;
        
        Transform bullet = poolBullet.Get().transform;
        float scaley = Mathf.Abs(bullet.localScale.y);
        scaley += GameManager.instance.skillContext.WarriorSkill6();
        bullet.localScale = new Vector3(bullet.localScale.x, scaley, bullet.localScale.z);

        bullet.parent = GameManager.instance.pool.transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.left, dir);
        FireStrike bulletScript = bullet.GetComponent<FireStrike>();

        // 전사 스킬 7번 투사체 속도 2배
        if(GameManager.instance.skillContext.WarriorSkill7()!=0){
            bulletScript.speed = bulletSpeed * GameManager.instance.skillContext.WarriorSkill7();
        } else {
            bulletScript.speed = bulletSpeed;
        }

        bulletScript.throwBullet = true;
        bulletScript.player = player;
        bulletScript.Fire(DamageManager.Instance.Critical(player,Damage,out bool isCritical), 0, dir, knockBackPower, bulletDuration, isCritical);

        AudioManager.Instance.SfxPlay(AudioManager.Sfx.Warrior_FireStrike);

        if(warriorFire){
            bulletScript.warriorFire = true;
            bulletScript.warriorFireDamge = warriorFireDamge;
            bulletScript.warriorFireTime = warriorFireTime;
        }

        if(GameManager.instance.skillContext.WarriorSkill11() && !clone){
            // 전사 11스킬 vec 위치 반대로 나가는 투사체 추가
            Vector2 vec2 = myVec - vec;
            Skill(vec2, true);
        }
    }
}
