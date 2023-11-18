using olimsko;
using Unity.VisualScripting;
using UnityEngine;

public class SwordWeapon : MeleeWeapon
{
    protected override void Awake()
    {
        base.Awake();
        CurDetectionAngle = DetectionAngle;
    }
    protected override void Update()
    {
        base.Update();
    }
    protected override void OnFire(Transform _targetTransform)
    {
        Vector3 targetPos = _targetTransform.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        float scalex = player.transform.localScale.x / 2;

        // 용사 1번 스킬 범위증가(베기) 베기스킬 범위 2배 증가
        float[] skill1Values = GameManager.instance.skillContext.WarriorSkill1(scalex, DetectionAngle);
        scalex = skill1Values[0];
        CurDetectionAngle = skill1Values[1];

        Transform bullet = poolBullet.Get().transform;

        bullet.parent = GameManager.instance.pool.transform;
        bullet.transform.localScale = new Vector3(scalex, bullet.transform.localScale.y, bullet.transform.localScale.z);
        bullet.position = transform.position + dir * spawnDistance;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<FireSword>().player = player;
        bullet.GetComponent<FireSword>().DetectionAngle = CurDetectionAngle;
        bullet.GetComponent<FireSword>().AttackRadius = CurDetectRadius;
        bullet.GetComponent<FireSword>().Fire(DamageManager.Instance.Critical(GetComponentInParent<Player>(), Damage, out bool isCritical), CurCount, Vector3.zero, knockBackPower, duration, isCritical);
        if(isCritical || this.isCritical) {this.isCritical=true;} else {this.isCritical=false;}

        // 용사 0번 스킬 숙련된 베기 하프에서 서클로 변경
        if(GameManager.instance.skillContext.WarriorSkill0()){
            Transform bulletBack = poolBullet.Get().transform;

            Vector3 backDir = new Vector3(dir.x * -1, dir.y * -1, dir.z);
            bulletBack.parent = GameManager.instance.pool.transform;
            bulletBack.transform.localScale = new Vector3(scalex, bulletBack.transform.localScale.y, bulletBack.transform.localScale.z);
            bulletBack.position = transform.position + backDir * spawnDistance;
            bulletBack.rotation = Quaternion.FromToRotation(Vector3.down, dir);
            bulletBack.GetComponent<FireSword>().player = player;
            bulletBack.GetComponent<FireSword>().DetectionAngle = CurDetectionAngle;
            bulletBack.GetComponent<FireSword>().AttackRadius = CurDetectRadius;
            bulletBack.GetComponent<FireSword>().Fire(DamageManager.Instance.Critical(player, Damage, out bool isCriticalb), CurCount, Vector3.zero, knockBackPower, duration, isCriticalb);
            if(isCriticalb || this.isCritical) {this.isCritical=true;} else {this.isCritical=false;}
        }

        // 오디오 재생
        AudioManager.Instance.SfxPlay(AudioManager.Sfx.Warrior_Attack);

        if (warriorFire)
        {
            curWarriorFireDamage = warriorFireDamage;
            curWarriorFireTime = warriorFireTime;
            
            // 용사 5스킬 화상 피해 증가 10마리당 0.1% 증가
            curWarriorFireDamage += GameManager.instance.skillContext.WarriorSkill5(GameManager.instance.kill);

            // 용사 7스킬 화상 스킬 지속시간 증가
            curWarriorFireTime += GameManager.instance.skillContext.WarriorSkill8();

            // 용사 14스킬 화상 스킬의 데미지 레벨당 0.1% 증가
            curWarriorFireDamage += GameManager.instance.skillContext.WarriorSkill14();

            bullet.GetComponent<FireSword>().warriorFire = true;
            bullet.GetComponent<FireSword>().warriorFireDamge = curWarriorFireDamage;
            bullet.GetComponent<FireSword>().warriorFireTime = curWarriorFireTime;

            // 용사 10스킬 화상 디버프가 있는 몬스터 사망시 주변 몬스터에게 화상 디버프 부여
            bullet.GetComponent<FireSword>().fireBurn = GameManager.instance.skillContext.WarriorSkill10();
        }
    }
}
