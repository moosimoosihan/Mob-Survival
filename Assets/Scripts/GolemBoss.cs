using System.Collections;
using UnityEngine;
using Spine.Unity;
using UnityEngine.Pool;

public class GolemBoss : Enemy
{
    [Header("보스 정보")]
    public float missileDamage;
    public GameObject bulletArea;
    public GameObject bulletAreaBack;
    public GameObject bullet;
    WaitForFixedUpdate wait;
    public float skillDelay;
    public float timer;

    bool isAttack = false;
    bool isCheck = false;

    public enum BossState { Check, MoveToPlayer, Fire, Rest }
    [Header("보스 패턴 정보")]
    public BossState bossState;
    public enum AnimationState
    {
        Move,
        Skill
    }

    private IObjectPool<GolemBoss> _ManagedPool;
    protected override void Awake()
    {
        base.Awake();

        skeletonAnimation = transform.GetChild(0).GetComponent<SkeletonAnimation>();
        bulletArea = transform.GetChild(1).gameObject;
        bullet = transform.GetChild(2).gameObject;
        bulletAreaBack = transform.GetChild(3).gameObject;
        skillDelay = 10;
        StartCoroutine(BossStateMachine());
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
        missileDamage = CurAttackDamage;
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
                case BossState.Fire:
                    yield return StartCoroutine(Fire());
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
            yield return bossState = timer >= skillDelay ? BossState.Fire : BossState.Check;
        }
        else
        {
            yield return bossState = BossState.Check;
        }
    }
    IEnumerator Fire()
    {
        if (nearestTarget != null)
        {
            // 플레이어 쪽으로 내려 찍기
            isAttack = true;
            timer = 0;
            isCheck = false;
            yield return null;

            SetAnimationState(AnimationState.Skill);
            // 범위 표시
            StartCoroutine(AreaOn());

            yield return new WaitForSeconds(3.3f);
            // 발사
            bulletArea.gameObject.SetActive(false);
            BulletFire();

            yield return new WaitForSeconds(1.033f);
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
            skeletonAnimation.AnimationName = "move";
        else if (_aniState == AnimationState.Skill)
            skeletonAnimation.AnimationName = "skill";
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
    IEnumerator AreaOn()
    {
        // x 스캐일 0~30까지 3.5초 동안 커지기
        float time = 3.3f;
        bulletAreaBack.gameObject.SetActive(true);
        bulletArea.gameObject.SetActive(true);
        StartCoroutine(MyCoroutines.CoChangeSizeX(bulletArea, 0, 30, time));
        yield return new WaitForSeconds(time);
        bulletAreaBack.gameObject.SetActive(false);
        bulletArea.gameObject.SetActive(false);
    }
    void BulletFire()
    {
        // 26까지가 최대
        float time = 0.5f;
        bool left = false;
        bullet.gameObject.SetActive(true);

        EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
        bulletScript.duration = time;
        bulletScript.Init(DamageManager.Instance.Critical(GetComponent<CharacterStatus>(), missileDamage, out bool isCritical), 100, isCritical, true);
        if (target.position.x < transform.position.x)
            left = true;

        bullet.transform.position = new Vector2(left ? transform.position.x - 4 : transform.position.x + 4, transform.position.y + 4);
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce((left ? Vector2.left : Vector2.right) * 50, ForceMode2D.Impulse);
    }
    public void SetManagedPool(IObjectPool<GolemBoss> pool)
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
}
