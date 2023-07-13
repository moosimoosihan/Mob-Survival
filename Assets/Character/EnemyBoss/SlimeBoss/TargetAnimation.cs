using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class TargetAnimation : MonoBehaviour
{
    [SerializeField]
    GameObject group;
    [SerializeField]
    GameObject innerCircle;

    [SerializeField]
    float fillUpScale;

    private IObjectPool<TargetAnimation> _ManagedPool;

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
        _ManagedPool.Release(this);
    }
    public void SetManagedPool(IObjectPool<TargetAnimation> pool)
    {
        _ManagedPool = pool;
    }

}
