using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineWeapon : Weapon
{
    // ???? ?????? ???? ????
    [SerializeField]
    List<GameObject> createdMineList = new List<GameObject>();
    [SerializeField]
    int curCreatCount = 0;
    [SerializeField]
    float weaponDuration;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        // ????? ???? ?? ?? ???? ????        
        timer += Time.deltaTime;
        if (timer > CurDelay)
        {
            bool ret = FireCheck();
            if (ret)
            {
                if (OnFire())
                {
                    timer = 0f;
                }
            }            
        }        
    }
    protected override void Fire()
    {

    }

    bool OnFire()
    {
        if (!player.scanner.nearestTarget)
            return false;

        CreateBullet();
        return true;
    }
    void CreateBullet()
    {
        Transform mineBullet = poolBullet.Get().transform;
        mineBullet.parent = GameManager.instance.pool.transform;
        Bullet bulletScript = mineBullet.GetComponent<Bullet>();
        //mineBullet.SetParent(transform);
        mineBullet.transform.position = transform.position;
        mineBullet.rotation = Quaternion.identity;
        
        if (!createdMineList.Contains(mineBullet.gameObject) || createdMineList.Count < CurCount)
        {
            createdMineList.Add(mineBullet.gameObject);
        }
        bulletScript.player = player;
        bulletScript.Init(DamageManager.Instance.Critical(GetComponentInParent<Player>(),Damage, out bool isCritical), -1, CurDuration, knockBackPower, isCritical);
        bulletScript.DeActivate(weaponDuration);
        if(isCritical || this.isCritical) {this.isCritical=true;} else {this.isCritical=false;}
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
        return curCreatCount < CurCount;
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
