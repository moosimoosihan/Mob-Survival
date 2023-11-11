using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Pool;
public class Bullet : MonoBehaviour
{
    [SerializeField]
    public GameObject hitEffectPrefab;

    public float damage;
    public float speed;
    public int per;
    public float knockBackPower;
    public bool throwBullet;
    protected bool hitOnlyOnce = true;
    public bool isCritical;
    protected Rigidbody2D rigid;
    protected List<Enemy> detectedEnemyList = new List<Enemy>();
    private IObjectPool<Bullet> _ManagedPool;
    public IObjectPool<HitEffect> hitEffectPool;

    // 유도탄 여부
    public bool isHoming = false;
    [SerializeField]
    Transform homingTarget;

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        hitEffectPool = new ObjectPool<HitEffect>(CreateEffect, OnGetEffect, OnReleaseEffect, OnDestroyEffect);
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    public virtual void Init(float _damage, int _per, float _knockBackPower, float _duration,  bool _isCritical, bool _deActivate = false)
    {
        damage = _damage;
        per = _per;
        knockBackPower = _knockBackPower;
        isCritical = _isCritical;

        //duration???Ŀ? ??? ??????
        if (_deActivate)
        {
            DeActivate(_duration);            
        }
    }

    public virtual void Fire(float _damage, int _per, Vector3 _dir, float _knockBackPower, float _duration, bool _isCritical, bool _deActivate = true, bool _hitOnlyOnce = true)
    {
        damage = _damage;
        per = _per;
        hitOnlyOnce = _hitOnlyOnce;
        knockBackPower = _knockBackPower;
        isCritical = _isCritical;

        if (per > -1)
        {
            rigid.velocity = _dir * speed;
        }

        //duration???Ŀ? ??? ??????
        if (_deActivate)
        {
            DeActivate(_duration);
        }
    }

    

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        OnTriggerEnter2DUpdate(collision);
    }

    public virtual void OnTriggerEnter2DUpdate(Collider2D collision)
    {
        if(per == -1)
            return;

        if (!collision.CompareTag("Enemy")){
            if(throwBullet)
                return;
            
            if(collision.CompareTag("Wall")){
                DeActivate(0);
            }
            return;
        }

        bool tempIsHit = false;
        if (collision != null)
        {
            Enemy detectedEnemy = collision.GetComponent<Enemy>();

            //??? ?????
            if (hitEffectPrefab != null)
            {
                HitEffect bulletHitEffect = hitEffectPool.Get();
                bulletHitEffect.transform.parent = GameManager.instance.pool.transform;
                bulletHitEffect.transform.position = transform.position;
            }

            if (hitOnlyOnce)
            {
                //??? ???? ????? ??? ?????? ??? ?????? ???? ??????? ????
                if (detectedEnemyList.Contains(detectedEnemy) == false)
                {
                    detectedEnemyList.Add(detectedEnemy);
                    if (detectedEnemy.gameObject.activeSelf){
                        tempIsHit = detectedEnemy.GetDamage(damage, knockBackPower, isCritical);
                    }
                }
            }
            else
            {
                tempIsHit = detectedEnemy.GetDamage(damage, knockBackPower, isCritical);
            }
        }

        // ??? ???? ??????? ????? ???
        OnCreateBullet();

        //??? ?¾?? ?????¾?? ??? ???? ??? ?¾??????? ????? ??? ???????????
        if(tempIsHit && !throwBullet){
            per--;
            if(isHoming){
                isHoming = false;
                homingTarget = null;
                Invoke("FindEnemy", 0.5f);
            }
        }

        if (per == -1)
        {
            rigid.velocity = Vector2.zero;
            DeActivate(0);
        }
    }
    public virtual void OnCreateBullet(){

    }

    public virtual void DeActivate(float _inTime)
    {
        StartCoroutine(CoDelayStarter(() =>
        {
            _ManagedPool.Release(this);
            detectedEnemyList.Clear();  //???????? ?????? ?????? ??? ???? ????????? ????
        },
        _inTime
        ));
    }

    protected IEnumerator CoDelayStarter(System.Action _action, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _action.Invoke();
    }

    public void SetManagedPool(IObjectPool<Bullet> pool)
    {
        _ManagedPool = pool;
    }
    HitEffect CreateEffect()
    {
        HitEffect buffEffect = Instantiate(hitEffectPrefab).GetComponent<HitEffect>();
        buffEffect.SetManagedPool(hitEffectPool);
        return buffEffect;
    }
    void OnGetEffect(HitEffect buffEffect)
    {
        buffEffect.gameObject.SetActive(true);
    }
    void OnReleaseEffect(HitEffect buffEffect)
    {
        if (buffEffect.gameObject.activeSelf)
            buffEffect.gameObject.SetActive(false);
    }
    void OnDestroyEffect(HitEffect buffEffect)
    {
        Destroy(buffEffect.gameObject);
    }
    void OnEnable()
    {
        homingTarget = null;
        isHoming = false;
    }
    void Update()
    {
        // 유도탄일 경우 적군이 주변에 존재하면 적군을 향해 날아간다. 없을경우 정해진 방향으로 날아간다.
        if(isHoming){
            // 타겟이 반경 10안에 있다면 타겟을 지정한다.
            if(homingTarget==null){
                // 주변에 타겟이 이미 닿았던 detectedEnemyList안에 있다면 다른 적군을 찾고 만약 없다면 해당 아군을 지우고 다시 타겟한다.
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10f);
                foreach(Collider2D collider in colliders){
                    if(collider.CompareTag("Enemy") && !detectedEnemyList.Contains(collider.GetComponent<Enemy>())){
                        homingTarget = collider.transform;
                        break;
                    }
                }
                // 주변에 이미 맞은 타겟만 있다면 리스트를 비우고 다시 타겟을 지정한다.
                if(homingTarget == null){
                    detectedEnemyList.Clear();
                    foreach(Collider2D collider in colliders){
                        if(collider.CompareTag("Enemy")){
                            homingTarget = collider.transform;
                            break;
                        }
                    }
                }
            }
            if(homingTarget != null){
                Vector3 dir = homingTarget.position - transform.position;
                dir = dir.normalized;
                transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
                rigid.velocity = dir * speed;
            } else {
                rigid.velocity = transform.up * speed;
            }
        }        
    }
    void FindEnemy()
    {
        isHoming = true;
    }
}
