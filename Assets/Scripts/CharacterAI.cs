using UnityEngine;

public class CharacterAI : MonoBehaviour
{
    public enum State
    {
        Init,
        Idle,
        Move,
        Patrol,
        Stop,
        Dead
    }

    Rigidbody2D rigid;
    Scaner scanner;

    [SerializeField]
    float speed;
    float curSpeed;

    //??Ｄ?╞?╞? ╱身?? ０?足??＝芋Ｙ
    public GameObject mainCharacter;

    [SerializeField]
    float distWithinMainCharacter;  //╱身?? ?昆╱?﹉??? ﹉?╱Ｙ ????
    [SerializeField]
    float distAwayFromEnemy;    //╱?卦足??０??? ﹉?╱Ｙ ????
    [SerializeField]
    float playerRadius;         //??﹞易??取? ??Ｄ???╞? 易?﹉屆

    [SerializeField]
    Vector3 dir;
    [SerializeField]
    Vector2 moveVec;

    [SerializeField]
    float sensorDist;

    [SerializeField]
    float dirShiftPower;

    Collider2D selfColl;

    Transform childTransform;

    Player playerScript;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        scanner = GetComponent<Scaner>();
        selfColl = GetComponent<Collider2D>();
        childTransform = transform.GetChild(0).GetComponent<Transform>();
        playerScript = GetComponent<Player>();

