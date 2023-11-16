using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Spine.Unity;

public class TwinHeadOgre : Enemy
{
    [Header("보스 정보")]
    public float missileDamage;
    public float specialDamage;
    public GameObject enemyBullet;

    bool isAttack = false;
    bool isCheck = false;

    public enum BossState { Check, MoveToPlayer, NormalFire, SpecialFire, Rest }
    [Header("보스 패턴 정보")]
    public BossState bossState;
    public float bulletSpeed;
    public enum AnimationState
    {
        Move,
        Skill1,
        Skill2,
    }
    float normalTimer;
    float specialTimer;
    float normarSkillDelay = 10;
    float specialFireDelay = 10;
    private IObjectPool<GoblinBoss> _ManagedPool;
    private IObjectPool<EnemyBullet> bulletPool;
    private IObjectPool<TargetAnimation> aimPool;
    bool bossPowerUp;
    // float specialStopTime = 1f;
    [SerializeField]
    GameObject aimPrefab;
    [SerializeField]
    GameObject specialBulletPrefab;

    protected override void Awake()
    {
        base.Awake();

        skeletonAnimation = transform.GetChild(0).GetComponent<SkeletonAnimation>();
        StartCoroutine(BossStateMachine());
        SetAnimationState(AnimationState.Move);
        bulletPool = new ObjectPool<EnemyBullet>(CreateBullet, OnGetBullet, OnReleaseBullet, OnDestroyBullet);
        aimPool = new ObjectPool<TargetAnimation>(CreateAim, OnGetAim, OnReleaseAim, OnDestroyAim);
    }
    protected override void FixedUpdate()
    {
        normalTimer += Time.fixedDeltaTime;
        if (bossPowerUp)
            specialTimer += Time.fixedDeltaTime;

        // 넉백 구현을 위해 Hit 에니메이션시 움직임 x ( 공격 혹은 기모을동안 움직임 제한 )
        if (isAttack)
        {
            rigid.velocity = Vector2.zero;
            return;
        }
        base.FixedUpdate();
    }
    protected override void LateUpdate()
    {
        if (!isBoss)
            isBoss = true;
        
        base.LateUpdate();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    public override void Init(MonsterTable data, float power)
    {
        base.Init(data, power);
        missileDamage = CurAttackDamage;
        specialDamage = CurAttackDamage * 2;
        bulletSpeed = 15;
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
                case BossState.NormalFire:
                    yield return StartCoroutine(NormalFire());
                    break;
                case BossState.SpecialFire:
                    yield return StartCoroutine(SpecialFire());
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
            SetAnimationState(AnimationState.Move);
            while (Vector2.Distance(transform.position, nearestTarget.transform.position) > 15f)
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
            yield return bossState = normalTimer >= normarSkillDelay ? BossState.NormalFire : specialTimer >= specialFireDelay ? BossState.SpecialFire : BossState.Check;

            isCheck = false;
        }
        else
        {
            yield return bossState = BossState.Check;
        }
    }
    IEnumerator NormalFire()
    {
        if (nearestTarget != null)
        {
            // 플레이어에게 미사일 발사
            isAttack = true;
            yield return null;
            float delay = 2f;
            SetAnimationState(AnimationState.Skill1);
            yield return new WaitForSeconds(delay);
            Vector3 targetPos = nearestTarget.position;
            Vector3 dir = targetPos - transform.position;
            dir = dir.normalized;
            EnemyBullet _bullet = bulletPool.Get();
            _bullet.transform.position = transform.position;
            _bullet.transform.parent = GameManager.instance.pool.transform;
            EnemyBullet bulletLogic = _bullet.GetComponent<EnemyBullet>();
            bulletLogic.duration = 5f;
            bulletLogic.Init(DamageManager.Instance.Critical(GetComponent<CharacterStatus>(), missileDamage, out bool isCritical), GameManager.instance.players.Length, isCritical, true);
            _bullet.transform.position = transform.position;
            Rigidbody2D rigid = _bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = nearestTarget.transform.position - transform.position;
            rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);
            float attackDelay = bossPowerUp ? 0.25f : 0.5f;
            yield return new WaitForSeconds(attackDelay);
            yield return bossState = BossState.Rest;
            SetAnimationState(AnimationState.Move);
            normalTimer = 0;
            bossState = BossState.Rest;
            SetAnimationState(AnimationState.Move);
        }
        else
        {
            yield return bossState = BossState.Check;
            isAttack = false;
        }
    }
    IEnumerator SpecialFire()
    {
        if (nearestTarget != null)
        {
            // 기를 모은 후 고함을 질러 광역 공격
            isAttack = true;
            yield return null;

            // 기 모으기
            SetAnimationState(AnimationState.Skill2);
            GameObject aimObj;
            TargetAnimation targetAnim;

            aimObj = aimPool.Get().gameObject;
            targetAnim = aimObj.GetComponent<TargetAnimation>();
            targetAnim.AttackTargetArea(transform.position, new Vector3(6, 6, 1), 1);
            yield return new WaitForSeconds(1);
            targetAnim.Done();

            // 발사
            specialBulletPrefab.SetActive(true);
            specialBulletPrefab.GetComponent<EnemyBullet>().Init(DamageManager.Instance.Critical(GetComponent<CharacterStatus>(), specialDamage, out bool isCritical), GameManager.instance.players.Length, isCritical, true, false);
            yield return new WaitForSeconds(1);
            specialBulletPrefab.SetActive(false);
            specialTimer = 0;
            bossState = BossState.Rest;
            SetAnimationState(AnimationState.Move);
        }
        else
        {
            yield return bossState = BossState.Check;
            isAttack = false;
        }
    }
    IEnumerator Rest()
    {
        // 잠깐 가만히 서서 쉬는 타임
        yield return new WaitForSeconds(1f);
        bossState = BossState.Check;
        isAttack = false;
    }
    void SetAnimationState(AnimationState _aniState)
    {
        if (skeletonAnimation == null || !isLive || !GameManager.instance.isPlay)
            return;



        if (_aniState == AnimationState.Move)
        {
            skeletonAnimation.AnimationName = "move";
        }
        else if (_aniState == AnimationState.Skill1)
        {
            skeletonAnimation.AnimationName = "skill";
        }
        // else if (_aniState == AnimationState.Skill2){
        //     skeletonAnimation.AnimationName = "skill2";
        // }
    }
    public void SetManagedPool(IObjectPool<GoblinBoss> pool)
    {
        _ManagedPool = pool;
    }
    protected override void Dead()
    {
        base.Dead();
        StopAllCoroutines();
    }

    EnemyBullet CreateBullet()
    {
        EnemyBullet bullet = Instantiate(enemyBullet).GetComponent<EnemyBullet>();
        bullet.SetManagedPool(bulletPool);
        return bullet;
    }
    void OnGetBullet(EnemyBullet bullet)
    {
        bullet.gameObject.SetActive(true);
    }
    void OnReleaseBullet(EnemyBullet bullet)
    {
        if (bullet.gameObject.activeSelf)
            bullet.gameObject.SetActive(false);
    }
    void OnDestroyBullet(EnemyBullet bullet)
    {
        Destroy(bullet.gameObject);
    }
    protected override void DestroyEnemy()
    {
        gameObject.SetActive(false);
    }
    protected override void BossPowerUp()
    {
        if (CurHP <= MaxHP / 2 && !bossPowerUp)
        {
            bossPowerUp = true;
        }
    }
    TargetAnimation CreateAim()
    {
        TargetAnimation aimObj = Instantiate(aimPrefab).GetComponent<TargetAnimation>();
        aimObj.SetManagedPool(aimPool);
        return aimObj;
    }
    void OnGetAim(TargetAnimation aimObj)
    {
        aimObj.gameObject.SetActive(true);
    }
    void OnReleaseAim(TargetAnimation aimObj)
    {
        if (aimObj.gameObject.activeSelf)
            aimObj.gameObject.SetActive(false);
    }
    void OnDestroyAim(TargetAnimation aimObj)
    {
        Destroy(aimObj.gameObject);
    }
}
