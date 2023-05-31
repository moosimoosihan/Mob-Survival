using System.Collections;
using UnityEngine;

public class WarriorActiveSkill : ActiveSkill
{
    public float damage;
    public float bulletSpeed;
    public float knockBackPower;
    public float bulletDuration;
    public override void ActiveSkillInit(){
        skillArea = transform.GetChild(0).gameObject;
        player = GetComponentInParent<Player>();
    }
    public override void ActiveSkillUpdate(){
        if(!player.inputEnabled || !GameManager.instance.isPlay){
            areaOn = false;
            skillArea.SetActive(areaOn);
            return;
        }
        
        if(!isActive && areaOn && Input.GetKeyDown(KeyCode.Mouse0)){
            Debug.Log("전사 스킬 시전");
            StartCoroutine(SkillDelay());
            Skill(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

        if(!isActive && Input.GetKeyDown(KeyCode.Mouse0)){
            areaOn = true;
        } else if(areaOn && Input.GetKeyDown(KeyCode.Mouse1)){
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
                
        Transform bullet = GameManager.instance.pool.Get(projectilePrefab).transform;

        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().speed = bulletSpeed;
        bullet.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(player,damage,out bool isCritical), 10, dir, knockBackPower, bulletDuration, isCritical);
    }
}
