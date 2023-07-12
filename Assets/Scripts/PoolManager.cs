using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class PoolManager : MonoBehaviour
{
    public IObjectPool<BuffEffect> buffPool;

    Dictionary<GameObject, List<GameObject>> objPoolDic = new Dictionary<GameObject, List<GameObject>>();

    void Awake()
    {
        buffPool = new ObjectPool<BuffEffect>(CreateBuffEffect, OnGetBuffEffect, OnReleaseBuffEffect, OnDestroyBuffEffect);
    }

    public GameObject Get(GameObject _obj)
    {
        GameObject select = null;
        List<GameObject> tempList;
        bool ret = objPoolDic.TryGetValue(_obj, out tempList);
        if (ret == false)
        {
            //���� ��ųʸ��� �������� ������ ���� �������ֱ�
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
            select = Instantiate(_obj, transform);
            tempList.Add(select);
            select.SetActive(true);
        }

        return select;
    }
    BuffEffect CreateBuffEffect()
    {
        BuffEffect buffEffect = Instantiate(GameManager.instance.burnEffect).GetComponent<BuffEffect>();
        buffEffect.SetManagedPool(buffPool);
        return buffEffect;
    }
    void OnGetBuffEffect(BuffEffect buffEffect)
    {
        buffEffect.gameObject.SetActive(true);
    }
    void OnReleaseBuffEffect(BuffEffect buffEffect)
    {
        buffEffect.gameObject.SetActive(false);
    }
    void OnDestroyBuffEffect(BuffEffect buffEffect)
    {
        Destroy(buffEffect.gameObject);
    }
}
