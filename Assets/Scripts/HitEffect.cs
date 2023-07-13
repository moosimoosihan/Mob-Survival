using UnityEngine;
using UnityEngine.Pool;

public class HitEffect : MonoBehaviour
{
    private IObjectPool<HitEffect> _ManagedPool;
    
    public void SetManagedPool(IObjectPool<HitEffect> pool)
    {
        _ManagedPool = pool;
    }
    public void DestroyEffect()
    {
        _ManagedPool.Release(this);
    }
}
