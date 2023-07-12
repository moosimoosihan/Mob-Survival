using UnityEngine;
using UnityEngine.Pool;

public class BuffEffect : MonoBehaviour
{
    private IObjectPool<BuffEffect> _ManagedPool;
    public void SetManagedPool(IObjectPool<BuffEffect> pool)
    {
        _ManagedPool = pool;
    }

    public void DestroyBuffEffect()
    {
        _ManagedPool.Release(this);
    }
}
