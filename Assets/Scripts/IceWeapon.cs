using UnityEngine;

public class IceWeapon : Weapon
{
    public GameObject iceGroundPrefab;
    public float groundDamage;
    public float groundDuration;

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

        // 현자 0스킬 크기 증가
        if(GameManager.instance.skillContext.WizardSkill0()[1]!=0){
            float val = GameManager.instance.skillContext.WizardSkill0()[1];
            bullet.transform.localScale = new Vector2(val,val);
        }

        bullet.parent = GameManager.instance.pool.transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.left, dir);
        bullet.GetComponent<IceBullet>().projectilePrefab = iceGroundPrefab;
        bullet.GetComponent<IceBullet>().groundDamage = groundDamage;
        bullet.GetComponent<IceBullet>().GroundDuration = groundDuration;
        bullet.GetComponent<IceBullet>().speed = bulletSpeed;
        bullet.GetComponent<IceBullet>().player = player;
        bullet.GetComponent<Bullet>().Fire(DamageManager.Instance.Critical(GetComponentInParent<Player>(),Damage,out bool isCritical), CurCount, dir, knockBackPower, duration, isCritical);
        if(isCritical || this.isCritical) {this.isCritical=true;} else {this.isCritical=false;}
        
        AudioManager.Instance.SfxPlay(AudioManager.Sfx.Wizard_Attack);

        // 개수 증가시 양 옆으로 각도를 틀어서 쏘도록 변경
        if(CurProjectilePrefabNum > 1){
            for(int i=1;i<=CurProjectilePrefabNum-1;i++){
                Transform bullets = poolBullet.Get().transform;

                // 현자 0스킬 크기 증가
                if(GameManager.instance.skillContext.WizardSkill0()[1]!=0){
                    float val = GameManager.instance.skillContext.WizardSkill0()[1];
                    bullets.transform.localScale = new Vector2(val,val);
                }

                int a = i%2 == 0 ? 1 : -1;
                Vector3 dirs = Quaternion.Euler(0,0,10*(i%2==0?i-1:i)*a) * dir;
                bullets.position = transform.position;
                bullets.parent = GameManager.instance.pool.transform;
                bullets.rotation = Quaternion.FromToRotation(Vector3.left, dirs);
                bullets.GetComponent<IceBullet>().projectilePrefab = iceGroundPrefab;
                bullets.GetComponent<IceBullet>().groundDamage = groundDamage;
                bullets.GetComponent<IceBullet>().GroundDuration = groundDuration;
                bullets.GetComponent<IceBullet>().speed = bulletSpeed;
                bullets.GetComponent<IceBullet>().player = player;
                bullets.GetComponent<IceBullet>().Fire(DamageManager.Instance.Critical(player,Damage,out bool isCriticals), CurCount, dirs, knockBackPower, duration, isCriticals);
                if(isCriticals || this.isCritical) {this.isCritical=true;} else {this.isCritical=false;}
            }
        }
    }

}