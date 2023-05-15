using System.Collections;
using UnityEngine;

public class DamageLabel : MonoBehaviour
{
    public enum Message { Miss }
    public Message message;
    public GameObject group;
    public SpriteRenderer[] curScoreSpriteRenderers;
    [SerializeField] Vector3[] digitOffsets;

    int[] degits = new int[6];
    int curDamage = 0;
    [SerializeField]
    Vector3 originScale;
    bool isHeal = false;


    public void UpdateScore(int _newDamage)
    {
        if(_newDamage<=0){
            // 힐로 분류
            isHeal = true;
            curDamage = Mathf.Abs(_newDamage);
        } else {
            // 데미지로 분류
            isHeal = false;
            curDamage = _newDamage;
        }
        
        degits[5] = curDamage / 100000;
        degits[4] = curDamage % 100000 / 10000;
        degits[3] = curDamage % 10000 / 1000;
        degits[2] = curDamage % 1000 / 100;
        degits[1] = curDamage % 100 / 10;
        degits[0] = curDamage % 10;

        for (int i = 0; i < degits.Length; i++)
        {
            curScoreSpriteRenderers[i].sprite = CheckCurrentDegitAndUpperDegitExist(ref degits, i, degits.Length - 1) ? DamageManager.Instance.numberSprites[degits[i]] : null;
            curScoreSpriteRenderers[i].color = isHeal ? new Color(0,1,0) :  new Color(1,1,1);
        }

        if(curDamage < 10)
        {
            group.transform.localPosition = digitOffsets[0];
        }
        else if (curDamage < 100)
        {
            group.transform.localPosition = digitOffsets[1];
        }
        else if (curDamage < 1000)
        {
            group.transform.localPosition = digitOffsets[2];
        }
        else if (curDamage < 10000)
        {
            group.transform.localPosition = digitOffsets[3];
        }
        else if (curDamage < 100000)
        {
            group.transform.localPosition = digitOffsets[4];
        }
        else
        {
            group.transform.localPosition = digitOffsets[5];
        }
    }
    public void UpdateMessage(Message _newMessage){
        for (int i = 0; i < degits.Length; i++)
        {
            curScoreSpriteRenderers[i].sprite = CheckCurrentDegitAndUpperDegitExist(ref degits, i, degits.Length - 1) ? DamageManager.Instance.numberSprites[degits[i]] : null;
        }
        switch(_newMessage){
            case Message.Miss:
                curDamage = 0;
                degits[5] = curDamage / 100000;
                degits[4] = curDamage % 100000 / 10000;
                degits[3] = curDamage % 10000 / 1000;
                degits[2] = curDamage % 1000 / 100;
                degits[1] = curDamage % 100 / 10;
                degits[0] = curDamage % 10;
                for (int i = 0; i < degits.Length; i++)
                {
                    curScoreSpriteRenderers[i].sprite = CheckCurrentDegitAndUpperDegitExist(ref degits, i, degits.Length - 1) ? DamageManager.Instance.numberSprites[degits[i]] : null;
                    curScoreSpriteRenderers[i].color = new Color(1,1,1);
                }
                curScoreSpriteRenderers[0].sprite = DamageManager.Instance.messages[0];
                break;
        }
    }

    bool CheckCurrentDegitAndUpperDegitExist(ref int[] _degits, int _currentDegit, int _maxDegit)
    {
        for (int i = _maxDegit; i >= _currentDegit; i--)
        {
            if (i == _currentDegit && _currentDegit == 0)
                return true;

            if (_degits[i] != 0)
                return true;
        }

        return false;
    }

