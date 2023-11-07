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
        yield return new WaitForSeconds(delay);
        isActive = false;
    }
    void Skill(Vector2 vec){
        // 해당 마우스 지점에 화염강타 발사
        Vector2 myVec = transform.position;
        Vector2 dir = vec - myVec;
        dir = dir.normalized;
                
        Transform bullet = poolBullet.Get().transform;

        bullet.parent = GameManager.instance.pool.transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.left, dir);
        FireStrike bulletScript = bullet.GetComponent<FireStrike>();
        bulletScript.speed = bulletSpeed;
        bulletScript.throwBullet = true;
        bulletScript.Fire(DamageManager.Instance.Critical(player,Damage,out bool isCritical), 0, dir, knockBackPower, bulletDuration, isCritical);

        AudioManager.Instance.SfxPlay(AudioManager.Sfx.Worrior_FireStrike);

        if(warriorFire){
            bulletScript.warriorFire = true;
            bulletScript.warriorFireDamge = warriorFireDamge;
            bulletScript.warriorFireTime = warriorFireTime;
        }
    }
}
