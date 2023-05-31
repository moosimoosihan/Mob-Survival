using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    public float maxHP;
    public float curHP;
    public float speed;
    public float critRate;
    public float critDamage = 2;

    public float attackDamage;

    [SerializeField]
    bool createFollowingHpBar;

    public void CreateFollowingHpBar()
    {
        if (createFollowingHpBar)
        {
            HealthFollow followingHpBar = UISingletonManager.Instance.followingBarManager.CreateFollowingHpBar().GetComponent<HealthFollow>();
            followingHpBar.Init(this.gameObject);
        }
    }
}
