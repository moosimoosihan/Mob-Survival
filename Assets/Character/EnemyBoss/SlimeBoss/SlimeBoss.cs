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

}
