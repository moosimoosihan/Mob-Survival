using UnityEngine;
using System.Collections;
using System;

public class CharacterStatus : MonoBehaviour
{
    public string character;
    private float maxHP;
    private float curHP = 0;
    public float maxShield;
    public float curShield;
    public bool isShield;
    public float shieldTime;
    public float shieldCurTime;
    public float speed;
    public float critRate;
    public float critDamage = 2;
    public float def;
    public float evasion;
    public float heal;

    public float attackDamage;

    public float MaxHP
    {
        get
        {
            return maxHP;
        }
        set
        {
            OnHpChanged?.Invoke();
            maxHP = value;
        }
    }

    public float CurHP
    {
        get
        {
            return curHP;
        }
        set
        {
            OnHpChanged?.Invoke();
            curHP = value;
        }
    }

    public Action OnHpChanged;

    [SerializeField]
    bool createFollowingHpBar;

    // 속도 저항
    public float resistance = 1;

    // 버프 이펙트
    public GameObject shielSfxPlayer;
    public void CreateFollowingHpBar()
    {
        if (createFollowingHpBar)
        {
            HealthFollow followingHpBar = UISingletonManager.Instance.followingBarManager.CreateFollowingHpBar().GetComponent<HealthFollow>();
            followingHpBar.Init(this.gameObject);
        }
    }
    public IEnumerator ShieldOn()
    {
        isShield = true;
        // 이펙트 생성해야 함
        HealthFollow followingSdBar = UISingletonManager.Instance.followingBarManager.CreateFollowingSdBar().GetComponent<HealthFollow>();
        followingSdBar.Init(this.gameObject);

        shielSfxPlayer = AudioManager.Instance.LoopSfxPlay(AudioManager.LoopSfx.Shield);

        shieldCurTime = shieldTime;
        while (shieldCurTime > 0)
        {
            shieldCurTime -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        shieldCurTime = 0;
        curShield = 0;

        shielSfxPlayer.GetComponent<LoopSFXPlayer>().Stop();
        //이펙트 제거해야 함
        isShield = false;
    }
    public IEnumerator Speedresistance(float _resistance, float _time)
    {
        resistance = _resistance;
        yield return new WaitForSeconds(_time);
        resistance = 1;
    }
}
