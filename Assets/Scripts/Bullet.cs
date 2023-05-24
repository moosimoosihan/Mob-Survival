using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    GameObject hitEffectPrefab;

    public float damage;
    public int per;
    public float knockBackPower;
    protected bool hitOnlyOnce = true;
    public bool isCritical;
    protected Rigidbody2D rigid;
    protected List<Enemy> detectedEnemyList = new List<Enemy>();

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
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
            rigid.velocity = _dir * 15f;
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
                GameObject bulletHitEffect = GameManager.instance.pool.Get(hitEffectPrefab);
                bulletHitEffect.transform.position = transform.position;
            }

            if (hitOnlyOnce)
            {
                //ó�� ���� ����� ��� ������ �ְ� ������ ���� ����Ʈ�� ����
                if (detectedEnemyList.Contains(detectedEnemy) == false)
                {
                    detectedEnemyList.Add(detectedEnemy);
                    if (detectedEnemy.gameObject.activeSelf)
                        tempIsHit = detectedEnemy.GetDamage(damage, knockBackPower, isCritical);
                }
            }
            else
            {
                tempIsHit = detectedEnemy.GetDamage(damage, knockBackPower, isCritical);
            }            
        }

        //�̹� �¾Ƽ� �׾�ߵǴ¾ְ� �ڿ� ���� �Ѿ� �¾������� �Ѿ��� �׳� ���������ϱ�
        if(tempIsHit)
            per--;

        if (per == -1)
        {
            rigid.velocity = Vector2.zero;
            DeActivate(0);
        }
    }

    public virtual void DeActivate(float _inTime)
    {
        StartCoroutine(CoDelayStarter(() =>
        {
            gameObject.SetActive(false);
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

}
