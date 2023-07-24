using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyBullet : MonoBehaviour
{
    public bool bossBullet;
    public float damage;
    public int per;
    public bool isCritical;
    public float duration;
    private IObjectPool<EnemyBullet> _ManagedPool;
    List<Player> damagedPlayers = new List<Player>();

    public virtual void Init(float _damage, int _per, bool _isCritical, bool _bossBullet = false, bool _deActivate = true)
    {
        damage = _damage;
        per = _per;
        isCritical = _isCritical;
        bossBullet = _bossBullet;
        if(_deActivate)
            DeActivate(duration);
        
        damagedPlayers.Clear();
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player") == false)
            return;

        if(!damagedPlayers.Contains(other.transform.GetComponent<Player>())){
            damagedPlayers.Add(other.transform.GetComponent<Player>());
            Attack(other.transform.GetComponent<Player>());
        }
    }
    void Attack(Player _player)
    {
        // 보스 공격은 무적시간 없음
        if(bossBullet){
            if(_player.isDamaged){
                _player.isDamaged = false;
                _player.StopCoroutine(_player.DamageDelay());
            }
        }

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
