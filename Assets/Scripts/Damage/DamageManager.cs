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
    public float Critical(CharacterStatus _character, float _damage, out bool _isCritical, bool trueCritical = false)
    {
        if(trueCritical){
            _isCritical = true;
            return _character.CurCritDamage * _damage;
        } else {
            int ran = Random.Range(0,100);
            if(_character.CurCritRate*100 > ran){
                //ũ��Ƽ�� ����
                _isCritical = true;
                return _character.CurCritDamage * _damage;
            } else {
                _isCritical = false;
                return _damage;
            }
        }
    }

    public float ElementalDamageCalculator(float damage, Elemental isAttacker, Elemental isDeffender, float isAttackerElementalDamager){
        // 물 물 = 80%
        // 물 불 = 150%
        // 물 땅 = 100%
        // 물 바람 = 50%

        // 불 물 = 50%
        // 불 불 = 80%
        // 불 땅 = 150%
        // 불 바람 = 100%

        // 땅 물 = 100%
        // 땅 불 = 50%
        // 땅 땅 = 80%
        // 땅 바람 = 150%

        // 바람 물 = 150%
        // 바람 불 = 100%
        // 바람 땅 = 50%
        // 바람 바람 = 80%
        if(isAttacker == Elemental.Water){
            if(isDeffender == Elemental.Water){
                return damage * 0.8f * isAttackerElementalDamager;
            } else if(isDeffender == Elemental.Fire){
                return damage * 1.5f* isAttackerElementalDamager;
            } else if(isDeffender == Elemental.Earth){
                return damage * 1f* isAttackerElementalDamager;
            } else if(isDeffender == Elemental.Wind){
                return damage * 0.5f* isAttackerElementalDamager;
            }
        } else if(isAttacker == Elemental.Fire){
            if(isDeffender == Elemental.Water){
                return damage * 0.5f* isAttackerElementalDamager;
            } else if(isDeffender == Elemental.Fire){
                return damage * 0.8f* isAttackerElementalDamager;
            } else if(isDeffender == Elemental.Earth){
                return damage * 1.5f* isAttackerElementalDamager;
            } else if(isDeffender == Elemental.Wind){
                return damage * 1f* isAttackerElementalDamager;
            }
        } else if(isAttacker == Elemental.Earth){
            if(isDeffender == Elemental.Water){
                return damage * 1f* isAttackerElementalDamager;
            } else if(isDeffender == Elemental.Fire){
                return damage * 0.5f* isAttackerElementalDamager;
            } else if(isDeffender == Elemental.Earth){
                return damage * 0.8f* isAttackerElementalDamager;
            } else if(isDeffender == Elemental.Wind){
                return damage * 1.5f* isAttackerElementalDamager;
            }
        } else if(isAttacker == Elemental.Wind){
            if(isDeffender == Elemental.Water){
                return damage * 1.5f* isAttackerElementalDamager;
            } else if(isDeffender == Elemental.Fire){
                return damage * 1f* isAttackerElementalDamager;
            } else if(isDeffender == Elemental.Earth){
                return damage * 0.5f* isAttackerElementalDamager;
            } else if(isDeffender == Elemental.Wind){
                return damage * 0.8f* isAttackerElementalDamager;
            }
        }
        return damage;
    }
}
