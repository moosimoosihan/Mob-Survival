using System.Collections;
using System.Collections.Generic;
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

        //duration이후에 총알 비활성화
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

        //duration이후에 총알 비활성화
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

            //피격 이펙트
            if (hitEffectPrefab != null)
            {
                HitEffect bulletHitEffect = hitEffectPool.Get();
                bulletHitEffect.transform.parent = GameManager.instance.pool.transform;
                bulletHitEffect.transform.position = transform.position;
            }

            if (hitOnlyOnce)
            {
                //처음 닿은 대상인 경우 데미지 주고 데미지 입힌 리스트에 보관
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

        // 피격 이후 생성하는 총알의 경우
        OnCreateBullet();

        //이미 맞아서 죽어야되는애가 뒤에 오는 총알 맞았을때는 총알이 그냥 지나가게하기
        if(tempIsHit && !throwBullet)
            per--;

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
            detectedEnemyList.Clear();  //비활성화시 이전에 데미지 줬던 녀석들 리스트에서 해제
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
}
