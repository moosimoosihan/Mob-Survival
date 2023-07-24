using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Spine.Unity;

public class Medusa : Enemy
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
    float specialFireDelay = 60;
    private IObjectPool<GoblinBoss> _ManagedPool;
    private IObjectPool<EnemyBullet> bulletPool;
    public SpriteRenderer specialArea;
    bool bossPowerUp;
    float specialStopTime = 1f;

    protected override void Awake()
    {
        base.Awake();
        
        skeletonAnimation = transform.GetChild(0).GetComponent<SkeletonAnimation>();
        StartCoroutine(BossStateMachine());
        SetAnimationState(AnimationState.Move);
        bulletPool = new ObjectPool<EnemyBullet>(CreateBullet, OnGetBullet, OnReleaseBullet, OnDestroyBullet);
    }
    protected override void FixedUpdate()
    {
        //normalTimer += Time.fixedDeltaTime;
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
        missileDamage = attackDamage;
        specialDamage = attackDamage * 2;
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
            float delay = bossPowerUp? 1f : 2f;
            SetAnimationState(AnimationState.Skill2);
            yield return new WaitForSeconds(delay);
            Vector3 targetPos = nearestTarget.position;
            Vector3 dir = targetPos - transform.position;
            dir = dir.normalized;
            EnemyBullet _bullet = bulletPool.Get();
            _bullet.transform.position = transform.position;
            _bullet.transform.rotation = Quaternion.FromToRotation(Vector3.left, dir);
            _bullet.transform.parent = GameManager.instance.pool.transform;
            EnemyBullet bulletLogic = _bullet.GetComponent<EnemyBullet>();
            bulletLogic.duration = 5f;
            bulletLogic.Init(DamageManager.Instance.Critical(GetComponent<CharacterStatus>(), missileDamage, out bool isCritical), 1, isCritical);
            _bullet.transform.position = transform.position;
            Rigidbody2D rigid = _bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = nearestTarget.transform.position - transform.position;
            rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);
            float attackDelay = bossPowerUp? 0.25f : 0.5f;
            yield return new WaitForSeconds(attackDelay);
            yield return bossState = BossState.Rest;
            SetAnimationState(AnimationState.Move);
            normalTimer = 0;
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
            // 메두사를 보는 플레이어 모두 스턴
            isAttack = true;
            yield return null;
            // 기 모으기
            float delay = bossPowerUp? 2.5f : 5f;

            SetAnimationState(AnimationState.Skill2);
            StartCoroutine(MyCoroutines.CoFadeInOut(specialArea, 0, 0.5f, delay));
            yield return new WaitForSeconds(delay);
            // 발사
            float attackDelay = bossPowerUp? 0.25f : 0.5f;
            float elapsedTime = 0f;
            while (elapsedTime < 0.5f)
            {
                // 플레이어와 오브젝트가 서로 마주보고 있다면
                if(transform.localScale.x==1){
                    for(int i = 0;i<GameManager.instance.players.Length;i++){
                        if(GameManager.instance.players[i].childTransform.localScale.x==-1){
                            if(GameManager.instance.players[i].resistance!=0){
                                GameManager.instance.players[i].GetDamage(DamageManager.Instance.Critical(GetComponent<CharacterStatus>(), specialDamage, out bool isCritical), isCritical);
                                GameManager.instance.players[i].StartCoroutine(GameManager.instance.players[i].Speedresistance(0, specialStopTime));
                            }
                        }
                    }
                } else if(transform.localScale.x==-1){
                    for(int i = 0;i<GameManager.instance.players.Length;i++){
                        if(GameManager.instance.players[i].childTransform.localScale.x==1){
                            if(GameManager.instance.players[i].resistance!=0){
                                GameManager.instance.players[i].GetDamage(DamageManager.Instance.Critical(GetComponent<CharacterStatus>(), specialDamage, out bool isCritical), isCritical);
                                GameManager.instance.players[i].StartCoroutine(GameManager.instance.players[i].Speedresistance(0,specialStopTime));
                            }
                        }
                    }
                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            specialArea.color = new Color(1, 0, 0, 0);
            yield return new WaitForSeconds(attackDelay);
            bossState = BossState.Rest;
            SetAnimationState(AnimationState.Move);
            specialTimer = 0;
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



        if (_aniState == AnimationState.Move){
            if(skeletonAnimation.timeScale==2){
                skeletonAnimation.timeScale = 1;
            }
            skeletonAnimation.AnimationName = "move";
        }
        else if (_aniState == AnimationState.Skill1){
            if(bossPowerUp && skeletonAnimation.timeScale == 1){
                skeletonAnimation.timeScale = 2;
            }
            skeletonAnimation.AnimationName = "skill1";
        }
        else if (_aniState == AnimationState.Skill2){
            if(bossPowerUp && skeletonAnimation.timeScale == 1){
                skeletonAnimation.timeScale = 2;
            }
            skeletonAnimation.AnimationName = "skill2";
        }
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
    protected override void BossPowerUp(){
        if(curHP <= maxHP/2 && !bossPowerUp){
            bossPowerUp = true;
            normarSkillDelay = normarSkillDelay/2;
            specialFireDelay = specialFireDelay/2;
        }
    }
}
