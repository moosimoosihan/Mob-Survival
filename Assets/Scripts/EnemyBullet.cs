using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyBullet : MonoBehaviour
{
    public float damage;
    public int per;
    public bool isCritical;
    public float duration;
    private IObjectPool<EnemyBullet> _ManagedPool;

    public virtual void Init(float _damage, int _per, bool _isCritical)
    {
        damage = _damage;
        per = _per;
        isCritical = _isCritical;
        DeActivate(duration);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player") == false)
            return;

        Attack(other.transform.GetComponent<Player>());
    }
    void Attack(Player _player)
    {
        _player.GetDamage(damage, isCritical);
        per--;

        if(per<1){
            gameObject.SetActive(false);
        }
    }
    void DeActivate(float _inTime)
    {
        StartCoroutine(CoDelayStarter(() =>
        {
            gameObject.SetActive(false);
        },
        _inTime
        ));        
    }
    protected IEnumerator CoDelayStarter(System.Action _action, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _action.Invoke();
    }
    public void SetManagedPool(IObjectPool<EnemyBullet> pool)
    {
        _ManagedPool = pool;
    }
}