    public void ShowDamageAnimation(int _damage, GameObject _OnObj)
    {
        UpdateScore(_damage);
        StartCoroutine(CoSpawnDamageLabel(_OnObj));
    }
    public void ShowDamageAnimation(Message _message, GameObject _OnObj)
    {
        UpdateMessage(_message);
        StartCoroutine(CoSpawnMessageLabel(_OnObj));
    }
    IEnumerator CoSpawnDamageLabel(GameObject _OnObj)
    {
        //시작 사이즈 살짝 랜덤화
        float tempOffset = Random.Range(-0.05f, 0.05f);
        transform.localScale = new Vector3(
            originScale.x + tempOffset,
            originScale.y + tempOffset,
            originScale.z);

        //살짝 위에서 시작
        Vector3 startPos = new Vector3(
            _OnObj.transform.position.x + Random.Range(-0.25f, 0.25f),
            _OnObj.transform.position.y + 0.75f + Random.Range(-0.25f, 0.25f),
            _OnObj.transform.position.z);

        Vector3 targetPos = new Vector3(startPos.x, startPos.y + 0.75f, startPos.z);

        //시작 위치 세팅
        transform.position = startPos;

        //흔들면서
        //StartCoroutine(CoShakeObj(tempScoreObj, 0.09f, 0.30f));   //안이쁨..

        //켜주기
        group.SetActive(true);
        
        //커진상태에서 작아지기
        StartCoroutine(MyCoroutines.CoChangeSize(gameObject, 3f, transform.localScale.x, 0.15f));

        //빠르게 페이드인하면서 등장
        for (int i = 0; i < curScoreSpriteRenderers.Length; i++)
        {
            StartCoroutine(MyCoroutines.CoFadeInOut(curScoreSpriteRenderers[i], 0, 1, 0.15f));
        }

        
        //잠깐 보여줬다가
        yield return new WaitForSeconds(0.5f);

        float fadeTime = 0.3f;
        //위로 올라가면서 페이드 아웃
        for (int i = 0; i < curScoreSpriteRenderers.Length; i++)
        {
            StartCoroutine(MyCoroutines.CoFadeInOut(curScoreSpriteRenderers[i], 1, 0, fadeTime));
        }

        StartCoroutine(MyCoroutines.CoGlobalMove_AnimationCurve(gameObject, startPos, targetPos, fadeTime, DamageManager.Instance.labelMoveUpAnimationCurve));

        yield return new WaitForSeconds(fadeTime);

        //Off시키기
        group.SetActive(false);
        gameObject.SetActive(false);
    }
    IEnumerator CoSpawnMessageLabel(GameObject _OnObj)
    {
        //시작 사이즈 살짝 랜덤화
        float tempOffset = Random.Range(-0.05f, 0.05f);
        transform.localScale = new Vector3(
            originScale.x + tempOffset,
            originScale.y + tempOffset,
            originScale.z);

        //살짝 위에서 시작
        Vector3 startPos = new Vector3(
            _OnObj.transform.position.x + Random.Range(-0.25f, 0.25f),
            _OnObj.transform.position.y + 0.75f + Random.Range(-0.25f, 0.25f),
            _OnObj.transform.position.z);

        Vector3 targetPos = new Vector3(startPos.x, startPos.y + 0.75f, startPos.z);

        //시작 위치 세팅
        transform.position = startPos;

        //흔들면서
        //StartCoroutine(CoShakeObj(tempScoreObj, 0.09f, 0.30f));   //안이쁨..

        //켜주기
        group.SetActive(true);
        
        //커진상태에서 작아지기
        StartCoroutine(MyCoroutines.CoChangeSize(gameObject, 3f, transform.localScale.x, 0.15f));

        //빠르게 페이드인하면서 등장
        StartCoroutine(MyCoroutines.CoFadeInOut(curScoreSpriteRenderers[0], 0, 1, 0.15f));
        
        //잠깐 보여줬다가
        yield return new WaitForSeconds(0.5f);

        float fadeTime = 0.3f;
        //위로 올라가면서 페이드 아웃
        StartCoroutine(MyCoroutines.CoFadeInOut(curScoreSpriteRenderers[0], 1, 0, fadeTime));

        StartCoroutine(MyCoroutines.CoGlobalMove_AnimationCurve(gameObject, startPos, targetPos, fadeTime, DamageManager.Instance.labelMoveUpAnimationCurve));

        yield return new WaitForSeconds(fadeTime);

        //Off시키기
        group.SetActive(false);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        group.SetActive(false);
        transform.localScale = originScale;
        StopAllCoroutines();
    }
}
