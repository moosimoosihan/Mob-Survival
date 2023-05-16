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
        //풀 미리 만들어놓기
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
            //N개 넘어가는 순간 부터는 순서대로 켜져있는애 강제로 끄고 가져오기
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

    public void ShowDamageLabelOnObj(int _damage, GameObject _OnObj, bool _isCritical)
    {
        GameObject tempScoreLabelObj = GetLabelFromPool();
        tempScoreLabelObj.SetActive(true);
        tempScoreLabelObj.GetComponent<DamageLabel>().ShowDamageAnimation(_damage, _OnObj, _isCritical);
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
        if(_character.critRate*100 > ran){
            //크리티컬 성공
            _isCritical = true;
            return _character.critDamage * _damage;
        } else {
            _isCritical = false;
            return _damage;
        }
    }
}
