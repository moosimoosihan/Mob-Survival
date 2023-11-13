using UnityEngine;

public class BowWeapon : Weapon
{
    int bowCount = 0;
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Update()
    {
        base.Update();
    }

    protected override void Fire()
    {
        if (!player.scanner.nearestTarget)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        Transform bullet = poolBullet.Get().transform;

        bullet.position = transform.position;
        bullet.parent = GameManager.instance.pool.transform;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().speed = bulletSpeed;
        
        // 궁수 10스킬 유도탄으로 변경
        bullet.GetComponent<Bullet>().isHoming = GameManager.instance.skillContext.ArcherSkill10();

        // 궁수 3스킬 5번 쏘면 100% 크리티컬
        if(GameManager.instance.skillContext.ActherSkill3()){
            bowCount++;
            if(bowCount >= 5){
                bullet.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(GetComponentInParent<Player>(),Damage, out bool isCritical), CurCount, dir, knockBackPower, duration, true);
                this.isCritical = true;
                bowCount = 0;
            } else {
                bullet.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(GetComponentInParent<Player>(),Damage,out bool isCritical), CurCount, dir, knockBackPower, duration, isCritical);
                this.isCritical = isCritical;
            }
        } else {
            bullet.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(GetComponentInParent<Player>(),Damage,out bool isCritical), CurCount, dir, knockBackPower, duration, isCritical);
            this.isCritical = isCritical;
        }        

        AudioManager.Instance.SfxPlay(AudioManager.Sfx.Archer_Attack);

        // 궁수 11스킬 화살 2개가 가운데 화살을 중심으로 양 옆 30도정도로 퍼져나간다.
        if(GameManager.instance.skillContext.ArcherSkill11()){
            Transform bullet2 = poolBullet.Get().transform;
            // 왼쪽 화살
            Vector3 dir2 = Quaternion.Euler(0,0,30) * dir;
            bullet2.position = transform.position;
            bullet2.parent = GameManager.instance.pool.transform;
            bullet2.rotation = Quaternion.FromToRotation(Vector3.up, dir2);
            bullet2.GetComponent<Bullet>().speed = bulletSpeed;
            bullet2.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(GetComponentInParent<Player>(),Damage,out bool isCritical2), CurCount, dir2, knockBackPower, duration, isCritical);
            isCritical = isCritical2;

            Transform bullet3 = poolBullet.Get().transform;
            // 오른쪽 화살
            Vector3 dir3 = Quaternion.Euler(0,0,-30) * dir;
            bullet3.position = transform.position;
            bullet3.parent = GameManager.instance.pool.transform;
            bullet3.rotation = Quaternion.FromToRotation(Vector3.up, dir3);
            bullet3.GetComponent<Bullet>().speed = bulletSpeed;
            bullet3.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(GetComponentInParent<Player>(),Damage,out bool isCritical3), CurCount, dir3, knockBackPower, duration, isCritical);
            isCritical = isCritical3;
        }
    }
}
