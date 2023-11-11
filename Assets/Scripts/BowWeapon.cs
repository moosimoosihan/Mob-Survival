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
    }
}
