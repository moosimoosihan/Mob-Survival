using Spine.Unity;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class GoblinBoss : Enemy
{
    [Header("보스 정보")]
    public float missileDamage;
    public GameObject bullet;
    WaitForFixedUpdate wait;

    bool isAttack = false;
    bool isCheck = false;

    public enum BossState{ Check, MoveToPlayer, NormalFire, CircleFire, Rest }
    [Header("보스 패턴 정보")]
    public BossState bossState;
    public int bossPatternLen = 2;
    public int[] patternNum = { 5, 5 }; // 패턴 반복 할 횟수
    public float bulletSpeed;
    public enum AnimationState
    {
        Move,
        Skill
    }
    float timer;
    float skillDelay;
    private IObjectPool<GoblinBoss> _ManagedPool;
    private IObjectPool<Bullet> bulletPool;

    public override void _Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();

        bullet = transform.GetChild(1).gameObject;
        radius = (coll as CapsuleCollider2D).size.x * transform.localScale.x / 2;
        skeletonAnimation = transform.GetChild(0).GetComponent<SkeletonAnimation>();
        StartCoroutine(BossStateMachine());
        SetAnimationState(AnimationState.Move);
        bulletPool = new ObjectPool<Bullet>(CreateBullet, OnGetBullet, OnReleaseBullet, OnDestroyBullet);
    }
    public override void _FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        // 넉백 구현을 위해 Hit 에니메이션시 움직임 x ( 공격 혹은 기모을동안 움직임 제한 )
        if (!isLive || knockBack || nearestTarget == null || isAttack){
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
            transform.localScale = new Vector3(-1,1,1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
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
    public override void Init(enemySpawnData data)
    {
        speed = data.speed;
        maxHP = data.health;
        curHP = maxHP;
        attackDamage = data.attackDamage;
        missileDamage = data.attackDamage/2;
        bulletSpeed = 3;
    }

    IEnumerator BossStateMachine()
    {
        while(gameObject.activeSelf || !isCheck || !isAttack){
            switch(bossState){
                case BossState.Check:
                    yield return StartCoroutine(Check());
                    break;
                case BossState.MoveToPlayer:
                    yield return StartCoroutine(MoveToPlayer());
                    break;
                case BossState.NormalFire:
                    yield return StartCoroutine(NormalFire());
                    break;
                case BossState.CircleFire:
                    yield return StartCoroutine(CircleFire());
                    break;
                case BossState.Rest:
                    yield return StartCoroutine(Rest());
                    break;
            }
        }
    }
    IEnumerator Check(){
        isCheck = true;
        if(nearestTarget!=null){
            // 적군이 있을 경우
            isCheck = false;
            yield return bossState = BossState.MoveToPlayer;
        } else {
            // 없을경우 1초마다 check 한다.
            yield return new WaitForSeconds(1);
            isCheck = false;
        }
    }
    IEnumerator MoveToPlayer(){
        if(nearestTarget){
            while(Vector2.Distance(transform.position, nearestTarget.transform.position) > 15f){
                if(nearestTarget != null){
                    yield return bossState = BossState.Check;
                    isAttack = false;
                    break;
                } else {
                    isAttack = false;
                    isCheck = true;
                    yield return null;
                }
            }
            int ran = Random.Range(0,bossPatternLen);
            yield return bossState = timer >= skillDelay ? (ran == 0 ? BossState.NormalFire : BossState.CircleFire) : BossState.Check;
            timer = 0;
            isCheck = false;
        } else {
            yield return bossState = BossState.Check;
        }
    }
    IEnumerator NormalFire(){
        if(nearestTarget != null){
            // 앞으로 하나씩 미사일 발사
            isAttack = true;
            yield return null;
            for(int i=0;i<patternNum[0];i++){
                yield return new WaitForSeconds(0.5f);
                Bullet _bullet = bulletPool.Get();
                EnemyBullet bulletLogic = _bullet.GetComponent<EnemyBullet>();
                bulletLogic.duration = 5f;
                bulletLogic.Init(DamageManager.Instance.Critical(GetComponent<CharacterStatus>(), missileDamage, out bool isCritical), 1, isCritical);
                _bullet.transform.position = transform.position;
                Rigidbody2D rigid = _bullet.GetComponent<Rigidbody2D>();
                Vector2 dirVec = nearestTarget.transform.position - transform.position;
                rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);
            }
            yield return bossState = BossState.Rest;
            skillDelay = 2;
        } else {
            yield return bossState = BossState.Check;
            isAttack = false;
        }
    }
    IEnumerator CircleFire(){
        if(nearestTarget != null){
            // 원형으로 미사일 발사
            isAttack = true;
            yield return null;
            // 기 모으기
            SetAnimationState(AnimationState.Skill);
            yield return new WaitForSeconds(4f);
            // 발사
            for(int i=0; i< patternNum[1];i++){
                yield return new WaitForSeconds(1f);
                int roundA = 10;
                int roundB = 9;
                int curRound = i % 2 == 0 ? roundA : roundB;
                for(int y=0;y<curRound;y++){
                    Bullet _bullet = bulletPool.Get();
                    EnemyBullet bulletLogic = _bullet.GetComponent<EnemyBullet>();
                    bulletLogic.Init(DamageManager.Instance.Critical(GetComponent<CharacterStatus>(), missileDamage, out bool isCritical), 1, isCritical);
                    _bullet.transform.position = transform.position;
                    bullet.transform.rotation = Quaternion.identity;

                    Rigidbody2D rigid = _bullet.GetComponent<Rigidbody2D>();
                    Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * y / curRound), Mathf.Sin(Mathf.PI * 2 * y / curRound));
                    rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);

                    Vector3 rotVec = Vector3.forward * 360 * y / curRound + Vector3.forward * 90;
                    bullet.transform.Rotate(rotVec);
                }
                yield return new WaitForSeconds(0.4f);
                bossState = BossState.Rest;
                SetAnimationState(AnimationState.Move);
            }
            skillDelay = 15;
        } else {
            yield return bossState = BossState.Check;
            isAttack = false;
        }
    }
    IEnumerator Rest(){
        // 잠깐 가만히 서서 쉬는 타임
        yield return new WaitForSeconds(1f);
        bossState = BossState.Check;
        isAttack = false;
    }
    void SetAnimationState(AnimationState _aniState){
        if(skeletonAnimation == null || !isLive || !GameManager.instance.isPlay)
            return;
        
        if(_aniState == AnimationState.Move)
            skeletonAnimation.AnimationName = "move";
        else if (_aniState == AnimationState.Skill)
            skeletonAnimation.AnimationName = "skill";
    }
    public void SetManagedPool(IObjectPool<GoblinBoss> pool)
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
    Bullet CreateBullet()
    {
        Bullet bullet = Instantiate(this.bullet).GetComponent<Bullet>();
        bullet.SetManagedPool(bulletPool);
        return bullet;
    }
    void OnGetBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(true);
    }
    void OnReleaseBullet(Bullet bullet)
    {
        if (bullet.gameObject.activeSelf)
            bullet.gameObject.SetActive(false);
    }
    void OnDestroyBullet(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }
}
