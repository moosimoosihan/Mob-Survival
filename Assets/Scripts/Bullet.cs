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
            rigid.velocity = _dir * 15f;
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
                GameObject bulletHitEffect = GameManager.instance.pool.Get(hitEffectPrefab);
                bulletHitEffect.transform.position = transform.position;
            }

            if (hitOnlyOnce)
            {
                //처음 닿은 대상인 경우 데미지 주고 데미지 입힌 리스트에 보관
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

        //이미 맞아서 죽어야되는애가 뒤에 오는 총알 맞았을때는 총알이 그냥 지나가게하기
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

}
