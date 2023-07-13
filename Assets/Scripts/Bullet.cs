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

        //duration���Ŀ� �Ѿ� ��Ȱ��ȭ
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

        //duration���Ŀ� �Ѿ� ��Ȱ��ȭ
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

            //�ǰ� ����Ʈ
            if (hitEffectPrefab != null)
            {
                HitEffect bulletHitEffect = hitEffectPool.Get();
                bulletHitEffect.transform.parent = GameManager.instance.pool.transform;
                bulletHitEffect.transform.position = transform.position;
            }

            if (hitOnlyOnce)
            {
                //ó�� ���� ����� ��� ������ �ְ� ������ ���� ����Ʈ�� ����
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

        // �ǰ� ���� �����ϴ� �Ѿ��� ���
        OnCreateBullet();

        //�̹� �¾Ƽ� �׾�ߵǴ¾ְ� �ڿ� ���� �Ѿ� �¾������� �Ѿ��� �׳� ���������ϱ�
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
            detectedEnemyList.Clear();  //��Ȱ��ȭ�� ������ ������ ��� �༮�� ����Ʈ���� ����
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
