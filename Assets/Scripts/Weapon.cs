using UnityEngine;

public abstract class Weapon : MonoBehaviour
{    
    [Header("무기 정보")]
    public float damage;
    public int count;
    public float delay;
    public float curDelay;
    public float bulletSpeed;
    public float knockBackPower;
    public float duration;
    public bool isCritical;
    
    protected float timer;
    protected Player player;

    [SerializeField]
    protected GameObject projectilePrefab;

    void Awake()
    {
        player = GetComponentInParent<Player>();
    }
    
    void Start()
    {
        curDelay = delay;
        InitWeapon();
    }

    void Update()
    {
        UpdateWeapon();
    }
    public void LevelUp()
    {

    }

    public abstract void InitWeapon();

    public abstract void UpdateWeapon();
}
