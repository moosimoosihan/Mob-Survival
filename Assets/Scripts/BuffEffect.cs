using UnityEngine;
using UnityEngine.Pool;

public class BuffEffect : MonoBehaviour
{
    public Transform target;
    private IObjectPool<BuffEffect> _ManagedPool;
    void FixedUpdate(){
        if(target != null && target.gameObject.activeSelf)
            transform.position = target.position;
        else
            DestroyBuffEffect();
    }
    public void SetManagedPool(IObjectPool<BuffEffect> pool)
    {
        _ManagedPool = pool;
    }
    public void DestroyBuffEffect()
    {
        _ManagedPool.Release(this);
    }
}
