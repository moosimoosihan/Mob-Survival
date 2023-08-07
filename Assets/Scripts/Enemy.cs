using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Spine.Unity;

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
    public Transform nearestTarget;
    private IObjectPool<Enemy> _ManagedPool;
    private IObjectPool<Item> itemPool;
    private IObjectPool<BuffEffect> poolBuffEffect;

    public SkeletonAnimation skeletonAnimation;
    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();
        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();

        radius = (coll as CapsuleCollider2D).size.x * transform.localScale.x / 2;
        itemPool = new ObjectPool<Item>(CreateItem, OnGetItem, OnReleaseItem, OnDestroyItem, maxSize : GameManager.instance.itemManager.itemPoolMaxSize);
        poolBuffEffect = new ObjectPool<BuffEffect>(CreateEffect, OnGetEffect, OnReleaseEffect, OnDestroyEffect);

        CreateFollowingHpBar();
    }

    protected virtual void Update()
    {
        if (!isLive || !GameManager.instance.isPlay)
            return;

        if (isFire)
        {
            fireTime += Time.deltaTime;
            if (fireTime >= 1)
            {
                fireTime = 0;
                GetDamage(curFireDamage, 0, false, true);
            }
        }
    }

    protected void FindClosestObject()
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

    protected virtual void FixedUpdate()
    {
         // 넉백 구현을 위해 Hit 에니메이션시 움직임 x
        if (!isLive || knockBack || nearestTarget == null || !GameManager.instance.isPlay)
            return;

        target = nearestTarget.GetComponent<Rigidbody2D>();

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;

        skeletonAnimation.timeScale = resistance;
    }

    protected virtual void LateUpdate()
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

    protected virtual void OnEnable()
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
    protected virtual void DestroyEnemy()
    {
        _ManagedPool.Release(this);
    }
    public virtual void Init(MonsterTable data, float powerValue)
    {
        //spriteType에 따른 모습 변경
        //anim.runtimeAnimatorController = animCon[data.spriteType];
        character = data.Name;
        power = powerValue;
        speed = data.Speed;
        maxHP = data.HP * power;
        curHP = maxHP;
        attackDamage = data.Attack * power;

        // 추가되어야 할 부분
        // def = data.Def;
        // evasion = data.Evasion;
        // heal = data.Heal;

        (coll as CapsuleCollider2D).size = new Vector2(1.8f,1.8f);
        (coll as CapsuleCollider2D).offset = Vector2.zero;
        //spriter.sprite = data.sprite;
        float scale = 1;
        Vector2 position = Vector2.zero;
        switch(data.Index){
            case 0:
                //슬라임
                scale = 0.45f;
                position = new Vector2(0,-0.9f);
            break;
            case 1:
                //빨간 슬라임
                scale = 0.5f;
                position = new Vector2(0,-0.9f);
            break;
            case 2:
                //파란 슬라임
                scale = 0.6f;
                position = new Vector2(0,-0.9f);
            break;
            case 3:
                //나무
                (coll as CapsuleCollider2D).offset = new Vector2(0.1f,0.2f);
                (coll as CapsuleCollider2D).size = new Vector2(1.5f,1.8f);
                scale = 1f;
                position = new Vector2(0,-0.9f);
            break;
            case 4:
                //뱀
                scale = 0.2342059f;
                position = new Vector2(0,-0.93f);
            break;
            case 5:
                //모기
                scale = 0.557689f;
                position = new Vector2(-0.12f,-0.7f);
            break;
            case 6:
                //고블린
                (coll as CapsuleCollider2D).offset = new Vector2(0,-0.1f);
                (coll as CapsuleCollider2D).size = new Vector2(1.5f,1.5f);
                scale = 0.1f;
                position = new Vector2(0,-0.9f);
            break;
            case 7:
                //독 모기
                scale = 0.557689f;
                position = new Vector2(-0.12f,-0.7f);
            break;
            case 8:
                //두꺼비
                scale = 0.5251f;
                position = new Vector2(-0.09f,-1.07f);
            break;
            case 9:
                // 슬라임 보스
                (coll as CapsuleCollider2D).offset = new Vector2(0.5f,0);
                (coll as CapsuleCollider2D).size = new Vector2(5,5);
                scale = 1.5f;
                position = new Vector2(0,-2.5f);
            break;
            case 10:
                // 골렘 보스
                (coll as CapsuleCollider2D).offset = new Vector2(0,3);
                (coll as CapsuleCollider2D).size = new Vector2(6,6);
                scale = 1f;
                position = new Vector2(0,0);
            break;
            case 11:
                // 고블린 메이지
                (coll as CapsuleCollider2D).size = new Vector2(4,4);
                scale = 1f;
                position = new Vector2(0,-2);
            break;
            case 12:
                // 고블린 킹
                (coll as CapsuleCollider2D).offset = new Vector2(0,0.5f);
                (coll as CapsuleCollider2D).size = new Vector2(3,3);
                scale = 1f;
                position = new Vector2(0,-1);
            break;
            case 13:
                // 코브라
                break;
            case 14:
                // 아머고블린
                break;
            case 15:
                // 돌연변이 슬라임
                break;
            case 16:
                // 자이언트 나무
                (coll as CapsuleCollider2D).offset = new Vector2(0,1.5f);
                (coll as CapsuleCollider2D).size = new Vector2(5.5f,7);
                scale = 2f;
                position = new Vector2(0,-2);
                break;
            case 17:
                // 바실리스크
                break;
            case 18:
                // 트윈헤드오우거
                (coll as CapsuleCollider2D).offset = new Vector2(0,0.5f);
                (coll as CapsuleCollider2D).size = new Vector2(2.5f,5.5f);
                scale = 2f;
                position = new Vector2(0,-2);
                break;
            case 19:
                // 메두사
                (coll as CapsuleCollider2D).size = new Vector2(3,5.5f);
                scale = 3f;
                position = new Vector2(0,-3);
                break;
        }
        skeletonAnimation.gameObject.transform.localPosition = position;
        skeletonAnimation.gameObject.transform.localScale = new Vector3(scale,scale,scale);

        CreateFollowingHpBar();
    }

    public virtual bool GetDamage(float _damage, float knockBackPower, bool _isCritical, bool trueDamage = false)
    {
        if (curHP <= 0 || !GameManager.instance.isPlay)
            return false;

        //데미지 구현 구간
        double dam = 0;
        if (_damage > 0 && !trueDamage)
        {
            dam = _damage / (1 + def * 0.01);
            // 회피
            float ran = Random.Range(0, 100);
            if (evasion * 100 > ran)
            {
                //회피 성공
                DamageManager.Instance.ShowMessageLabelOnObj(DamageLabel.Message.Miss, gameObject);
                return false;
            }

            //보호막이 있을 경우 보호막이 먼저 깎인다.
            if (curShield > 0)
            {
                if (curShield > _damage)
                {
                    curShield -= _damage;
                    DamageManager.Instance.ShowDamageLabelOnObj((int)_damage, gameObject, _isCritical, false);
                    return false;
                }
                else if (curShield == _damage)
                {
                    DamageManager.Instance.ShowDamageLabelOnObj((int)_damage, gameObject, _isCritical, false);
                    curShield = 0;
                    if (curShield <= 0)
                    {
                        if (isShield)
                        {
                            StopCoroutine(ShieldOn());
                            isShield = false;
                            shieldCurTime = 0;
                        }
                    }
                    return false;
                }
                else
                {
                    float tempDamage = _damage - curShield;
                    DamageManager.Instance.ShowDamageLabelOnObj((int)curShield, gameObject, _isCritical, false);
                    tempDamage = (float)(tempDamage / (1 + def * 0.01));
                    dam = tempDamage;
                    curShield = 0;
                    if (curShield <= 0)
                    {
                        if (isShield)
                        {
                            StopCoroutine(ShieldOn());
                            isShield = false;
                            shieldCurTime = 0;
                        }
                    }
                }
            }
        }
        else
        {
            // 회복의 경우
            dam = _damage * (1 + heal);
        }

        curHP -= System.Convert.ToSingle(dam);
        DamageManager.Instance.ShowDamageLabelOnObj((int)dam, gameObject, _isCritical, false);

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
            Item expItem = itemPool.Get();
            expItem.transform.parent = GameManager.instance.pool.transform;
            Vector2 randomPosition = Random.insideUnitCircle.normalized;
            expItem.transform.position = (Vector2)transform.position + randomPosition;
            expItem.GetComponent<Item>().Init(GameManager.instance.itemManager.itemDataList[0]);
            expItem.gameObject.SetActive(true);

            // 일정 확률로 골드 아이템 생성
            int ran = Random.Range(1, 101);
            if (ran <= 50)
            {
                Item goldItem = itemPool.Get();
                goldItem.transform.parent = GameManager.instance.pool.transform;
                Vector2 randomPositionGold = Random.insideUnitCircle.normalized;
                goldItem.transform.position = (Vector2)transform.position + randomPositionGold;
                goldItem.GetComponent<Item>().Init(GameManager.instance.itemManager.itemDataList[1]);
                goldItem.gameObject.SetActive(true);
            }

            // 일정 확률로 인게임 아이템 생성


            //경험치 획득
            //GameManager.instance.GetExp();

            //에니메이션에 Dead를 넣는 대신 바로 호출
            Dead();
        }
        BossPowerUp();
        return true;
    }
    protected virtual void BossPowerUp()
    {

    }
    IEnumerator KnockBack(float knockBackPower)
    {

        knockBack = false;
        yield return wait;
        knockBack = true;
        if (nearestTarget != null)
        {
            Vector3 playerPos = nearestTarget.transform.position;
            Vector3 dirVec = transform.position - playerPos;
            rigid.AddForce(dirVec.normalized * knockBackPower, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.1f);
            knockBack = false;
        }
    }

    protected virtual void Dead()
    {
        //gameObject.SetActive(false);
        StopCoroutine("WarriorFireOn");
        CancelInvoke("FindClosestObject");
        DestroyEnemy();
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player") == false)
            return;

        if (isAttackable == false)
            return;

        Attack(collision.transform.GetComponent<Player>());
    }
    void Attack(Player _player)
    {
        _player.GetDamage(DamageManager.Instance.Critical(GetComponent<CharacterStatus>(), attackDamage, out bool isCritical), isCritical);
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
        BuffEffect effect = poolBuffEffect.Get();
        effect.transform.parent = GameManager.instance.pool.transform;
        effect.transform.position = transform.position;
        effect.target = transform;

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
    Item CreateItem()
    {
        Item item = Instantiate(GameManager.instance.itemManager.itemPrefab).GetComponent<Item>();
        item.SetManagedPool(itemPool);
        return item;
    }
    void OnGetItem(Item item)
    {

    }
    void OnReleaseItem(Item item)
    {
        item.gameObject.SetActive(false);
    }
    void OnDestroyItem(Item item)
    {
        Destroy(item.gameObject);
    }
    BuffEffect CreateEffect()
    {
        BuffEffect buffEffect = Instantiate(GameManager.instance.burnEffect).GetComponent<BuffEffect>();
        buffEffect.SetManagedPool(poolBuffEffect);
        return buffEffect;
    }
    void OnGetEffect(BuffEffect buffEffect)
    {
        buffEffect.gameObject.SetActive(true);
    }
    void OnReleaseEffect(BuffEffect buffEffect)
    {
        if (buffEffect.gameObject.activeSelf)
            buffEffect.gameObject.SetActive(false);
    }
    void OnDestroyEffect(BuffEffect buffEffect)
    {
        Destroy(buffEffect.gameObject);
    }
}
