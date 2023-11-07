using olimsko;
using Unity.VisualScripting;
using UnityEngine;

public class SwordWeapon : MeleeWeapon
{
    protected override void Awake()
    {
        base.Awake();
        CurDetectionAngle = detectionAngle;
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
        float[] skill1Values = LevelUpSkills.WorriorSkill1(scalex, detectionAngle);
        scalex = skill1Values[0];
        CurDetectionAngle = skill1Values[1];

        Transform bullet = poolBullet.Get().transform;

        bullet.parent = GameManager.instance.pool.transform;
        bullet.transform.localScale = new Vector3(scalex, bullet.transform.localScale.y, bullet.transform.localScale.z);
        bullet.position = transform.position + dir * spawnDistance;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(GetComponentInParent<Player>(), Damage, out bool isCritical), count, Vector3.zero, knockBackPower, duration, isCritical);
        bullet.GetComponent<EffectBullet>().detectionAngle = CurDetectionAngle;

        // 용사 0번 스킬 숙련된 베기 하프에서 서클로 변경
        LevelUpSkills.WorriorSkill0(transform, poolBullet, dir, scalex, spawnDistance, CurDetectionAngle, player, Damage, count, knockBackPower, duration);

        AudioManager.Instance.SfxPlay(AudioManager.Sfx.Worrior_Attack);

        if (warriorFire)
        {
            bullet.GetComponent<FireSword>().warriorFire = true;
            bullet.GetComponent<FireSword>().warriorFireDamge = warriorFireDamge;
            bullet.GetComponent<FireSword>().warriorFireTime = warriorFireTime;
        }
    }
}
