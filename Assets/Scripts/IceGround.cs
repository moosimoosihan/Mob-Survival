using UnityEngine;

public class IceGround : Bullet
{
    public float time;

    public virtual void OnTriggerStay2D(Collider2D collision) {
        if (!collision.CompareTag("Enemy"))
            return;
        
        time += Time.deltaTime;
        if(time>1){
            Enemy detectedEnemy = collision.GetComponent<Enemy>();
            detectedEnemy.GetDamage(damage, 0, false);
            time = 0;
        }
    }
}
