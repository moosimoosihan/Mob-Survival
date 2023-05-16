using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingBarManager : MonoBehaviour
{
    [SerializeField]
    GameObject hpBarPrefab;

    [SerializeField]
    GameObject poolGroup;

    Dictionary<GameObject, List<GameObject>> objPoolDic = new Dictionary<GameObject, List<GameObject>>();

    public GameObject Get(GameObject _obj)
    {
        GameObject select = null;
        List<GameObject> tempList;
        bool ret = objPoolDic.TryGetValue(_obj, out tempList);
        if (ret == false)
        {
            //아직 딕셔너리에 생성한적 없으면 새로 생성해주기
            tempList = new List<GameObject>();
            objPoolDic.Add(_obj, tempList);
        }

        foreach (GameObject item in tempList)
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }

        if (!select)
        {
            select = Instantiate(_obj, poolGroup.transform);
            tempList.Add(select);
            select.SetActive(true);
        }

        return select;
    }

    public GameObject CreateFollowingHpBar()
    {
        GameObject tempObj = Get(hpBarPrefab);
        return tempObj;
    }
}
