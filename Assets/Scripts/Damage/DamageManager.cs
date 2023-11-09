using System.Collections.Generic;
using UnityEngine;

public class DamageManager : Singleton<DamageManager>
{
    public Sprite[] numberSprites;
    public Sprite[] messages;
    public GameObject scoreLabelPrefab;

    public AnimationCurve labelMoveUpAnimationCurve;

    List<GameObject> labelPoolList = new List<GameObject>();

    [SerializeField]
    int poolLimitCount;

    void Start()
    {
        //Ǯ �̸� ��������
        GameObject tempObj = null;
        for (int i = 0; i < poolLimitCount; i++)
        {
            tempObj = Instantiate(scoreLabelPrefab, transform);
            labelPoolList.Add(tempObj);
        }
    }
    

    GameObject GetLabelFromPool()
    {
        GameObject tempLabel = null;
        for (int i = 0; i < labelPoolList.Count; i++)
        {
            if (labelPoolList[i].activeSelf == false)
            {
                tempLabel = labelPoolList[i];
                break;
            }
        }

        if (tempLabel == null)
        {
            //N�� �Ѿ�� ���� ���ʹ� ������� �����ִ¾� ������ ���� ��������
            if(labelPoolList.Count >= poolLimitCount)
            {
                for (int i = 0; i < labelPoolList.Count; i++)
                {
                    if (labelPoolList[i].activeSelf)
                    {
                        labelPoolList[i].SetActive(false);
                        tempLabel = labelPoolList[i];
                        break;
                    }
                }
            }
            else
            {
                tempLabel = Instantiate(scoreLabelPrefab, transform);
                labelPoolList.Add(tempLabel);
            }
        }

        return tempLabel;
    }

    public void ShowDamageLabelOnObj(int _damage, GameObject _OnObj, bool _isCritical, bool _isPlayerDamge)
    {
        GameObject tempScoreLabelObj = GetLabelFromPool();
        tempScoreLabelObj.SetActive(true);
        tempScoreLabelObj.GetComponent<DamageLabel>().ShowDamageAnimation(_damage, _OnObj, _isCritical, _isPlayerDamge);
    }
    public void ShowMessageLabelOnObj(DamageLabel.Message _message, GameObject _OnObj)
    {
        GameObject tempScoreLabelObj = GetLabelFromPool();
        tempScoreLabelObj.SetActive(true);
        tempScoreLabelObj.GetComponent<DamageLabel>().ShowDamageAnimation(_message, _OnObj);
    }
    public float Critical(CharacterStatus _character, float _damage, out bool _isCritical)
    {
        int ran = Random.Range(0,100);
        if(_character.CurCritRate*100 > ran){
            //ũ��Ƽ�� ����
            _isCritical = true;
            return _character.CurCritRate * _damage;
        } else {
            _isCritical = false;
            return _damage;
        }
    }
}
