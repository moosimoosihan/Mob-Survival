using System.Collections;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float damage;
    public int per;
    protected float duration = 5;

    public void Init(float _damage, int _per)
    {
        damage = _damage;
        per = _per;
        DeActivate(duration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player") == false)
            return;

        Attack(other.transform.GetComponent<Player>());
    }
    void Attack(Player _player)
    {
        _player.GetDamage(damage);
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
}
