using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAnimation : MonoBehaviour
{
    [SerializeField]
    GameObject group;
    [SerializeField]
    GameObject innerCircle;

    [SerializeField]
    float fillUpScale;


    public bool isReady = true;

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    FillUp();
        //}
    }
    
    public void AttackTargetArea(Vector3 _targetPos, Vector3 _areaScale, float _totalTime)
    {
        if (isReady == false)
            return;

        isReady = false;

        transform.position = (Vector2)_targetPos;
        transform.localScale = _areaScale;
        innerCircle.transform.localScale = Vector3.zero;
        group.SetActive(true);

        StartCoroutine(CoFillUp(_totalTime));
    }

    IEnumerator CoFillUp(float _totalTime)
    {
        StartCoroutine(MyCoroutines.CoLocalScale(innerCircle, fillUpScale, _totalTime));
        yield return new WaitForSeconds(_totalTime);
    }

    public void Done()
    {
        isReady = true;
        group.SetActive(false);
    }
}
