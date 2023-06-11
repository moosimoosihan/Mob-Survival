using UnityEngine;

public class CharacterAI : MonoBehaviour
{
    Rigidbody2D rigid;
    Scaner scanner;

    [SerializeField]
    float speed;
    float curSpeed;

    //따라다닐 메인 오브젝트
    public GameObject mainCharacter;

    [SerializeField]
    float distWithinMainCharacter;  //메인 캐릭과의 거리 유지
    [SerializeField]
    float distAwayFromEnemy;    //몬스터와의 거리 유지
    [SerializeField]
    float playerRadius;         //플레이어 콜라이더 반경

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


    //백터를 원하는 각도 만큼 기울여 주는 함수
    Vector3 EulerAngleVector(Vector3 _dir, float _angle)
    {
        float tempAddRadian = _angle * Mathf.Deg2Rad;
        float tempRadian = Mathf.Atan2(_dir.y, _dir.x) + tempAddRadian;
        Vector3 result = new Vector3(Mathf.Cos(tempRadian), Mathf.Sin(tempRadian), 0);
        return result;
    }


    //이동 방향 좌우로 센서 2개씩 부착해서 닿으면 반대 방향으로 조금씩 틀기
    void UpdateObstacleSensors()
    {
        Vector3 tempStartPos;

        int tempRand = Random.Range(1, 10001);
        if (tempRand <= 5000)
        {
            //좌측 센서1 에 장애물 발견하면
            Vector3 leftSensorDir = EulerAngleVector(dir, -20);
            tempStartPos = transform.position + leftSensorDir.normalized * playerRadius * 1.25f;
            RaycastHit2D[] hit2ds_Left = Physics2D.RaycastAll(tempStartPos, leftSensorDir, sensorDist * 0.75f);
            Debug.DrawRay(tempStartPos, leftSensorDir * sensorDist * 0.75f, Color.red);

            if (CheckSensorColliders(hit2ds_Left))
            {
                dir = Vector3.Lerp(dir, EulerAngleVector(dir, 40f), Time.fixedDeltaTime * dirShiftPower);
                //Debug.Log("좌측1탐지");
            }

            //좌측 센서2 에 장애물 발견하면
            leftSensorDir = EulerAngleVector(dir, -60);
            tempStartPos = transform.position + leftSensorDir.normalized * playerRadius * 1.25f;
            hit2ds_Left = Physics2D.RaycastAll(tempStartPos, leftSensorDir, sensorDist * 0.5f);
            Debug.DrawRay(tempStartPos, leftSensorDir * sensorDist * 0.5f, Color.red);

            if (CheckSensorColliders(hit2ds_Left))
            {
                dir = Vector3.Lerp(dir, EulerAngleVector(dir, 80f), Time.fixedDeltaTime * dirShiftPower);
                //Debug.Log("좌측2탐지");
            }

            //우측 센서1 에 장애물 발견하면
            Vector3 rightSensorDir = EulerAngleVector(dir, 20);
            tempStartPos = transform.position + rightSensorDir.normalized * playerRadius * 1.25f;
            RaycastHit2D[] hit2ds_Right = Physics2D.RaycastAll(tempStartPos, rightSensorDir, sensorDist * 0.75f);
            Debug.DrawRay(tempStartPos, rightSensorDir * sensorDist * 0.75f, Color.red);

            if (CheckSensorColliders(hit2ds_Right))
            {
                dir = Vector3.Lerp(dir, EulerAngleVector(dir, -40f), Time.fixedDeltaTime * dirShiftPower);
                //Debug.Log("우측1 탐지");
            }

            //우측 센서2 에 장애물 발견하면
            rightSensorDir = EulerAngleVector(dir, 60);
            tempStartPos = transform.position + rightSensorDir.normalized * playerRadius * 1.25f;
            hit2ds_Right = Physics2D.RaycastAll(tempStartPos, rightSensorDir, sensorDist * 0.5f);
            Debug.DrawRay(tempStartPos, rightSensorDir * sensorDist * 0.5f, Color.red);

            if (CheckSensorColliders(hit2ds_Right)) 
            {
                dir = Vector3.Lerp(dir, EulerAngleVector(dir, -80f), Time.fixedDeltaTime * dirShiftPower);
                //Debug.Log("우측2 탐지");
            }
        }
        else
        {
            //우측 센서1 에 장애물 발견하면
            Vector3 rightSensorDir = EulerAngleVector(dir, 20);
            tempStartPos = transform.position + rightSensorDir.normalized * playerRadius * 1.25f;
            RaycastHit2D[] hit2ds_Right = Physics2D.RaycastAll(tempStartPos, rightSensorDir, sensorDist * 0.75f);
            Debug.DrawRay(tempStartPos, rightSensorDir * sensorDist * 0.75f, Color.red);

            if (CheckSensorColliders(hit2ds_Right))
            {
                dir = Vector3.Lerp(dir, EulerAngleVector(dir, -40f), Time.fixedDeltaTime * dirShiftPower);
                //Debug.Log("우측1 탐지");
            }

            //우측 센서2 에 장애물 발견하면
            rightSensorDir = EulerAngleVector(dir, 60);
            tempStartPos = transform.position + rightSensorDir.normalized * playerRadius * 1.25f;
            hit2ds_Right = Physics2D.RaycastAll(tempStartPos, rightSensorDir, sensorDist * 0.5f);
            Debug.DrawRay(tempStartPos, rightSensorDir * sensorDist * 0.5f, Color.red);

            if (CheckSensorColliders(hit2ds_Right))
            {
                dir = Vector3.Lerp(dir, EulerAngleVector(dir, -80f), Time.fixedDeltaTime * dirShiftPower);
                //Debug.Log("우측2 탐지");
            }

            //좌측 센서1 에 장애물 발견하면
            Vector3 leftSensorDir = EulerAngleVector(dir, -20);
            tempStartPos = transform.position + leftSensorDir.normalized * playerRadius * 1.25f;
            RaycastHit2D[] hit2ds_Left = Physics2D.RaycastAll(tempStartPos, leftSensorDir, sensorDist * 0.75f);
            Debug.DrawRay(tempStartPos, leftSensorDir * sensorDist * 0.75f, Color.red);

            if (CheckSensorColliders(hit2ds_Left))
            {
                dir = Vector3.Lerp(dir, EulerAngleVector(dir, 40f), Time.fixedDeltaTime * dirShiftPower);
                //Debug.Log("좌측1탐지");
            }

            //좌측 센서2 에 장애물 발견하면
            leftSensorDir = EulerAngleVector(dir, -60);
            tempStartPos = transform.position + leftSensorDir.normalized * playerRadius * 1.25f;
            hit2ds_Left = Physics2D.RaycastAll(tempStartPos, leftSensorDir, sensorDist * 0.5f);
            Debug.DrawRay(tempStartPos, leftSensorDir * sensorDist * 0.5f, Color.red);

            if (CheckSensorColliders(hit2ds_Left))
            {
                dir = Vector3.Lerp(dir, EulerAngleVector(dir, 80f), Time.fixedDeltaTime * dirShiftPower);
                //Debug.Log("좌측2탐지");
            }

        }
                
    }
    bool CheckSensorColliders(RaycastHit2D[] _hit2ds)
    {
        int collisionCount = 0;
        for (int i = 0; i < _hit2ds.Length; i++)
        {
            // 총알   //나중엔 적총알  플레이어 총알 구분해야할듯
            if (_hit2ds[i].collider.CompareTag("Bullet"))
                continue;

            // 자신
            if (_hit2ds[i].collider == selfColl)
                continue;

            // 벽
            if (_hit2ds[i].collider.CompareTag("Wall"))
                continue;

            // 플레이어
            if (_hit2ds[i].collider.CompareTag("Player"))
                continue;

            //하나라도 충돌시
            collisionCount++;
            break;
        }

        if(collisionCount >= 1)
            return true;
        else
            return false;
    }


