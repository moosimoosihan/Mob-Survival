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
            Vector2 size = bullet.transform.localScale;
            size = new Vector2(val,val);
            bullet.transform.localScale = size;
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
        
        AudioManager.Instance.SfxPlay(AudioManager.Sfx.Wizard_Attack);
    }

}