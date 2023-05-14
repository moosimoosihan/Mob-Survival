using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    GameObject hitEffectPrefab;

    public float damage;
    public int per;
    protected bool hitOnlyOnce = true;

    [SerializeField]
    protected float duration = 3;

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

    public virtual void Init(float _damage, int _per, bool _deActivate = false)
    {
        damage = _damage;
        per = _per;

        //duration���Ŀ� �Ѿ� ��Ȱ��ȭ
        if (_deActivate)
        {
            DeActivate(duration);            
        }
    }

    public virtual void Fire(float _damage, int _per, Vector3 _dir, bool _deActivate = true, bool _hitOnlyOnce = true)
    {
        damage = _damage;
        per = _per;
        hitOnlyOnce = _hitOnlyOnce;

        if (per > -1)
        {
            rigid.velocity = _dir * 15f;
        }

        //duration���Ŀ� �Ѿ� ��Ȱ��ȭ
        if (_deActivate)
        {
            DeActivate(duration);
        }
    }

    

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        OnTriggerEnter2DUpdate(collision);
    }

    public virtual void OnTriggerEnter2DUpdate(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy") || per == -1)
            return;

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
                        tempIsHit = detectedEnemy.GetDamage(damage);
                }
            }
            else
            {
                tempIsHit = detectedEnemy.GetDamage(damage);
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
