using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Spine.Unity;

public class GiantEnt : Enemy
{
    [Header("보스 정보")]
    public float missileDamage;
    public GameObject bullet;
    bool isAttack = false;
    bool isCheck = false;

    public enum BossState { Check, MoveToPlayer, NormalFire, Rest }
    [Header("보스 패턴 정보")]
    public BossState bossState;
    public float bulletSpeed;
    public enum AnimationState
    {
        Move,
        Skill
    }
    float timer;
    float skillDelay;
    private IObjectPool<GoblinBoss> _ManagedPool;
    [SerializeField]
    GameObject aimPrefab;
    private IObjectPool<TargetAnimation> aimPool;


    protected override void Awake()
    {
        base.Awake();
        
        skeletonAnimation = transform.GetChild(0).GetComponent<SkeletonAnimation>();
        StartCoroutine(BossStateMachine());
        SetAnimationState(AnimationState.Move);

        aimPool = new ObjectPool<TargetAnimation>(CreateAim, OnGetAim, OnReleaseAim, OnDestroyAim);
    }
    protected override void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

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
        base.LateUpdate();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    public override void Init(MonsterTable data, float power)
    {
        base.Init(data, power);
        missileDamage = attackDamage;
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
                // case BossState.Rest:
                //     yield return StartCoroutine(Rest());
                //     break;
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
            yield return bossState = timer >= skillDelay ? BossState.NormalFire: BossState.Check;
            
            isCheck = false;
        }
        else
        {
            yield return bossState = BossState.Check;
        }
    }
    GameObject aimObj;

    IEnumerator NormalFire()
    {
        if (nearestTarget != null)
        {
            SetAnimationState(AnimationState.Skill);
            // aim 시간이 다 되면 bullet 생성
            Vector2 targetPos = nearestTarget.transform.position;
            float totalTime = 3f;
            isAttack = true;
            // 가까운 적군위치에 aim 생성
            aimObj = aimPool.Get().gameObject;
            aimObj.transform.position = targetPos;
            TargetAnimation targetAnim = aimObj.GetComponent<TargetAnimation>();
            targetAnim.AttackTargetArea(targetPos, new Vector3(3.5f,2,0), totalTime);

            yield return new WaitForSeconds(3f);

            targetAnim.Done();


            // aim 시간이 다 되면 bullet 생성
            bullet.SetActive(true);
            bullet.transform.position = targetPos;
            EnemyBullet bulletLogic = bullet.GetComponent<EnemyBullet>();
            bulletLogic.duration = 1f;
            bulletLogic.Init(DamageManager.Instance.Critical(GetComponent<CharacterStatus>(), missileDamage, out bool isCritical), 5, isCritical);
            
            yield return new WaitForSeconds(0.8f);

            yield return bossState = BossState.Check;
            skillDelay = 3;
            timer = 0;
        }
        else
        {
            yield return bossState = BossState.Check;
            isAttack = false;
        }
    }
    // IEnumerator Rest()
    // {
    //     // 잠깐 가만히 서서 쉬는 타임
    //     yield return new WaitForSeconds(1f);
    //     bossState = BossState.Check;
    //     isAttack = false;
    // }
    void SetAnimationState(AnimationState _aniState)
    {
        if (skeletonAnimation == null || !isLive || !GameManager.instance.isPlay)
            return;

        if (_aniState == AnimationState.Move)
            skeletonAnimation.AnimationName = "move";
        else if (_aniState == AnimationState.Skill)
            skeletonAnimation.AnimationName = "skill";
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
    protected override void DestroyEnemy()
    {
        gameObject.SetActive(false);
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
