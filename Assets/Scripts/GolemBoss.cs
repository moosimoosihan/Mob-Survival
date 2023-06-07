using System.Collections;
using UnityEngine;
using Spine.Unity;

public class GolemBoss : Enemy
{
   [Header("보스 정보")]
    public float missileDamage;
    public GameObject bulletArea;
    public GameObject bullet;
    WaitForFixedUpdate wait;
    public float skillDelay;
    public float timer;

    bool isAttack = false;
    bool isCheck = false;

    public enum BossState{ Check, MoveToPlayer, Fire, Rest }
    [Header("보스 패턴 정보")]
    public BossState bossState;
    public enum AnimationState
    {
        Move,
        Skill
    }
    SkeletonAnimation skeletonAnimation;
    public override void _Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();
        scaner = GetComponent<Scaner>();

        
        radius = (coll as CapsuleCollider2D).size.x * transform.localScale.x / 2;
        skeletonAnimation = transform.GetChild(0).GetComponent<SkeletonAnimation>();
        bulletArea = transform.GetChild(1).gameObject;
        bullet = transform.GetChild(2).gameObject;
        skillDelay = 10;
        StartCoroutine(BossStateMachine());
    }
    public override void _FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        // 넉백 구현을 위해 Hit 에니메이션시 움직임 x ( 공격 혹은 기모을동안 움직임 제한 )
        if (!isLive || knockBack || scaner.nearestTarget == null || isAttack){
            rigid.velocity = Vector2.zero;
            return;
        } 

        target = scaner.nearestTarget.GetComponent<Rigidbody2D>();

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }
    public override void _LateUpdate()
    {
        if (!isLive || scaner.nearestTarget == null || target == null || isAttack)
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
    }
    public override bool GetDamage(float _damage,float knockBackPower, bool _isCritical)
    {
        if (curHP <= 0)
            return false;

        //데미지 구현 구간
        if (_damage > 0)
            DamageManager.Instance.ShowDamageLabelOnObj((int)_damage, gameObject, _isCritical, false);

        curHP -= _damage;

        // 보스 넉백 x?
        // if (gameObject.activeSelf)
        //     StartCoroutine(KnockBack());

        if (curHP > 0)
        {
            // 히트 모션? 히트 색상?
        }
        else
        {
            curHP = 0;
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            
            GameManager.instance.bossKill++;
            
            // 경험치 아이템 생성
            GameObject expItem = GameManager.instance.pool.Get(GameManager.instance.itemManager.itemDataList[0].itemPrefab);
            Vector2 randomPosition = Random.insideUnitCircle.normalized;
            expItem.transform.position = (Vector2)transform.position+randomPosition;
            expItem.SetActive(true);
            expItem.GetComponent<Item>().Init(GameManager.instance.itemManager.itemDataList[0]);

            // 보스의 경우 아웃게임 아이템 100프로 드랍 현재는 골드 100프로
            int ran = Random.Range(1,101);
            if(ran <= 100){
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
                case BossState.Fire:
                    yield return StartCoroutine(Fire());
                    break;
                case BossState.Rest:
                    yield return StartCoroutine(Rest());
                    break;
            }
        }
    }
    IEnumerator Check(){
        isCheck = true;
        if(scaner.nearestTarget!=null){
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
        while(Vector2.Distance(transform.position, scaner.nearestTarget.transform.position) > 10f){
            if(scaner.nearestTarget != null){
                yield return bossState = BossState.Check;
                isAttack = false;
                break;
            } else {
                isAttack = false;
                isCheck = true;
                yield return null;
            }
        }
        yield return bossState = timer >= skillDelay ? BossState.Fire : BossState.Check;
    }
    IEnumerator Fire(){
        if(scaner.nearestTarget != null){
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
    private void OnDisable()
    {
        if (bullet != null && bullet.activeSelf){
            bullet.SetActive(false);
        }
        if(bulletArea != null && bulletArea.activeSelf){
            bulletArea.SetActive(false);
        }
    }
    IEnumerator AreaOn(){
        // x 스캐일 0~30까지 3.5초 동안 커지기
        float time = 3.3f;
        bulletArea.gameObject.SetActive(true);
        for(int i=0;i<60;i++){
            yield return new WaitForSeconds(time/60);
            bulletArea.transform.localScale = new Vector2 (i/2,8f);
        }
        bulletArea.transform.localScale = new Vector2 (0,8f);
        bulletArea.gameObject.SetActive(false);
    }
    void BulletFire(){
        // 26까지가 최대
        float time = 0.5f;
        bool left = false;
        bullet.gameObject.SetActive(true);
        
        EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
        bulletScript.duration = time;
        bulletScript.Init(DamageManager.Instance.Critical(GetComponent<CharacterStatus>(), missileDamage, out bool isCritical), 100, isCritical);
        if(target.position.x<transform.position.x)
            left = true;
                        
        bullet.transform.position = new Vector2(left? transform.position.x - 4 : transform.position.x + 4, transform.position.y + 4);
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce((left? Vector2.left : Vector2.right) * 50, ForceMode2D.Impulse);
    }
}
