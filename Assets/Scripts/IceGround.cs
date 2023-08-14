using UnityEngine;

public class IceGround : Bullet
{
    public float time;
    GameObject loopSfxPlayer;

    // 현재 여러마리가 올라와 있으면 빠르게 데미지를 주는 현상이 있음 교체해야 함!
    public virtual void OnTriggerStay2D(Collider2D collision) {
        if (!collision.CompareTag("Enemy"))
            return;
        
        time += Time.deltaTime;
        if(time>1){
            Enemy detectedEnemy = collision.GetComponent<Enemy>();
            detectedEnemy.GetDamage(damage, 0, false, true);
            time = 0;
        }
    }
    void OnEnable()
    {
        loopSfxPlayer = AudioManager.Instance.LoopSfxPlay(AudioManager.LoopSfx.Wizard_IceAge);
    }
    void OnDisable()
    {
        loopSfxPlayer.GetComponent<LoopSFXPlayer>().Stop();
    }
}
