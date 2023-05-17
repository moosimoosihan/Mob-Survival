using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IreliaWeapon : Weapon
{
    //�߻�ü ����
    [SerializeField]
    List<Bullet> createdBulletList = new List<Bullet>();

    //�߻�ü �� �����Ǿ�� �غ�Ϸ�
    [SerializeField]
    bool isCreatingBullets = false;
    [SerializeField]
    bool isBulletsCreated = false;
    [SerializeField]
    bool isRotate = false;
    [SerializeField]
    bool onTarget = false;

    Vector3 targetPos;
    Vector3 lastDir;

    [SerializeField]
    int creationCount = 1;

    [SerializeField]
    float rotationSpeed = 10;

    public override void InitWeapon()
    {
        delay = 1;
    }

    public override void UpdateWeapon()
    {
        //�Ѿ� �����Ǿ� ������ ȸ�������ְ� �ƴϸ� �������� ����
        if (isBulletsCreated)
        {
            //�߻�
            timer += Time.deltaTime;
            if (timer > delay)
            {
                if (Fire())
                {
                    //isRotate = false;
                    timer = 0f;
                }
            }
        }
        else
        {
            //����
            timer += Time.deltaTime;
            if (timer > delay)
            {
                if(CreateBullets())
                {
                    isRotate = true;
                    timer = 0f;
                }
            }
        }

        //��� ȸ���ϱ�
        if (isRotate)
        {
            transform.localRotation = Quaternion.Euler(0, 0, transform.localRotation.eulerAngles.z + Time.deltaTime * rotationSpeed);            

            if (onTarget)
            {
                for (int i = 0; i < createdBulletList.Count; i++)
                {
                    Vector2 tempTargetDir = targetPos - createdBulletList[i].transform.position;
                    float targetDegree = Mathf.Atan2(tempTargetDir.y, tempTargetDir.x) * Mathf.Rad2Deg - 90;    //�����ߵż�
                    createdBulletList[i].transform.rotation = Quaternion.Lerp(createdBulletList[i].transform.rotation , Quaternion.Euler(0, 0, targetDegree), Time.deltaTime * 20);
                }
            }
            else
            {
                for (int i = 0; i < createdBulletList.Count; i++)
                {
                    createdBulletList[i].transform.rotation = Quaternion.identity;
                }
            }
        }        
    }

    
    bool Fire()
    {
        if (!player.scanner.nearestTarget)
            return false;

        targetPos = player.scanner.nearestTarget.position;
        lastDir = targetPos - transform.position;
        lastDir = lastDir.normalized;

        onTarget = true;
        StartCoroutine(CoFire());
        return true;
    }

    IEnumerator CoFire()
    {
        yield return new WaitForSeconds(0.75f);

        while (createdBulletList.Count > 0)
        {
            Bullet tempBullet = createdBulletList[0];
            tempBullet.transform.SetParent(GameManager.instance.transform);
            createdBulletList.RemoveAt(0);

            if (player.scanner.nearestTarget)
            {
                targetPos = player.scanner.nearestTarget.position;
                lastDir = targetPos - tempBullet.transform.position;
                lastDir = lastDir.normalized;
            }

            tempBullet.transform.rotation = Quaternion.FromToRotation(Vector3.up, lastDir);

            tempBullet.Fire(DamageManager.Instance.Critical(GetComponentInParent<Player>(), damage, out bool isCritical), count, lastDir, knockBackPower, isCritical);
            yield return new WaitForSeconds(0.1f);                
        }


        onTarget = false;
        createdBulletList.Clear();

        yield return new WaitForSeconds(0.1f);
        isCreatingBullets = false;
        isBulletsCreated = false;
    }
        
    

    bool CreateBullets()
    {
        if (isCreatingBullets)
            return false;

        if (creationCount <= 0)
            return false;

        isCreatingBullets = true;
        isBulletsCreated = false;
        StartCoroutine(CoCreateBullets());
        return true;
    }

    IEnumerator CoCreateBullets()
    {
        Vector3 dir = Vector3.up;
        int tempPenetrationCount = 10000;
        float distanceFromPivot = 1.5f;
        float angleOffset = 360 / creationCount;
        float angleCumulative = 0;

        for (int i = 0; i < creationCount; i++)
        {
            Transform tempBullet = GameManager.instance.pool.Get(projectilePrefab).transform;
            Bullet bulletScript = tempBullet.GetComponent<Bullet>();
            tempBullet.SetParent(transform);

            Vector3 startPos = transform.localPosition;

            //�Ѿ� �������� ��ġ            
            float x = Mathf.Cos(angleCumulative * Mathf.Deg2Rad);
            float y = Mathf.Sin(angleCumulative * Mathf.Deg2Rad);

            Vector3 positionOffset = new Vector3(x, y, startPos.z);

            tempBullet.localPosition = Vector3.zero + (positionOffset.normalized * distanceFromPivot);
            tempBullet.rotation = Quaternion.identity;
            bulletScript.Fire(damage, tempPenetrationCount, Vector3.zero, knockBackPower, false, false);
            createdBulletList.Add(bulletScript);
            angleCumulative += angleOffset;

            yield return new WaitForSeconds(0.5f);
        }

        isBulletsCreated = true;
    }


    IEnumerator CoDelayStarter(System.Action _action ,float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _action.Invoke();
    }
}