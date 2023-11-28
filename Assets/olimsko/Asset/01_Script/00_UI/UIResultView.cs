using UnityEngine;
using UnityEngine.UI;
using olimsko;
using System.Collections.Generic;
using System;

public class UIResultView : UIView
{
    [SerializeField] private GameObject[] m_ResultObject;
    [SerializeField] private List<UIResultPlayerInfo> m_ListPlayerInfo = new List<UIResultPlayerInfo>();

    private PlayerContext PlayerContext => OSManager.GetService<ContextManager>().GetContext<PlayerContext>();
    private StageContext StageContext => OSManager.GetService<ContextManager>().GetContext<StageContext>();

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnShow()
    {
        base.OnShow();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }

    private void SetResult()
    {
        for (int i = 0; i < StageContext.ListSelectedHero.Count; i++)
        {
            int playerID = StageContext.ListSelectedHero[i];
            m_ListPlayerInfo[i].SetPlayerInfo(playerID);
        }

        Get<UITMPText>("PlayTime").text = TimeSpan.FromSeconds(GameManager.instance.GameTime).ToString(@"mm\:ss");
        Get<UITMPText>("KillCount").text = GameManager.instance.kill.ToString();
        Get<UITMPText>("BossKillCount").text = GameManager.instance.bossKill.ToString();

    }
}