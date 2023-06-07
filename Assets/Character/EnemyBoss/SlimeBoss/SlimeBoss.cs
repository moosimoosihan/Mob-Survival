using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SlimeBoss : Enemy
{
    [SerializeField]
    GameObject aimPrefab;

    [SerializeField]
    AnimationCurve moveCurve;

    [SerializeField]
    AnimationCurve scaleCurve_X;
    [SerializeField]
    AnimationCurve scaleCurve_Y;

    [SerializeField]
    AnimationCurve scaleCurve_X_Afterwards;
    [SerializeField]
    AnimationCurve scaleCurve_Y_Afterwards;

    //[SerializeField]
    //Transform shadowTransform;

    [SerializeField]
    float attackRadius;
    [SerializeField]
    float detectionAngle;

    float timer = 0;

    [SerializeField]
    bool drawRay = false;
    private void OnDrawGizmosSelected()
    {
        if (drawRay)
        {
            Debug.DrawRay(transform.GetChild(0).transform.position, EulerAngleToVector(detectionAngle * 0.5f) * attackRadius);
            Debug.DrawRay(transform.GetChild(0).transform.position, EulerAngleToVector(-detectionAngle * 0.5f) * attackRadius);
        }
    }

    Vector3 EulerAngleToVector(float _angle)
    {
        float tempAngle = _angle + transform.eulerAngles.y;
        float tempRadian = tempAngle * Mathf.Deg2Rad;
        Vector3 result = new Vector3(Mathf.Sin(tempRadian), Mathf.Cos(tempRadian), 0);
        return result;
    }

    public override void _LateUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= attackDelay)
        {
            if (isReady)
            {
                if (scaner.nearestTarget != null)
                {
                    Vector3 targetPos = scaner.nearestTarget.position;
                    JumpAttack(new Vector3(targetPos.x, targetPos.y));
                    timer = 0;
                }
            }
        }
        
        if (!isLive || scaner.nearestTarget == null || target == null || !GameManager.instance.isPlay)
            return;

        if (target.position.x > rigid.position.x)
        {
            //타겟 왼쪽에 있는 경우
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    bool isReady = true;
    TargetAnimation targetAnim;
    public void JumpAttack(Vector3 _targetPos)
    {
        if (isReady == false)
            return;

        isReady = false;
        StartCoroutine(CoJumpAttack(_targetPos));
    }

    GameObject aimObj;
    IEnumerator CoJumpAttack(Vector3 _targetPos)
    {
        float timer = 0;
        Vector3 childTransformStartPos = transform.position;

        float xDistance = Mathf.Abs(_targetPos.x - childTransformStartPos.x);
        float totalTime = xDistance / 10.0f;

        if (totalTime >= 1.1f)
            totalTime = 1.1f;

        if (totalTime <= 0.6f)
            totalTime = 0.6f;

        aimObj = GameManager.instance.pool.Get(aimPrefab);        
        targetAnim = aimObj.GetComponent<TargetAnimation>();
        targetAnim.AttackTargetArea(_targetPos, aimObj.transform.localScale, totalTime);
        yield return new WaitForSeconds(totalTime / 2.0f);

        //점프높이 타겟과의 y값
        float jumpY = xDistance * 0.5f;
        if (jumpY < 0.5f)
            jumpY += 0.5f;

        //시작점이랑 타겟점 높이 비교해서 기준점 정해주기
        float jumpHeight = 0;
        if(_targetPos.y > childTransformStartPos.y)
        {
            jumpHeight = _targetPos.y + jumpY;
        }
        else
        {
            jumpHeight = childTransformStartPos.y + jumpY;
        }

        float posX = childTransformStartPos.x;
        float posY = childTransformStartPos.y;

        float scaleX = 1;
        float scaleY = 1;

        //점프전에 콜라이더 꺼주기
        //capsuleCollider2D.enabled = false;

        while (true)
        {
            timer += Time.deltaTime;

            //위치
            posX = Mathf.Lerp(childTransformStartPos.x, _targetPos.x, moveCurve.Evaluate(timer / totalTime));

            if(timer < totalTime / 2.0f)
            {
                float t = timer / (totalTime / 2.0f);
                posY = Mathf.Lerp(childTransformStartPos.y, jumpHeight, moveCurve.Evaluate(t));
            }
            else
            {
                float t = (timer - (totalTime / 2.0f)) / (totalTime / 2.0f);
                posY = Mathf.Lerp(jumpHeight, _targetPos.y, moveCurve.Evaluate(t));
            }

            //몸통 이동
            transform.position = new Vector3(posX, posY, transform.position.z);
                      
            //사이즈 꿀렁이기
            scaleX = scaleCurve_X.Evaluate(timer / totalTime);
            scaleY = scaleCurve_Y.Evaluate(timer / totalTime);
            transform.localScale = new Vector3(scaleX, scaleY, transform.localScale.z);

            if (timer >= totalTime)
                break;

            yield return null;
        }        

        targetAnim.Done();

        //점프후 콜라이더 켜주기
        //capsuleCollider2D.enabled = true;

        //공격
        AttackArea();

        //착지하고나서 꿀렁이기
        timer = 0;
        totalTime = 0.35f;
        while (true)
        {
            timer += Time.deltaTime;

            //사이즈 꿀렁이기
            scaleX = scaleCurve_X_Afterwards.Evaluate(timer / totalTime);
            scaleY = scaleCurve_Y_Afterwards.Evaluate(timer / totalTime);
            transform.localScale = new Vector3(scaleX, scaleY, transform.localScale.z);

            if (timer >= totalTime)
                break;

            yield return null;
        }

        isReady = true;
    }

    protected List<Player> Getplayers()
    {
        List<Player> result = new List<Player>();

        //원 안의 오브젝트들을 가져오기 위해 physics 사용
        Collider2D[] colliders2D = Physics2D.OverlapCircleAll(transform.position, attackRadius, LayerMask.GetMask("Player"));

        if (colliders2D.Length > 0)
        {
            foreach (var targetCol in colliders2D)
            {
                Vector3 direction = targetCol.transform.position - transform.position;
                float angle = Vector3.Angle(transform.up, direction);

                // ↖↑↗ 부채꼴 각도 안에 있는 애들만 가져오기   
                if (angle <= detectionAngle * 0.5f)
                {
                    //0 ~ 180도 -0 - 180
                    Player playerScript = targetCol.GetComponent<Player>();
                    if (playerScript != null)
                        result.Add(playerScript);
                }
            }
        }

        return result;
    }

    bool AttackArea()
    {
        List<Player> tempPlayerList = Getplayers();
        if (tempPlayerList.Count <= 0)
            return false;

        for (int i = 0; i < tempPlayerList.Count; i++)
        {
            //Debug.Log($"데미지 [{damage}]");
            tempPlayerList[i].GetDamage(attackDamage, false);
        }

        return true;
    }

    private void OnDisable()
    {
        if (aimObj != null && aimObj.activeSelf)
        {
            aimObj.SetActive(false);
        }
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
}
