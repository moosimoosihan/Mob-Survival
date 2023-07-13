using UnityEngine;
using UnityEngine.Pool;

public class BuffEffect : MonoBehaviour
{
    private IObjectPool<BuffEffect> _ManagedPool;
    public Transform target;
    private void Update() {
        if(target.gameObject.activeSelf)
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
