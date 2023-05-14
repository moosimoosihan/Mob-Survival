using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    public float maxHP;
    public float curHP;
    public float speed;

    public float attackDamage;

    [SerializeField]
    bool createFollowingHpBar;

    protected void CreateFollowingHpBar()
    {
        if (createFollowingHpBar)
        {
            HealthFollow followingHpBar = UISingletonManager.Instance.followingBarManager.CreateFollowingHpBar().GetComponent<HealthFollow>();
            followingHpBar.Init(this.gameObject);
        }
    }
}
