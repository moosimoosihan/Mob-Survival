using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Spine.Unity;

public class Basilisk : Enemy
{
    [Header("보스 정보")]
    public float missileDamage;
    public float bulletSpeed;
    public GameObject poisonBullet;
    bool isAttack = false;
    bool isCheck = false;
    bool bossPowerUp;

    public enum BossState { Check, MoveToPlayer, PoisonFire, NormalFire, SpecialFire, Rest }
    [Header("보스 패턴 정보")]
    public BossState bossState;
    public enum AnimationState { Move, Skill1, Skill2, Skill3_1, Skill3_2 }
    float normalTimer;
    float poisonTimer;
    float specialTimer;
    float normalSkillDelay = 10;
    float poisonSkillDelay = 20;
    float specialSkillDelay = 30;
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
        missileDamage = CurAttackDamage;

        aimPool = new ObjectPool<TargetAnimation>(CreateAim, OnGetAim, OnReleaseAim, OnDestroyAim);
    }
    protected override void FixedUpdate()
    {
        normalTimer += Time.fixedDeltaTime;
        poisonTimer += Time.fixedDeltaTime;
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
                case BossState.PoisonFire:
                    yield return StartCoroutine(PoisonFire());
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
            yield return bossState = (bossPowerUp && specialTimer >= specialSkillDelay) ? BossState.SpecialFire : poisonTimer >= poisonSkillDelay ? BossState.PoisonFire : normalTimer >= normalSkillDelay ? BossState.NormalFire : BossState.Check;

            isCheck = false;
        }
        else
        {
            yield return bossState = BossState.Check;
        }
    }
    IEnumerator PoisonFire()
    {
        if (nearestTarget != null)
        {
            SetAnimationState(AnimationState.Skill1);
            Vector2 dirVec = nearestTarget.transform.position - transform.position;
            isAttack = true;
            //2초 후 독 뿌리기
            yield return new WaitForSeconds(2f);
            //발사
            poisonBullet.transform.position = transform.position;
            if (dirVec.x < 0)
                poisonBullet.transform.rotation = Quaternion.FromToRotation(Vector3.left, dirVec);
            else
                poisonBullet.transform.rotation = Quaternion.FromToRotation(Vector3.right, dirVec);

            poisonBullet.GetComponent<EnemyBullet>().duration = 0.5f;
            poisonBullet.SetActive(true);
            poisonBullet.GetComponent<EnemyBullet>().Init(DamageManager.Instance.Critical(GetComponent<CharacterStatus>(), missileDamage, out bool isCritical), GameManager.instance.players.Length + 1, isCritical, true);
            poisonBullet.GetComponent<Rigidbody2D>().AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);
            yield return new WaitForSeconds(1.2f);
            poisonTimer = 0;
            bossState = BossState.Rest;
            SetAnimationState(AnimationState.Move);
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
            SetAnimationState(AnimationState.Skill2);
            // 1초 후 범위공격
            // 쿨 10초
            yield return new WaitForSeconds(1f);
            //발사
            (coll as CapsuleCollider2D).size = new Vector2(15f, 5f);
            yield return new WaitForSeconds(0.5f);
            (coll as CapsuleCollider2D).size = new Vector2(4.5f, 4.5f);
            yield return new WaitForSeconds(0.5f);
            normalTimer = 0;
            bossState = BossState.Rest;
            SetAnimationState(AnimationState.Move);
        }
        else
        {
            yield return bossState = BossState.Check;
        }
    }
    GameObject aimObj;
    IEnumerator SpecialFire()
    {
        if (nearestTarget != null)
        {
            isAttack = true;
            Vector2 dirVec = nearestTarget.transform.position;
            SetAnimationState(AnimationState.Skill3_1);
            // 3초 후 나타나면서 공격
            (coll as CapsuleCollider2D).enabled = false;
            yield return new WaitForSeconds(3.9f);
            skeletonAnimation.timeScale = 0;

            // 잠시 없어짐
            aimObj = aimPool.Get().gameObject;
            aimObj.transform.position = dirVec;
            TargetAnimation aim = aimObj.GetComponent<TargetAnimation>();
            aim.AttackTargetArea(dirVec, new Vector3(2, 2.5f, 0), 3);
            float time = 2;
            while (time > 0)
            {
                time -= 0.1f;
                aim.transform.position = nearestTarget.transform.position;
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(1f);
            aim.Done();

            //발사
            transform.position = aim.transform.position;
            SetAnimationState(AnimationState.Skill3_2);
            skeletonAnimation.timeScale = 1;
            (coll as CapsuleCollider2D).enabled = true;
            yield return new WaitForSeconds(0.5f);
            SetAnimationState(AnimationState.Move);
            specialTimer = 0;
            bossState = BossState.Rest;
        }
        else
        {
            yield return bossState = BossState.Check;
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
            skeletonAnimation.AnimationName = "move";
        else if (_aniState == AnimationState.Skill1)
            skeletonAnimation.AnimationName = "skill1";
        else if (_aniState == AnimationState.Skill2)
            skeletonAnimation.AnimationName = "skill2";
        else if (_aniState == AnimationState.Skill3_1)
            skeletonAnimation.AnimationName = "skill3_1";
        else if (_aniState == AnimationState.Skill3_2)
            skeletonAnimation.AnimationName = "skill3_2";
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
    protected override void BossPowerUp()
    {
        if (CurHP <= MaxHP / 2 && !bossPowerUp)
        {
            bossPowerUp = true;
        }
    }
}