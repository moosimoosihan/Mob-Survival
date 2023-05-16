using Spine.Unity;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : CharacterStatus
{
    // 자신의 캐릭터
    public enum characterInfo { 용사, 궁수, 현자, 사제 }
    public characterInfo chacter;

    [Header("플레이어 컨트롤 설정")]
    public Vector2 inputVec;
    Rigidbody2D rigid;
    public bool isBorder;

    [Header("이미지 혹은 에니메이션 설정")]
    SpriteRenderer spriter;
    public Animator anim;
    public RuntimeAnimatorController[] animCharacter;

    [Header("플레이어 상태 설정")]
    public bool playerDead = false;
    // 무속성, 불, 물, 흙, 바람, 전기, 정신, 빛, 어둠 등 상태 추가 예정
    public bool isDamaged = false;
    public float damageDelay = 0.3f;

    [Header("플레이어 공격 설정")]
    bool isAttack;
    bool isAttackReady;
    public float attackDeley;
    public Scaner scanner;

    [SerializeField]
    public bool inputEnabled;
    new Collider2D collider2D;
    Transform childTransform;
    Transform weaponTransform;
    [Header("플레이어 스탯 설정")]
    public int def;
    public float heal;
    public float hpRegen;
    public float evasion;

    public enum AnimationState
    {
        Idle,
        Run,
        Death
    }

    SkeletonAnimation skeletonAnimation;

    void Awake()
    {
        //초기화
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        scanner = GetComponent<Scaner>();
        collider2D = GetComponent<Collider2D>();

        
        childTransform = transform.GetChild(0).GetComponent<Transform>();
        weaponTransform = transform.GetChild(1).GetComponent<Transform>();
        skeletonAnimation = transform.GetChild(0).GetChild(0).GetComponent<SkeletonAnimation>();

        //사운드
        //audioSource = GetComponent<AudioSource>(); 
        
        CreateFollowingHpBar();
    }
    public void Init(playerData data){
        maxHP = data.maxHP;
        curHP = maxHP;
        def = data.def;
        speed = data.moveSpeed;
        hpRegen = data.hpRegen;
        evasion = data.evasion;
        heal = data.heal;
        critRate = data.critRate;
        critDamage = data.critDamage;
    }
    void FixedUpdate()
    {
        if (playerDead || !GameManager.instance.isPlay)
            return;

        if(inputEnabled)
        {
            StopToWall(inputVec);
            Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
            rigid.MovePosition(isBorder? rigid.position : rigid.position + nextVec);

            if (nextVec != Vector2.zero)
            {
                SetAnimationState(AnimationState.Run);
            }
            else
            {
                SetAnimationState(AnimationState.Idle);
            }
        }
    }
    public void StopToWall(Vector2 _inputVec)
    {
        Debug.DrawRay(transform.position, _inputVec * 2, Color.green);
        isBorder = Physics2D.Raycast(transform.position, _inputVec, 1, LayerMask.GetMask("Wall"));
    }
    void OnMove(InputValue value)
    {
        if (playerDead || !GameManager.instance.isPlay)
            return;

        if (inputEnabled)
            inputVec = value.Get<Vector2>();
    }
    void LateUpdate()
    {
        if (playerDead || !GameManager.instance.isPlay)
            return;

        if (inputVec.x != 0){
            if (inputVec.x < 0){
                childTransform.localScale = new Vector3(1, 1, 1);
            } else {
                childTransform.localScale = new Vector3(-1, 1, 1);
            }
        }
        //spriter.flipX = inputVec.x > 0;
    }

    public void GetDamage(float _damage, bool _isCritical)
    {
        if (playerDead || !GameManager.instance.isPlay || isDamaged)
            return;

        double dam = 0;
        if(_damage>0){
            dam = _damage/(1+def*0.01);
            // 회피
            float ran = Random.Range(0,100);
            if(evasion*100 > ran){
                //회피 성공
                DamageManager.Instance.ShowMessageLabelOnObj(DamageLabel.Message.Miss, gameObject);
                StartCoroutine(DamageDelay());
                return;
            }
        } else {
            dam = _damage;
        }
        
        curHP -= System.Convert.ToSingle(dam);
        DamageManager.Instance.ShowDamageLabelOnObj((int)dam, gameObject, _isCritical);
        StartCoroutine(DamageDelay());

        if (curHP < 0)
        {
            curHP = 0;
            //죽음
            SetAnimationState(AnimationState.Death);
            gameObject.SetActive(false);    //애니메이션 없으면 그냥 사라지게
            //if(ret == false)
        }
    }

    public void SetAnimationState(AnimationState _aniState)
    {
        if (skeletonAnimation == null || playerDead || !GameManager.instance.isPlay)
           return;

        if (_aniState == AnimationState.Idle)
           skeletonAnimation.AnimationName = "idle";
        else if (_aniState == AnimationState.Run)
           skeletonAnimation.AnimationName = "run";
        else if (_aniState == AnimationState.Death)
        {
           skeletonAnimation.AnimationName = "death_roop";
           playerDead = true;
           gameObject.layer = LayerMask.NameToLayer("Default");
           weaponTransform.gameObject.SetActive(false);
           collider2D.enabled = false;
        }
    }
    IEnumerator DamageDelay(){
        isDamaged = true;
        yield return new WaitForSeconds(damageDelay);
        isDamaged = false;
    }
}
