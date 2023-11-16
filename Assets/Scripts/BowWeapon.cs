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
        bullet.GetComponent<Bullet>().player = player;
        
        // 궁수 10스킬 유도탄으로 변경
        bullet.GetComponent<Bullet>().isHoming = GameManager.instance.skillContext.ArcherSkill10();

        // 궁수 3스킬 5번 쏘면 100% 크리티컬
        if(GameManager.instance.skillContext.ActherSkill3()){
            if(bowCount >= 5){
                bullet.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(player,Damage, out bool isCritical, true), CurCount, dir, knockBackPower, duration, isCritical);
                if(isCritical || this.isCritical) {this.isCritical=true;} else {this.isCritical=false;}
                bowCount = 0;
            } else {
                bowCount++;
                bullet.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(player,Damage,out bool isCritical), CurCount, dir, knockBackPower, duration, isCritical);
                if(isCritical || this.isCritical) {this.isCritical=true;} else {this.isCritical=false;}
            }
        } else {
            bullet.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(player,Damage,out bool isCritical), CurCount, dir, knockBackPower, duration, isCritical);
            if(isCritical || this.isCritical) {this.isCritical=true;} else {this.isCritical=false;}
        }        

        AudioManager.Instance.SfxPlay(AudioManager.Sfx.Archer_Attack);        

        // 화살 개수 증가시 양 옆으로 각도를 조금씩 틀어서 쏘도록 변경
        if(CurProjectilePrefabNum > 1){
            for(int i=1;i<=CurProjectilePrefabNum-1;i++){
                Transform bullets = poolBullet.Get().transform;
                
                int a = i%2 == 0 ? 1 : -1;
                Vector3 dirs = Quaternion.Euler(0,0,10*(i%2==0?i-1:i)*a) * dir;
                bullets.position = transform.position;
                bullets.parent = GameManager.instance.pool.transform;
                bullets.rotation = Quaternion.FromToRotation(Vector3.up, dirs);
                bullets.GetComponent<Bullet>().speed = bulletSpeed;
                bullets.GetComponent<Bullet>().player = player;
                // 궁수 10스킬 유도탄으로 변경
                bullets.GetComponent<Bullet>().isHoming = GameManager.instance.skillContext.ArcherSkill10();
                // 궁수 3스킬 5번 쏘면 100% 크리티컬
                if(GameManager.instance.skillContext.ActherSkill3()){
                    if(bowCount >= 5){
                        bullets.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(player,Damage, out bool isCritical, true), CurCount, dirs, knockBackPower, duration, isCritical);
                        if(isCritical || this.isCritical) {this.isCritical=true;} else {this.isCritical=false;}
                        bowCount = 0;
                    } else {
                        bowCount++;
                        bullets.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(player,Damage,out bool isCritical), CurCount, dirs, knockBackPower, duration, isCritical);
                        if(isCritical || this.isCritical) {this.isCritical=true;} else {this.isCritical=false;}
                    }
                } else {
                    bullets.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(player,Damage,out bool isCritical), CurCount, dirs, knockBackPower, duration, isCritical);
                    if(isCritical || this.isCritical) {this.isCritical=true;} else {this.isCritical=false;}
                }
            }
        }
    }
}
