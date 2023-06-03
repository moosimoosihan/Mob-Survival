using UnityEngine;
using System.Collections;

public class IceActiveSkill : ActiveSkill
{
    public float damage;
    public float skillDuration;
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
        if(!isActive && Input.GetKeyDown(KeyCode.Mouse0)){
            areaOn = true;
        } else if(areaOn && Input.GetKeyDown(KeyCode.Mouse1)){
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
        Transform bullet = GameManager.instance.pool.Get(projectilePrefab).transform;
        bullet.position = vec;
        bullet.GetComponent<Bullet>().Init(damage, -1, 0, skillDuration, false, true);
    }
}
