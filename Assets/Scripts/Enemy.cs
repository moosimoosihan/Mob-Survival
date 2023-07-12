using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy : CharacterStatus
{
    [Header("적군 정보")]
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;
    public Rigidbody2D originTargetRigid2D;
    public float radius;
    public bool isLive;
    public bool knockBack;
    public float power = 1;
    

    public Rigidbody2D rigid;
    public Collider2D coll;
    public SpriteRenderer spriter;
    public Animator anim;
    WaitForFixedUpdate wait;
    public float attackDelay;

    bool isAttackable = true;

    public float fireDeBuffTime;
    public bool isFire;
    float fireTime;
    public float curFireDamage;
    BuffEffect effect;
    public Transform nearestTarget;

    private IObjectPool<Enemy> _ManagedPool;

    void Awake()
    {
        _Awake();
        CreateFollowingHpBar();
    }    
    public virtual void _Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();

        radius = (coll as CapsuleCollider2D).size.x * transform.localScale.x / 2;
    }

    void Update()
    {
        if (!isLive || !GameManager.instance.isPlay)
            return;

        if (isFire)
        {
            fireTime += Time.deltaTime;
            if (fireTime >= 1)
            {
                fireTime = 0;
                GetDamage(curFireDamage, 0, false);
            }
        }
    }

    private void FindClosestObject()
    {
        if (!isLive || !GameManager.instance.isPlay)
            return;

        float maxDistance = 1000f;
        float playerCount = 0;
        foreach (Player player in GameManager.instance.players)
        {
            if (!player.playerDead)
            {
                playerCount++;
                float distance = Vector2.Distance(transform.position, player.transform.position);
                if (distance < maxDistance)
                {
                    maxDistance = distance;
                    nearestTarget = player.gameObject.transform;
                }
            }
        }
        if (playerCount == 0)
        {
            nearestTarget = null;
        }
    }

    void FixedUpdate()
    {
        _FixedUpdate();
    }
    public virtual void _FixedUpdate()
    {
        // 넉백 구현을 위해 Hit 에니메이션시 움직임 x
        if (!isLive || knockBack || nearestTarget == null || !GameManager.instance.isPlay)
            return;

        target = nearestTarget.GetComponent<Rigidbody2D>();

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }
    void LateUpdate()
    {
        _LateUpdate();
    }
    public virtual void _LateUpdate()
    {
        if (!isLive || nearestTarget == null || target == null || !GameManager.instance.isPlay)
            return;

        if (target.position.x > rigid.position.x)
        {
            //타겟 왼쪽에 있는 경우
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        }
    }
    public virtual void _OnEnable()
    {
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        isFire = false;
        fireDeBuffTime = 0;
        curHP = maxHP;

        InvokeRepeating("FindClosestObject", 0f, 0.1f);
    }
    public void SetManagedPool(IObjectPool<Enemy> pool)
    {
        _ManagedPool = pool;
    }
    public void DestroyEnemy()
    {
        if(effect!=null && effect.gameObject.activeSelf){
            effect.DestroyBuffEffect();
        }
        _ManagedPool.Release(this);
    }
    public virtual void Init(enemySpawnData data)
    {
        //spriteType에 따른 모습 변경
        //anim.runtimeAnimatorController = animCon[data.spriteType];
        character = data.enemyName;
        power = data.power;
        speed = data.speed;
        maxHP = data.health * power;
        curHP = maxHP;
        attackDamage = data.attackDamage * power;
        //spriter.sprite = data.sprite;

        CreateFollowingHpBar();
    }

    public virtual bool GetDamage(float _damage, float knockBackPower, bool _isCritical)
    {
        if (curHP <= 0)
            return false;

        //데미지 구현 구간
        if (_damage > 0)
            DamageManager.Instance.ShowDamageLabelOnObj((int)_damage, gameObject, _isCritical, false);

        curHP -= _damage;

        if (gameObject.activeSelf)
            StartCoroutine(KnockBack(knockBackPower));

        if (curHP > 0)
        {
            //anim.SetTrigger("Hit");
        }
        else
        {
            curHP = 0;
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            // spriter.sortingOrder = 1;
            //anim.SetBool("Dead",true);
            GameManager.instance.kill++;
            
            // 경험치 아이템 생성
            GameObject expItem = GameManager.instance.pool.Get(GameManager.instance.itemManager.itemDataList[0].itemPrefab);
            Vector2 randomPosition = Random.insideUnitCircle.normalized;
            expItem.transform.position = (Vector2)transform.position+randomPosition;
            expItem.SetActive(true);
            expItem.GetComponent<Item>().Init(GameManager.instance.itemManager.itemDataList[0]);

            // 일정 확률로 골드 아이템 생성
            int ran = Random.Range(1,101);
            if(ran <= 50){
                GameObject goldItem = GameManager.instance.pool.Get(GameManager.instance.itemManager.itemDataList[1].itemPrefab);
                Vector2 randomPositionGold = Random.insideUnitCircle.normalized;
                goldItem.transform.position = (Vector2)transform.position+randomPositionGold;
                goldItem.SetActive(true);
                goldItem.GetComponent<Item>().Init(GameManager.instance.itemManager.itemDataList[1]);
            }

            // 일정 확률로 인게임 아이템 생성
            
            
            //경험치 획득
            //GameManager.instance.GetExp();

            //에니메이션에 Dead를 넣는 대신 바로 호출
            Dead();
        }

        return true;
    }

    IEnumerator KnockBack(float knockBackPower)
    {

        knockBack = false;
        yield return wait;
        knockBack = true;
        if(nearestTarget != null)
        {
            Vector3 playerPos = nearestTarget.transform.position;
            Vector3 dirVec = transform.position - playerPos;
            rigid.AddForce(dirVec.normalized * knockBackPower, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.1f);
            knockBack = false;
        }
    }

    public void Dead()
    {
        //gameObject.SetActive(false);
        StopCoroutine("WarriorFireOn");
        CancelInvoke("FindClosestObject");
        DestroyEnemy();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player") == false)
            return;

        if (isAttackable == false)
            return;

        Attack(collision.transform.GetComponent<Player>());
    }
    void Attack(Player _player)
    {
        _player.GetDamage(DamageManager.Instance.Critical(GetComponent<CharacterStatus>(), attackDamage,out bool isCritical), isCritical);
        isAttackable = false;

        if (gameObject.activeSelf)
        {
            StartCoroutine(MyCoroutines.CoDelayStarter(() =>
            {
                isAttackable = true;
            },
            0
            ));
        }
    }
    public IEnumerator WarriorFireOn(float _damage, float _debuffTime)
    {
        isFire = true;
        effect = GameManager.instance.pool.buffPool.Get();
        effect.transform.parent = transform;
        effect.transform.position = transform.position;

        fireTime = 0;
        FireInit(_damage, _debuffTime);
        while (fireDeBuffTime > 0)
        {
            fireDeBuffTime -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        fireDeBuffTime = 0;
        effect.DestroyBuffEffect();
        isFire = false;
    }

    public void FireInit(float _damage, float _debuffTime)
    {
        curFireDamage = _damage;
        fireDeBuffTime = _debuffTime;
    }
}
