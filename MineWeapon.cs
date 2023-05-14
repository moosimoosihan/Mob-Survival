using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineWeapon : Weapon
{
    // 현재 생성된 발사체 개수
    [SerializeField]
    List<GameObject> createdMineList = new List<GameObject>();
    [SerializeField]
    int curCreatCount = 0;
    [SerializeField]
    protected float duration = 5;

    public override void InitWeapon()
    {
        delay = 1;
    }

    public override void UpdateWeapon()
    {
        // 지뢰를 생성 할 수 있다면 생성        
        timer += Time.deltaTime;
        if (timer > delay)
        {
            bool ret = FireCheck();
            if (ret)
            {
                if (Fire())
                {
                    timer = 0f;
                }
            }            
        }        
    }

    bool Fire()
    {
        if (!player.scanner.nearestTarget)
            return false;

        CreateBullet();
        return true;
    }
    void CreateBullet()
    {
        Transform mineBullet = GameManager.instance.pool.Get(projectilePrefab).transform;
        Bullet bulletScript = mineBullet.GetComponent<Bullet>();
        //mineBullet.SetParent(transform);
        mineBullet.transform.position = transform.position;
        mineBullet.rotation = Quaternion.identity;
        
        if (!createdMineList.Contains(mineBullet.gameObject) || createdMineList.Count < count)
        {
            createdMineList.Add(mineBullet.gameObject);
        }

        bulletScript.Init(damage, -1);
        bulletScript.DeActivate(duration);
    }

    bool FireCheck()
    {
        int tempCount = 0;

        for (int i = 0; i < createdMineList.Count; i++)
        {
            GameObject obj = createdMineList[i];
            if (obj.activeSelf)
            {
                tempCount++;
            }
        }

        curCreatCount = tempCount;
        return curCreatCount < count;
    }

    protected void DeActivate()
    {
        gameObject.SetActive(false);
    }
    IEnumerator CoDelayStarter(System.Action _action, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _action.Invoke();
    }
}