    //방향 업데이트
    //1. 패트롤 / 멈춰잇기
    //2. 적 쫒기 / 뒷걸음질치기
    //3. 메인캐릭터 쫒기
    Enemy scannedEnemy;
    float patrolTime = 2;
    float patrolTimer = 0;
    bool isMoving = false;
    bool isChasingEnemy = false;

    void UpdateDirection()
    {
        //배회하기
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

            //Debug.Log("배회");
        }
       

        //몬스터가 캐릭터 반경내에 들어온 경우
        if (scanner.nearestTarget != null)
        {
            scannedEnemy = scanner.nearestTarget.GetComponent<Enemy>();

            //적이 이미 근처에 있으면 스탑
            if (Vector3.Distance(transform.position, scanner.nearestTarget.position) - scannedEnemy.radius - playerRadius > distAwayFromEnemy &&
                Vector3.Distance(transform.position, scanner.nearestTarget.position) - scannedEnemy.radius - playerRadius < distAwayFromEnemy)
            {
                isMoving = false;
                isChasingEnemy = true;
                //Debug.Log("멈춤");
            }

            //메인캐릭터와의 반경 내에서 몬스터를 탐지한 경우 쫒기
            if (Vector3.Distance(transform.position, scanner.nearestTarget.position) - scannedEnemy.radius - playerRadius > distAwayFromEnemy &&
                Vector3.Distance(scanner.nearestTarget.position, mainCharacter.transform.position) - scannedEnemy.radius <= distWithinMainCharacter)
            {
                Vector2 chaseDir = scannedEnemy.transform.position - transform.position;

                dir = Vector3.Lerp(dir, chaseDir, Time.fixedDeltaTime * dirShiftPower);
                isMoving = true;
                isChasingEnemy = true;
                //Debug.Log("몬스터 쫒기");
            }
                        
            //몬스터가 가까이 왓으면 뒷걸음질
            if (Vector3.Distance(transform.position, scanner.nearestTarget.position) - scannedEnemy.radius - playerRadius < distAwayFromEnemy)
            {
                Vector2 backDir = transform.position - scannedEnemy.transform.position;

                dir = Vector3.Lerp(dir, backDir, Time.fixedDeltaTime * dirShiftPower * 5);
                isMoving = true;
                isChasingEnemy = true;
                //Debug.Log("ㅌㅌ");
            }
        }


        //메인 캐릭과 멀어지면 따라가기
        if (Vector3.Distance(transform.position, mainCharacter.transform.position) > distWithinMainCharacter)
        {
            Vector2 followDir = mainCharacter.transform.position - transform.position;

            dir = Vector3.Lerp(dir, followDir, Time.fixedDeltaTime * dirShiftPower);
            isMoving = true;
            isChasingEnemy = false;
            //Debug.Log("복귀");
        }

        if (isMoving)
        {
            if(isChasingEnemy == false)
                UpdateObstacleSensors();
        }
    }
}