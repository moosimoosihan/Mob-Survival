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
        float scaley = player.transform.localScale.y / 2;


        // 용사 1번 스킬 범위증가(베기) 베기스킬 범위 2배 증가
        float[] skill1Values = LevelUpSkills.WorriorSkill1(scalex, DetectionAngle);
        scalex = skill1Values[0];
        CurDetectionAngle = skill1Values[1];

        // 용사 7번 스킬 range 증가
        float[] skill6Values = LevelUpSkills.WorriorSkill7(scaley, DetectRadius);
        scaley = skill6Values[0];
        CurDetectRadius = skill6Values[1];

        Transform bullet = poolBullet.Get().transform;

        bullet.parent = GameManager.instance.pool.transform;
        bullet.transform.localScale = new Vector3(scalex, scaley, bullet.transform.localScale.z);
        bullet.position = transform.position + dir * spawnDistance;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(GetComponentInParent<Player>(), Damage, out bool isCritical), count, Vector3.zero, knockBackPower, duration, isCritical);
        bullet.GetComponent<EffectBullet>().DetectionAngle = CurDetectionAngle;
        bullet.GetComponent<EffectBullet>().AttackRadius = CurDetectRadius;

        // 용사 0번 스킬 숙련된 베기 하프에서 서클로 변경
        LevelUpSkills.WorriorSkill0(transform, poolBullet, dir, scalex, spawnDistance, CurDetectionAngle, player, Damage, count, knockBackPower, duration, scaley, DetectRadius);

        // 오디오 재생
        AudioManager.Instance.SfxPlay(AudioManager.Sfx.Worrior_Attack);

        if (warriorFire)
        {
            // 용사 5스킬 화상 피해 증가 10마리당 0.1% 증가
            CurWarriorFireDamage = LevelUpSkills.WorriorSkill5(WarriorFireDamage, GameManager.instance.kill);

            // 용사 7스킬 화상 스킬 지속시간 증가
            CurWarriorFireTime = LevelUpSkills.WorriorSkill8(WarriorFireTime);

            // 용사 14스킬 화상 스킬의 데미지 레벨당 0.1% 증가
            CurWarriorFireDamage += WarriorFireDamage - LevelUpSkills.WorriorSkill14(WarriorFireDamage);

            bullet.GetComponent<FireSword>().warriorFire = true;
            bullet.GetComponent<FireSword>().warriorFireDamge = CurWarriorFireDamage;
            bullet.GetComponent<FireSword>().warriorFireTime = CurWarriorFireTime;

            // 용사 10스킬 화상 디버프가 있는 몬스터 사망시 주변 몬스터에게 화상 디버프 부여
            bullet.GetComponent<FireSword>().fireBurn = LevelUpSkills.WorriorSkill10();
        }
    }
}
