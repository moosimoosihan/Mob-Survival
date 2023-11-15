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
            detectedEnemy.GetDamage(damage, 0, false, null, true);
            time = 0;

            if(GameManager.instance.skillContext.WizardSkill3()){
                // 현자 스킬3 적중시 빙결1스택 증가 최대 4스택 빙결최대 3초
                if(!detectedEnemy.slowDeBuff && !detectedEnemy.stunDeBuff){
                    detectedEnemy.slowDeBuffTime = 3;
                    detectedEnemy.slowDeBuffCount++;
                    StartCoroutine(detectedEnemy.SlowDeBuff());
                } else {
                    detectedEnemy.slowDeBuffTime = 3;
                    if(detectedEnemy.slowDeBuffCount<4){
                        // 현자 8스킬 빙결 2스택 증가
                        if(GameManager.instance.skillContext.WizardSkill8()[0]!=0){
                            detectedEnemy.slowDeBuffCount+=2;
                            if(detectedEnemy.slowDeBuffCount>4)
                                detectedEnemy.slowDeBuffCount = 4;
                        } else {
                            detectedEnemy.slowDeBuffCount++;
                        }
                    } else if (detectedEnemy.slowDeBuffCount==4 && GameManager.instance.skillContext.WizardSkill9()){
                        // 현자 9스킬 빙결 4스택에서 한번 더 타격시 빙결 슬로우 관련 버프와 코루틴 제거 후 빙결 코루틴 시작 빙결은 2초 빙결시에는 슬로우 내성
                        detectedEnemy.slowDeBuffCount = 0;
                        detectedEnemy.slowDeBuffTime = 0;
                        detectedEnemy.slowDeBuff = false;
                        StopCoroutine(detectedEnemy.SlowDeBuff());

                        detectedEnemy.stunDeBuffTime = 2;
                        detectedEnemy.stunDeBuff = true;
                        StartCoroutine(detectedEnemy.StunDeBuff());

                        // 빙결 이펙트(만들기)
                    }
                }
                

            }
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
