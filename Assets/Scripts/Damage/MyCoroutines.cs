using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyCoroutines
{
    public static IEnumerator CoLocalScale(GameObject _obj, float _toScale, float _totalTime)
    {
        Vector3 startScale = _obj.transform.localScale;
        Vector3 targetScale = new Vector3(_toScale, _toScale, _toScale);
        float timer = 0;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer < _totalTime)
            {
                _obj.transform.localScale = Vector3.Lerp(startScale, targetScale, timer / _totalTime);
            }
            else
            {
                _obj.transform.localScale = targetScale;
                break;
            }

            yield return null;
        }
    }

    public static IEnumerator CoChangeSize(GameObject _obj, float _fromScale, float _toScale, float _totalTime)
    {
        Vector3 startScale = new Vector3(_fromScale, _fromScale, 1);
        Vector3 targetScale = new Vector3(_toScale, _toScale, 1);
        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer < _totalTime)
            {
                _obj.transform.localScale = Vector3.Lerp(startScale, targetScale, timer / _totalTime);
            }
            else
            {
                _obj.transform.localScale = targetScale;
                break;
            }

            yield return null;
        }
    }
    public static IEnumerator CoChangeSizeX(GameObject _obj, float _fromScale, float _toScale, float _totalTime)
    {
        Vector3 startScale = new Vector3(_fromScale, _obj.transform.localScale.y, 1);
        Vector3 targetScale = new Vector3(_toScale, _obj.transform.localScale.y, 1);
        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer < _totalTime)
            {
                _obj.transform.localScale = Vector3.Lerp(startScale, targetScale, timer / _totalTime);
            }
            else
            {
                _obj.transform.localScale = targetScale;
                break;
            }

            yield return null;
        }
    }
    public static IEnumerator CoShakeObj(GameObject _obj, float _shakeAmount, float _shakeTime = 0.25f)
    {
        Vector3 originPos = _obj.transform.position;
        Vector3 shakeVec;
        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer < _shakeTime)
            {
                shakeVec = Random.insideUnitCircle * _shakeAmount;
                _obj.transform.position = new Vector3(originPos.x + shakeVec.x, originPos.y + shakeVec.y, _obj.transform.position.z);
            }
            else
            {
                _obj.transform.position = originPos;
                break;
            }

            yield return null;
        }
    }
    public static IEnumerator CoFadeInOut(GameObject _obj, float _fromAlpha, float _toAlpha, float _totalTime)
    {
        SpriteRenderer tempSpriteRenderer = _obj.GetComponent<SpriteRenderer>();
        Color color = tempSpriteRenderer.color;

        float timer = 0;
        while (tempSpriteRenderer != null)
        {
            timer += Time.deltaTime;
            if (timer < _totalTime)
            {
                color.a = Mathf.Lerp(_fromAlpha, _toAlpha, timer / _totalTime);
                if (tempSpriteRenderer != null)
                    tempSpriteRenderer.color = color;
            }
            else
            {
                color.a = _toAlpha;
                if (tempSpriteRenderer != null)
                    tempSpriteRenderer.color = color;
                break;
            }

            yield return null;
        }
    }
    public static IEnumerator CoFadeInOut(SpriteRenderer _spriteRenderer, float _fromAlpha, float _toAlpha, float _totalTime)
    {
        Color color = _spriteRenderer.color;

        float timer = 0;
        while (_spriteRenderer != null)
        {
            timer += Time.deltaTime;
            if (timer < _totalTime)
            {
                color.a = Mathf.Lerp(_fromAlpha, _toAlpha, timer / _totalTime);
                if (_spriteRenderer != null)
                    _spriteRenderer.color = color;
            }
            else
            {
                color.a = _toAlpha;
                if (_spriteRenderer != null)
                    _spriteRenderer.color = color;
                break;
            }

            yield return null;
        }
    }

    public static IEnumerator CoFadeInOut_AnimationCurve(GameObject _obj, float _fromAlpha, float _toAlpha, float _totalTime, AnimationCurve _aniCurve)
    {
        SpriteRenderer tempSpriteRenderer = _obj.GetComponent<SpriteRenderer>();
        Color color = tempSpriteRenderer.color;

        float timer = 0;
        float e = 0;
        while (tempSpriteRenderer != null)
        {
            timer += Time.deltaTime;
            e = _aniCurve.Evaluate(timer / _totalTime);

            if (timer < _totalTime)
            {
                color.a = Mathf.Lerp(_fromAlpha, _toAlpha, e);
                if (tempSpriteRenderer != null)
                    tempSpriteRenderer.color = color;
            }
            else
            {
                color.a = _toAlpha;
                if (tempSpriteRenderer != null)
                    tempSpriteRenderer.color = color;
                break;
            }

            yield return null;
        }
    }

    public static IEnumerator CoGlobalMove_AnimationCurve(GameObject _obj, Vector3 _startPos, Vector3 _targetPos, float _totalTime, AnimationCurve _aniCurve)
    {
        float timer = 0;
        float e = 0;

        while (true)
        {
            timer += Time.deltaTime;
            e = _aniCurve.Evaluate(timer / _totalTime);

            if (timer < _totalTime)
            {
                _obj.transform.position = Vector3.Lerp(_startPos, _targetPos, e);
            }
            else
            {
                _obj.transform.position = _targetPos;
                break;
            }

            yield return null;
        }
    }

    public static IEnumerator CoDelayStarter(System.Action _action, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _action.Invoke();
    }
}
