using UnityEngine;
using UnityEngine.Pool;

public class GiantEntBullet : EnemyBullet
{
    [SerializeField]
    public GameObject hitEffectPrefab;
    public IObjectPool<BuffEffect> hitEffectPool;
    void Awake()
    {
        hitEffectPool = new ObjectPool<BuffEffect>(CreateEffect, OnGetEffect, OnReleaseEffect, OnDestroyEffect);
    }
    public override void Init(float _damage, int _per, bool _isCritical)
    {
        base.Init(_damage, _per, _isCritical);
    }
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        
        if(other.transform.CompareTag("Player")==false)
            return;
            
        // 해당 플레이어의 이동속도를 제한 몇초동안? 묶인 이펙트를 생성해야 함
        Player player = other.GetComponent<Player>();
        player.StartCoroutine(player.Speedresistance(0, 1));
        BuffEffect hitEffect = hitEffectPool.Get();
        hitEffect.transform.parent = GameManager.instance.pool.transform;
        hitEffect.target = other.transform;
        hitEffect.Invoke("DestroyBuffEffect", 1.0f);
    }
    public void DeActivate()
    {
        gameObject.SetActive(false);
    }
    BuffEffect CreateEffect()
    {
        BuffEffect buffEffect = Instantiate(hitEffectPrefab).GetComponent<BuffEffect>();
        buffEffect.SetManagedPool(hitEffectPool);
        return buffEffect;
    }
    void OnGetEffect(BuffEffect buffEffect)
    {
        buffEffect.gameObject.SetActive(true);
    }
    void OnReleaseEffect(BuffEffect buffEffect)
    {
        if (buffEffect.gameObject.activeSelf)
            buffEffect.gameObject.SetActive(false);
    }
    void OnDestroyEffect(BuffEffect buffEffect)
    {
        Destroy(buffEffect.gameObject);
    }
}
