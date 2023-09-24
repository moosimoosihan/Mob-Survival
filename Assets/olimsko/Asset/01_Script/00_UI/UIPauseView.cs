using UnityEngine;
using UnityEngine.UI;
using olimsko;
using DG.Tweening;

public class UIPauseView : UIView
{
    private Sequence m_Sequence;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnShow()
    {
        base.OnShow();
        ShowPanelAnimation();

        GetEntity<UIInventory>()?.Show();
    }

    protected override void OnHide()
    {
        base.OnHide();
        GetEntity<UIInventory>()?.Hide();
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
    }
}