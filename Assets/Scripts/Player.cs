using Spine.Unity;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : CharacterStatus
{
    // 자신의 캐릭터

    [Header("플레이어 컨트롤 설정")]
    public Vector2 inputVec;
    Rigidbody2D rigid;
    public bool isBorder;

    [Header("이미지 혹은 에니메이션 설정")]
    SpriteRenderer spriter;
    public Animator anim;

    [Header("플레이어 상태 설정")]
    public bool playerDead = false;
    // 무속성, 불, 물, 흙, 바람, 전기, 정신, 빛, 어둠 등 상태 추가 예정
    public bool isDamaged = false;
    float damageDelay = 1f;

    [Header("플레이어 공격 설정")]
    public Scaner scanner;

    [SerializeField]
    public bool inputEnabled;
    public new Collider2D collider2D;
    public Transform childTransform;
    public Transform weaponTransform;
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

    void Start()
    {
        //초기화
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        scanner = GetComponent<Scaner>();
        collider2D = GetComponent<Collider2D>();

        GameManager.instance.inputManager.GetAction("Move").Enable();
        GameManager.instance.inputManager.GetAction("Move").performed += OnMove;

        childTransform = transform.GetChild(0).GetComponent<Transform>();
        weaponTransform = transform.GetChild(1).GetComponent<Transform>();
        skeletonAnimation = transform.GetChild(0).GetChild(0).GetComponent<SkeletonAnimation>();

        //사운드
        //audioSource = GetComponent<AudioSource>(); 

        CreateFollowingHpBar();
    }

    private void OnDestroy()
    {
        GameManager.instance.inputManager.GetAction("Move").performed -= OnMove;
        GameManager.instance.inputManager.GetAction("Move").Disable();
    }
    public void Init(CharacterTable data)
    {
        character = data.Name;
        maxHP = data.HP;
        curHP = maxHP;
        def = data.Def;
        speed = data.MoveSpeed;
        hpRegen = data.HPRegen;
        evasion = data.Evasion;
        heal = data.Heal;
        critRate = data.CritRate;
        critDamage = data.CritDamage;
    }
    void FixedUpdate()
    {
        if (playerDead || !GameManager.instance.isPlay)
            return;

        if (inputEnabled)
        {
            // 현재 다이나믹으로 했을 경우 필요 없음
            //StopToWall(inputVec);
            Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime * resistance;
            rigid.MovePosition(isBorder ? rigid.position : rigid.position + nextVec);

            if (nextVec != Vector2.zero)
            {
                SetAnimationState(AnimationState.Run);
            }
            else
            {
                SetAnimationState(AnimationState.Idle);
            }
        }
        else
        {
            inputVec = Vector2.zero;
        }

        skeletonAnimation.timeScale = resistance;
    }
    public void StopToWall(Vector2 _inputVec)
    {
        Debug.DrawRay(transform.position, _inputVec * 2, Color.green);
        isBorder = Physics2D.Raycast(transform.position, _inputVec, 1, LayerMask.GetMask("Wall"));
    }


    private void OnMove(InputAction.CallbackContext obj)
    {
        if (playerDead || !GameManager.instance.isPlay)
            return;
            
        if(inputEnabled)
            inputVec = obj.ReadValue<Vector2>();
    }

    void LateUpdate()
    {
        if (playerDead || !GameManager.instance.isPlay)
            return;

        if (inputVec.x != 0)
        {
            if (inputVec.x < 0)
            {
                childTransform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
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
        if (_damage > 0)
        {
            isDamaged = true;
            StartCoroutine(DamageDelay());
            dam = _damage / (1 + def * 0.01);
            // 회피
            float ran = Random.Range(0, 100);
            if (evasion * 100 > ran)
            {
                //회피 성공
                DamageManager.Instance.ShowMessageLabelOnObj(DamageLabel.Message.Miss, gameObject);
                return;
            }

            //보호막이 있을 경우 보호막이 먼저 깎인다.
            if (curShield > 0)
            {
                if (curShield > _damage)
                {
                    curShield -= _damage;
                    DamageManager.Instance.ShowDamageLabelOnObj((int)_damage, gameObject, _isCritical, true);
                    return;
                }
                else if (curShield == _damage)
                {
                    DamageManager.Instance.ShowDamageLabelOnObj((int)_damage, gameObject, _isCritical, true);
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
                    return;
                }
                else
                {
                    float tempDamage = _damage - curShield;
                    DamageManager.Instance.ShowDamageLabelOnObj((int)curShield, gameObject, _isCritical, true);
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
        DamageManager.Instance.ShowDamageLabelOnObj((int)dam, gameObject, _isCritical, true);

        if (curHP <= 0)
        {
            curHP = 0;
            //죽음
            SetAnimationState(AnimationState.Death);

            int livePlayerCount = 0;
            for (int i = 0; i < GameManager.instance.players.Length; i++)
            {
                if (!GameManager.instance.players[i].playerDead)
                {
                    livePlayerCount++;
                }
            }

            if (livePlayerCount == 0)
            { // 전부 죽었을 경우
                GameManager.instance.isPlay = false;
                // 부활

                // 게임오버

                // 다시하기
            }

            if (inputEnabled && livePlayerCount > 0)
            {
                GameManager.instance.playerControl.PlayerDeadNextPlayer();
            }
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
            skeletonAnimation.AnimationName = "death_loop";
            playerDead = true;
            gameObject.layer = LayerMask.NameToLayer("Default");
            weaponTransform.gameObject.SetActive(false);
            collider2D.enabled = false;
        }
    }
    IEnumerator DamageDelay()
    {
        yield return new WaitForSeconds(damageDelay);
        isDamaged = false;
    }
    public void Revival()
    {
        curHP = maxHP;
        playerDead = false;
        collider2D.enabled = true;
        CreateFollowingHpBar();
        weaponTransform.gameObject.SetActive(true);
        gameObject.layer = 7;

    }
}
