using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class LoopSFXPlayer : MonoBehaviour
{
    private IObjectPool<LoopSFXPlayer> _ManagedPool;
    private AudioSource _AudioSource;
    public void SetManagedPool(IObjectPool<LoopSFXPlayer> pool)
    {
        _ManagedPool = pool;
    }
    public void Stop()
    {
        _ManagedPool.Release(this);
    }
    private void Awake()
    {
        _AudioSource =  GetComponent<AudioSource>();
    }
    private void Update()
    {
        if(!GameManager.instance.isPlay && _AudioSource.isPlaying){
            _AudioSource.Pause();
        } else if (GameManager.instance.isPlay && !_AudioSource.isPlaying){
            _AudioSource.UnPause();
        }
    }
}
