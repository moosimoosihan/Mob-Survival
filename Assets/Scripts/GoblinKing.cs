using System.Collections;
using UnityEngine;
using Spine.Unity;
using UnityEngine.Pool;

public class GoblinKing : Enemy
{
    [Header("보스 정보")]
    public float missileDamage;
    public GameObject bulletArea;
    public GameObject bullet;
    WaitForFixedUpdate wait;
    public float skillDelay = 20;
    public float timer;

    bool isAttack = false;
    bool isCheck = false;
    bool bossPowerUp;

    public enum BossState { Check, MoveToPlayer, Fire1, Fire2, Fire3, Rest }
    [Header("보스 패턴 정보")]
    public BossState bossState;
    public enum AnimationState
    {
        Move,
        Skill1,
        Skill2,
        Skill3
    }
    private IObjectPool<GoblinKing> _ManagedPool;
    public override void _Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();


        radius = (coll as CapsuleCollider2D).size.x * transform.localScale.x / 2;
        skeletonAnimation = transform.GetChild(0).GetComponent<SkeletonAnimation>();
        bulletArea = transform.GetChild(1).gameObject;
        bullet = transform.GetChild(2).gameObject;
        skillDelay = 20;
        StartCoroutine(BossStateMachine());
    }
    public override void _FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        // 넉백 구현을 위해 Hit 에니메이션시 움직임 x ( 공격 혹은 기모을동안 움직임 제한 )
        if (!isLive || knockBack || nearestTarget == null || isAttack)
        {
            rigid.velocity = Vector2.zero;
            return;
        }

        target = nearestTarget.GetComponent<Rigidbody2D>();

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }
    public override void _LateUpdate()
    {
        if (!isLive || nearestTarget == null || target == null || isAttack)
            return;

        if (target.position.x > rigid.position.x)
        {
            //타겟 왼쪽에 있는 경우
            transform.localScale = new Vector3(-2, 2, 2);
        }
        else
        {
            transform.localScale = new Vector3(2, 2, 2);
        }
    }
    public override void _OnEnable()
    {
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2;
        curHP = maxHP;
    }
    public override void Init(MonsterTable data, float power)
    {
        speed = data.Speed;
        maxHP = data.HP;
        curHP = maxHP;
        attackDamage = data.Attack;
        missileDamage = data.Attack / 2;

    }

    IEnumerator BossStateMachine()
    {
        while (gameObject.activeSelf || !isCheck || !isAttack)
        {
            switch (bossState)
            {
                case BossState.Check:
                    yield return StartCoroutine(Check());
                    break;
                case BossState.MoveToPlayer:
                    yield return StartCoroutine(MoveToPlayer());
                    break;
                case BossState.Fire1:
                    yield return StartCoroutine(Fire1());
                    break;
                case BossState.Fire2:
                    yield return StartCoroutine(Fire2());
                    break;
                case BossState.Fire3:
                    yield return StartCoroutine(Fire3());
                    break;
                case BossState.Rest:
                    yield return StartCoroutine(Rest());
                    break;
            }
        }
    }
    IEnumerator Check()
    {
        isCheck = true;
        if (nearestTarget != null)
        {
            // 적군이 있을 경우
            isCheck = false;
            yield return bossState = BossState.MoveToPlayer;
        }
        else
        {
            // 없을경우 1초마다 check 한다.
            yield return new WaitForSeconds(1);
            isCheck = false;
        }
    }
    IEnumerator MoveToPlayer()
    {
        if (nearestTarget)
        {
            while (Vector2.Distance(transform.position, nearestTarget.transform.position) > 10f)
            {
                if (nearestTarget != null)
                {
                    yield return bossState = BossState.Check;
                    isAttack = false;
                    break;
                }
                else
                {
                    isAttack = false;
                    isCheck = true;
                    yield return null;
                }
            }
            if (timer > skillDelay)
            {
                yield return bossState = BossState.Fire2;
            }
            else
            {
                yield return bossState = bossPowerUp ? BossState.Fire3 : BossState.Fire1;
            }
        }
        else
        {
            yield return bossState = BossState.Check;
        }
    }
    IEnumerator Fire1()
    {
        if (nearestTarget != null)
        {
            // 플레이어 쪽으로 내려 찍기
            isCheck = false;
            yield return null;

            SetAnimationState(AnimationState.Skill1);
            // 범위 표시
            bulletArea.gameObject.SetActive(true);

            yield return new WaitForSeconds(2.45f);
            // 발사
            bulletArea.gameObject.SetActive(false);
            BulletFire();

            yield return new WaitForSeconds(2.683f);
            bossState = BossState.Rest;
            SetAnimationState(AnimationState.Move);
        }
        else
        {
            yield return bossState = BossState.Check;
        }
    }
    IEnumerator Fire2()
    {
        if (nearestTarget != null)
        {
            // 아군 버프
            isAttack = true;
            timer = 0;
            isCheck = false;
            yield return null;

            SetAnimationState(AnimationState.Skill2);
            yield return new WaitForSeconds(2.3f);
            // 버프
            EnemyBuff();
            yield return new WaitForSeconds(0.8f);
            bossState = BossState.Rest;
            SetAnimationState(AnimationState.Move);
        }
        else
        {
            yield return bossState = BossState.Check;
            isAttack = false;
        }
    }
    IEnumerator Fire3()
    {
        if (nearestTarget != null)
        {
            // 플레이어 쪽으로 세번 내려 찍기
            isCheck = false;
            yield return null;

            SetAnimationState(AnimationState.Skill3);
            // 범위 표시
            bulletArea.gameObject.SetActive(true);

            yield return new WaitForSeconds(2.45f);
            // 발사
            bulletArea.gameObject.SetActive(false);
            StartCoroutine(BossPowerFire());

            yield return new WaitForSeconds(2.683f);
            bossState = BossState.Rest;
            SetAnimationState(AnimationState.Move);
        }
        else
        {
            yield return bossState = BossState.Check;
        }
    }

    IEnumerator Rest()
    {
        // 잠깐 가만히 서서 쉬는 타임
        yield return new WaitForSeconds(2f);
        bossState = BossState.Check;
        isAttack = false;
    }
    void SetAnimationState(AnimationState _aniState)
    {
        if (skeletonAnimation == null || !isLive || !GameManager.instance.isPlay)
            return;

        if (_aniState == AnimationState.Move)
            skeletonAnimation.AnimationName = "move";
        else if (_aniState == AnimationState.Skill1)
            skeletonAnimation.AnimationName = "skill1";
        else if (_aniState == AnimationState.Skill2)
            skeletonAnimation.AnimationName = "skill2";
        else if (_aniState == AnimationState.Skill3)
            skeletonAnimation.AnimationName = "skill3";
    }
    private void OnDisable()
    {
        if (bullet != null && bullet.activeSelf)
        {
            bullet.SetActive(false);
        }
        if (bulletArea != null && bulletArea.activeSelf)
        {
            bulletArea.SetActive(false);
        }
    }
    void BulletFire()
    {
        float time = 0.5f;
        bullet.gameObject.SetActive(true);
        EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
        bulletScript.duration = time;
        bulletScript.Init(DamageManager.Instance.Critical(GetComponent<CharacterStatus>(), missileDamage, out bool isCritical), 100, isCritical);
    }
    IEnumerator BossPowerFire()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.2f);
            float time = 0.2f;
            bullet.gameObject.SetActive(true);
            EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
            bulletScript.duration = time;
            bulletScript.Init(DamageManager.Instance.Critical(GetComponent<CharacterStatus>(), missileDamage, out bool isCritical), 100, isCritical);
            yield return new WaitForSeconds(time);
            bullet.gameObject.SetActive(false);
        }
    }
    void EnemyBuff()
    {

    }
    public void SetManagedPool(IObjectPool<GoblinKing> pool)
    {
        _ManagedPool = pool;
    }
    private void Dead()
    {
        //gameObject.SetActive(false);
        StopCoroutine("WarriorFireOn");
        CancelInvoke("FindClosestObject");
        DestroyEnemy();
    }
    private void DestroyEnemy()
    {
        _ManagedPool.Release(this);
    }
}
