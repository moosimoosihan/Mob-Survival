using UnityEngine;
using UnityEngine.UI;
using olimsko;
using DG.Tweening;
using System.Collections.Generic;

public class UIPauseView : UIView
{
    private Sequence m_Sequence;
    private UICharacterInfoItem[] m_ListCharacterInfoItems;

    protected override void Awake()
    {
        base.Awake();
        m_ListCharacterInfoItems = GetComponentsInChildren<UICharacterInfoItem>();
        SetCharacterInfo();
    }

    protected override void OnShow()
    {
        base.OnShow();
        ShowPanelAnimation();

        GetEntity<UIInventory>()?.Show();

        GameManager.instance?.Stop();
    }

    protected override void OnHide()
    {
        base.OnHide();
        GetEntity<UIInventory>()?.Hide();

        GameManager.instance?.Resume();
    }

    public void ShowPanelAnimation()
    {
        if (m_Sequence != null) m_Sequence.Kill();

        m_Sequence = DOTween.Sequence();
        m_Sequence.PrependCallback(() =>
        {
            Get<UIImage>("LeftPanel").rectTransform.anchoredPosition = new Vector2(-Get<UIImage>("LeftPanel").OriginPosition.x, Get<UIImage>("LeftPanel").OriginPosition.y);
            Get<UIImage>("RightPanel").rectTransform.anchoredPosition = new Vector2(-Get<UIImage>("RightPanel").OriginPosition.x, Get<UIImage>("RightPanel").OriginPosition.y);
        });
        m_Sequence.Append(Get<UIImage>("LeftPanel").rectTransform.DOAnchorPosX(Get<UIImage>("LeftPanel").OriginPosition.x, AnimTime));
        m_Sequence.Join(Get<UIImage>("RightPanel").rectTransform.DOAnchorPosX(Get<UIImage>("RightPanel").OriginPosition.x, AnimTime));
        m_Sequence.SetUpdate(true);
        m_Sequence.Play();
    }

    public void SetCharacterInfo()
    {
        List<int> listSelectedHero = OSManager.GetService<ContextManager>().GetContext<StageContext>().ListSelectedHero;
        for (int i = 0; i < m_ListCharacterInfoItems.Length; i++)
        {
            if (i < listSelectedHero.Count)
            {
                m_ListCharacterInfoItems[i].gameObject.SetActive(true);
                m_ListCharacterInfoItems[i].SetCharacter(listSelectedHero[i]);
            }
            else
                m_ListCharacterInfoItems[i].gameObject.SetActive(false);
        }
    }

    public void ContinueBtn()
    {
        Hide();
    }

    public void ResetStage()
    {
        Hide();
        Time.timeScale = 1;
        OSManager.GetService<ContextManager>().GetContext<LoadingContext>().LoadSceneAsyncTrigger("main");
    }

    public void ExitStage()
    {
        Hide();
        Time.timeScale = 1;
        OSManager.GetService<ContextManager>().GetContext<LoadingContext>().LoadSceneAsyncTrigger("01_Main");
    }
}