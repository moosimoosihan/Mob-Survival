using UnityEngine;
using UnityEngine.UI;
using olimsko;

public class UIHUDView : UIView
{
    private UIPlayerInfoEnitity[] m_PlayerInfoEnitities;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        m_PlayerInfoEnitities = GetComponentsInChildren<UIPlayerInfoEnitity>(true);
        GameManager.instance.OnKillMonster += OnKillMonster;
        GameManager.instance.OnKillBoss += OnKillBoss;
        GameManager.instance.OnEXPBar += OnEXPBar;

        SetPlayer();
        Init();
    }

    protected override void OnShow()
    {
        base.OnShow();
        // SetPlayer();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameManager.instance.OnKillMonster -= OnKillMonster;
        GameManager.instance.OnKillBoss -= OnKillBoss;
        GameManager.instance.OnEXPBar -= OnEXPBar;
    }

    private void Init()
    {
        Get<UITMPText>("KillCount").SetText(GameManager.instance.kill.ToString());
        Get<UITMPText>("BossKillCount").SetText($"{GameManager.instance.bossKill.ToString()}/4");
        Get<UITMPText>("LevelText").SetText(GameManager.instance.level);
        OnEXPBar(0);
    }

    public void SetPlayer()
    {
        StageContext stageContext = OSManager.GetService<ContextManager>().GetContext<StageContext>();

        for (int i = 0; i < m_PlayerInfoEnitities.Length; i++)
        {
            if (i < stageContext.ListSelectedHero.Count)
            {
                m_PlayerInfoEnitities[i].SetPlayer(stageContext.ListSelectedHero[i], i);
            }
            else
            {
                m_PlayerInfoEnitities[i].gameObject.SetActive(false);
            }
        }
    }

    void OnKillMonster(int value)
    {
        Get<UITMPText>("KillCount").SetText(value.ToString());
    }

    void OnKillBoss(int value)
    {
        Get<UITMPText>("BossKillCount").SetText(value.ToString());
    }

    private void OnEXPBar(float value)
    {
        float curExp = GameManager.instance.exp;
        float maxExp = GameManager.instance.nextExp[Mathf.Min(GameManager.instance.level, GameManager.instance.nextExp.Length - 1)];
        float val = curExp / maxExp;
        Get<UISlider>("EXPBar").SetValue(val);
        Get<UITMPText>("LevelText").SetText($"Lv. {GameManager.instance.level}");
    }
}