        playerRadius = (selfColl as CapsuleCollider2D).size.x * transform.localScale.x / 2;
        speed = playerScript.speed;
    }

    bool isRunning = true;
    void FixedUpdate ()
    {
        if (playerScript.playerDead)
            return;

        if (isRunning && mainCharacter != null && !GetComponentsInParent<Player>()[0].inputEnabled)
        {
            UpdateDirection();

            if (isMoving == false)
            {
                curSpeed = Mathf.Lerp(curSpeed, 0, Time.fixedDeltaTime * 8);
                playerScript.SetAnimationState(Player.AnimationState.Idle);
            }
            else
            {
                curSpeed = Mathf.Lerp(curSpeed, speed, Time.fixedDeltaTime * 8);
                playerScript.SetAnimationState(Player.AnimationState.Run);
            }

            moveVec = dir.normalized * curSpeed * Time.fixedDeltaTime;
            if (moveVec.x < 0)
            {
                childTransform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                childTransform.localScale = new Vector3(-1, 1, 1);
            }
            gameObject.GetComponent<Player>().StopToWall(moveVec);
            rigid.MovePosition(gameObject.GetComponent<Player>().isBorder? rigid.position : rigid.position + moveVec);
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, EulerAngleVector(dir.normalized, 0) * sensorDist, Color.green);
        //Debug.DrawRay(transform.position, EulerAngleVector(dir.normalized, +45) * detectObstacleRadius, Color.white);
        //Debug.DrawRay(transform.position, EulerAngleVector(dir.normalized, -45) * detectObstacleRadius, Color.white);
    }


    //易???╱? ０帚??╞? ﹉??? ╱╱?〝 ▽?０?０? ??╞? ??卹?
    Vector3 EulerAngleVector(Vector3 _dir, float _angle)
    {
        float tempAddRadian = _angle * Mathf.Deg2Rad;
        float tempRadian = Mathf.Atan2(_dir.y, _dir.x) + tempAddRadian;
        Vector3 result = new Vector3(Mathf.Cos(tempRadian), Mathf.Sin(tempRadian), 0);
        return result;
    }


    //???０ 易屆?? ??０?﹞? 卹取卹〝 2﹉昆取０ 足??帚?赤卹〝 ╞??╱╱? 易?╞? 易屆???╱﹞? ?Ｄ▽?取０ 芋昌▽?
    void UpdateObstacleSensors()
    {
        Vector3 tempStartPos;

        int tempRand = Random.Range(1, 10001);
        if (tempRand <= 5000)
        {
            //???帚 卹取卹〝1 ０╳ ??取?易﹉ 易帕﹉帕??╱?
            Vector3 leftSensorDir = EulerAngleVector(dir, -20);
            tempStartPos = transform.position + leftSensorDir.normalized * playerRadius * 1.25f;
            RaycastHit2D[] hit2ds_Left = Physics2D.RaycastAll(tempStartPos, leftSensorDir, sensorDist * 0.75f);
            Debug.DrawRay(tempStartPos, leftSensorDir * sensorDist * 0.75f, Color.red);

            if (CheckSensorColliders(hit2ds_Left))
            {
                dir = Vector3.Lerp(dir, EulerAngleVector(dir, 40f), Time.fixedDeltaTime * dirShiftPower);
                //Debug.Log("???帚1?卦??");
            }

            //???帚 卹取卹〝2 ０╳ ??取?易﹉ 易帕﹉帕??╱?
            leftSensorDir = EulerAngleVector(dir, -60);
            tempStartPos = transform.position + leftSensorDir.normalized * playerRadius * 1.25f;
            hit2ds_Left = Physics2D.RaycastAll(tempStartPos, leftSensorDir, sensorDist * 0.5f);
            Debug.DrawRay(tempStartPos, leftSensorDir * sensorDist * 0.5f, Color.red);

            if (CheckSensorColliders(hit2ds_Left))
            {
                dir = Vector3.Lerp(dir, EulerAngleVector(dir, 80f), Time.fixedDeltaTime * dirShiftPower);
                //Debug.Log("???帚2?卦??");
            }

            //０??帚 卹取卹〝1 ０╳ ??取?易﹉ 易帕﹉帕??╱?
            Vector3 rightSensorDir = EulerAngleVector(dir, 20);
            tempStartPos = transform.position + rightSensorDir.normalized * playerRadius * 1.25f;
            RaycastHit2D[] hit2ds_Right = Physics2D.RaycastAll(tempStartPos, rightSensorDir, sensorDist * 0.75f);
            Debug.DrawRay(tempStartPos, rightSensorDir * sensorDist * 0.75f, Color.red);

            if (CheckSensorColliders(hit2ds_Right))
            {
                dir = Vector3.Lerp(dir, EulerAngleVector(dir, -40f), Time.fixedDeltaTime * dirShiftPower);
                //Debug.Log("０??帚1 ?卦??");
            }

            //０??帚 卹取卹〝2 ０╳ ??取?易﹉ 易帕﹉帕??╱?
            rightSensorDir = EulerAngleVector(dir, 60);
            tempStartPos = transform.position + rightSensorDir.normalized * playerRadius * 1.25f;
            hit2ds_Right = Physics2D.RaycastAll(tempStartPos, rightSensorDir, sensorDist * 0.5f);
            Debug.DrawRay(tempStartPos, rightSensorDir * sensorDist * 0.5f, Color.red);

            if (CheckSensorColliders(hit2ds_Right)) 
            {
                dir = Vector3.Lerp(dir, EulerAngleVector(dir, -80f), Time.fixedDeltaTime * dirShiftPower);
                //Debug.Log("０??帚2 ?卦??");
            }
        }
        else
        {
            //０??帚 卹取卹〝1 ０╳ ??取?易﹉ 易帕﹉帕??╱?
            Vector3 rightSensorDir = EulerAngleVector(dir, 20);
            tempStartPos = transform.position + rightSensorDir.normalized * playerRadius * 1.25f;
            RaycastHit2D[] hit2ds_Right = Physics2D.RaycastAll(tempStartPos, rightSensorDir, sensorDist * 0.75f);
            Debug.DrawRay(tempStartPos, rightSensorDir * sensorDist * 0.75f, Color.red);

            if (CheckSensorColliders(hit2ds_Right))
            {
                dir = Vector3.Lerp(dir, EulerAngleVector(dir, -40f), Time.fixedDeltaTime * dirShiftPower);
                //Debug.Log("０??帚1 ?卦??");
            }

            //０??帚 卹取卹〝2 ０╳ ??取?易﹉ 易帕﹉帕??╱?
            rightSensorDir = EulerAngleVector(dir, 60);
            tempStartPos = transform.position + rightSensorDir.normalized * playerRadius * 1.25f;
            hit2ds_Right = Physics2D.RaycastAll(tempStartPos, rightSensorDir, sensorDist * 0.5f);
            Debug.DrawRay(tempStartPos, rightSensorDir * sensorDist * 0.5f, Color.red);

            if (CheckSensorColliders(hit2ds_Right))
            {
                dir = Vector3.Lerp(dir, EulerAngleVector(dir, -80f), Time.fixedDeltaTime * dirShiftPower);
                //Debug.Log("０??帚2 ?卦??");
            }

            //???帚 卹取卹〝1 ０╳ ??取?易﹉ 易帕﹉帕??╱?
            Vector3 leftSensorDir = EulerAngleVector(dir, -20);
            tempStartPos = transform.position + leftSensorDir.normalized * playerRadius * 1.25f;
            RaycastHit2D[] hit2ds_Left = Physics2D.RaycastAll(tempStartPos, leftSensorDir, sensorDist * 0.75f);
            Debug.DrawRay(tempStartPos, leftSensorDir * sensorDist * 0.75f, Color.red);

            if (CheckSensorColliders(hit2ds_Left))
            {
                dir = Vector3.Lerp(dir, EulerAngleVector(dir, 40f), Time.fixedDeltaTime * dirShiftPower);
                //Debug.Log("???帚1?卦??");
            }

            //???帚 卹取卹〝2 ０╳ ??取?易﹉ 易帕﹉帕??╱?
            leftSensorDir = EulerAngleVector(dir, -60);
            tempStartPos = transform.position + leftSensorDir.normalized * playerRadius * 1.25f;
            hit2ds_Left = Physics2D.RaycastAll(tempStartPos, leftSensorDir, sensorDist * 0.5f);
            Debug.DrawRay(tempStartPos, leftSensorDir * sensorDist * 0.5f, Color.red);

            if (CheckSensorColliders(hit2ds_Left))
            {
                dir = Vector3.Lerp(dir, EulerAngleVector(dir, 80f), Time.fixedDeltaTime * dirShiftPower);
                //Debug.Log("???帚2?卦??");
            }

        }
                
    }
    bool CheckSensorColliders(RaycastHit2D[] _hit2ds)
    {
        int collisionCount = 0;
        for (int i = 0; i < _hit2ds.Length; i++)
        {
            // ??取?   //昆見?帕０? ????取?  ??﹞易??取? ??取? ▽╱足芍?赤取帕????
            if (_hit2ds[i].collider.CompareTag("Bullet"))
                continue;

            // ??卦?
            if (_hit2ds[i].collider == selfColl)
                continue;

            //??昆見Ｄ??? ?屆?易卦?
            collisionCount++;
            break;
        }

        if(collisionCount >= 1)
            return true;
        else
            return false;
    }


    //易屆?? 取㊣????芋Ｙ
    //1. 芋芍芋Ｙ﹞? / ╱赤????▽?
    //2. ?? ?i▽? / ?身﹉??卦???╳▽?
    //3. ╱身???昆╱??? ?i▽?
    Enemy scannedEnemy;
    float patrolTime = 2;
    float patrolTimer = 0;
    bool isMoving = false;
    bool isChasingEnemy = false;

    void UpdateDirection()
    {
        //易??╱??▽?
        patrolTimer += Time.deltaTime;
        if(patrolTimer >= patrolTime)
        {
            patrolTimer = 0;
            patrolTime = Random.Range(1.75f, 2.5f);
            int tempRand = Random.Range(1, 10001);
            if (tempRand <= 5000)
            {
                Vector2 randDir = new Vector2(
                    mainCharacter.transform.position.x + Random.Range(-distWithinMainCharacter, distWithinMainCharacter),
                    mainCharacter.transform.position.y + Random.Range(-distWithinMainCharacter, distWithinMainCharacter));
                dir = Vector3.Lerp(dir, randDir, Time.fixedDeltaTime * dirShiftPower);
                isMoving = true;
                isChasingEnemy = false;
            }
            else
            {
                //dir = Vector3.Lerp(dir, new Vector3(0, 0, 0), Time.deltaTime * dirShiftPower);
                isMoving = false;
                isChasingEnemy = false;
            }

            //Debug.Log("易??╱");
        }
       

        //╱?卦足??﹉╳ ?昆╱??? 易?﹉屆昆?０╳ ??取?０? ﹉屆０?
        if (scanner.nearestTarget != null)
        {
            scannedEnemy = scanner.nearestTarget.GetComponent<Enemy>();

            //???? ??易? ▽??昆０╳ ???╱╱? 卦足?取
            if (Vector3.Distance(transform.position, scanner.nearestTarget.position) - scannedEnemy.radius - playerRadius > distAwayFromEnemy &&
                Vector3.Distance(transform.position, scanner.nearestTarget.position) - scannedEnemy.radius - playerRadius < distAwayFromEnemy)
            {
                isMoving = false;
                isChasingEnemy = true;
                //Debug.Log("╱赤??");
            }

            //╱身???昆╱???０??? 易?﹉屆 昆?０╳卹〝 ╱?卦足??╱? ?卦???? ﹉屆０? ?i▽?
            if (Vector3.Distance(transform.position, scanner.nearestTarget.position) - scannedEnemy.radius - playerRadius > distAwayFromEnemy &&
                Vector3.Distance(scanner.nearestTarget.position, mainCharacter.transform.position) - scannedEnemy.radius <= distWithinMainCharacter)
            {
                Vector2 chaseDir = scannedEnemy.transform.position - transform.position;

                dir = Vector3.Lerp(dir, chaseDir, Time.fixedDeltaTime * dirShiftPower);
                isMoving = true;
                isChasingEnemy = true;
                //Debug.Log("╱?卦足?? ?i▽?");
            }
                        
            //╱?卦足??﹉╳ ﹉╳▽??? ０??╱╱? ?身﹉??卦??
            if (Vector3.Distance(transform.position, scanner.nearestTarget.position) - scannedEnemy.radius - playerRadius < distAwayFromEnemy)
            {
                Vector2 backDir = transform.position - scannedEnemy.transform.position;

                dir = Vector3.Lerp(dir, backDir, Time.fixedDeltaTime * dirShiftPower * 5);
                isMoving = true;
                isChasingEnemy = true;
                //Debug.Log("５卹５卹");
            }
        }


        //╱身?? ?昆╱?﹉? ╱?取???╱? ??Ｄ?﹉╳▽?
        if (Vector3.Distance(transform.position, mainCharacter.transform.position) > distWithinMainCharacter)
        {
            Vector2 followDir = mainCharacter.transform.position - transform.position;

            dir = Vector3.Lerp(dir, followDir, Time.fixedDeltaTime * dirShiftPower);
            isMoving = true;
            isChasingEnemy = false;
            //Debug.Log("足易▽?");
        }

        if (isMoving)
        {
            if(isChasingEnemy == false)
                UpdateObstacleSensors();
        }
    }
}
