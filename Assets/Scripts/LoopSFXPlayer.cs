using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class LoopSFXPlayer : MonoBehaviour
{
    private IObjectPool<LoopSFXPlayer> _ManagedPool;
    public void SetManagedPool(IObjectPool<LoopSFXPlayer> pool)
    {
        _ManagedPool = pool;
    }
    public void Stop()
    {
        _ManagedPool.Release(this);
    }
}
