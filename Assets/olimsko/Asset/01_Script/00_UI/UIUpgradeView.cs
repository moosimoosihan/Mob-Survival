using UnityEngine;
using UnityEngine.UI;
using olimsko;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;

public class UIUpgradeView : UIView
{
    [SerializeField] private List<UIUpgradeHeroSelectItem> m_ListSelectableHeroItem = new List<UIUpgradeHeroSelectItem>();
    [SerializeField] private List<UIStatUpgradeItem> m_ListStatUpgradeItem = new List<UIStatUpgradeItem>();
    [SerializeField] private List<Sprite> m_ListCharSprite = new List<Sprite>();
    [SerializeField] private List<Sprite> m_ListCharBGSprite = new List<Sprite>();

    private StageContext StageContext => OSManager.GetService<ContextManager>().GetContext<StageContext>();

    private Sequence m_Sequence;
    private int m_SelectedHeroIndex = 0;

    protected override void Awake()
    {
        base.Awake();
        m_ListStatUpgradeItem = GetComponentsInChildren<UIStatUpgradeItem>().ToList();
    }

    protected override void OnShow()
    {
        base.OnShow();
        InitializeData();
    }

    protected override void OnHide()
    {
        base.OnHide();
        if (m_Sequence != null) m_Sequence.Kill();
    }

    private void InitializeData()
    {
        m_SelectedHeroIndex = 0;

        m_ListSelectableHeroItem[0].Toggle.isOn = true;

        for (int i = 0; i < m_ListSelectableHeroItem.Count; i++)
        {
            if (i == 0) m_ListSelectableHeroItem[i].Toggle.isOn = true;
            if (i < StageContext.ListSelectableHero.Count)
            {
                int index = i;
                m_ListSelectableHeroItem[i].SetData(StageContext.GetCharacterTable(StageContext.ListSelectableHero[index]), isLock: false);
                m_ListSelectableHeroItem[i].Toggle.onValueChanged.RemoveAllListeners();
                m_ListSelectableHeroItem[i].Toggle.onValueChanged.AddListener((isOn) => OnToggleSelectableHero(isOn, index));
            }
            else
                m_ListSelectableHeroItem[i].SetData(null, isLock: true);
        }

        CharSpriteAnimation();

        for (int i = 0; i < m_ListStatUpgradeItem.Count; i++)
        {
            m_ListStatUpgradeItem[i].SetCharacterStatAsync(StageContext.ListSelectableHero[m_SelectedHeroIndex]);
        }
    }

    public void OnToggleSelectableHero(bool isOn, int index)
    {
        if (isOn)
        {
            m_SelectedHeroIndex = index;
        }
        CharSpriteAnimation();
    }

    public void CharSpriteAnimation()
    {
        if (m_Sequence != null) m_Sequence.Kill();

        m_Sequence = DOTween.Sequence();
        m_Sequence.PrependCallback(() =>
        {
            Get<UIImage>("CharImage").sprite = m_ListCharSprite[m_SelectedHeroIndex];
            Get<UIImage>("CharImage").color = new Color(1f, 1f, 1f, 0f);
            Get<UIImage>("CharImage").rectTransform.anchoredPosition = new Vector2(Get<UIImage>("CharImage").OriginPosition.x + 400f, Get<UIImage>("CharImage").OriginPosition.y);

            Get<UIImage>("CharImageBG").sprite = m_ListCharBGSprite[m_SelectedHeroIndex];
            Get<UIImage>("CharImageBG").color = new Color(1f, 1f, 1f, 0f);
            Get<UIImage>("CharImageBG").rectTransform.anchoredPosition = new Vector2(Get<UIImage>("CharImageBG").OriginPosition.x - 300f, Get<UIImage>("CharImageBG").OriginPosition.y);
        });
        m_Sequence.Append(Get<UIImage>("CharImage").DOFade(1f, 0.5f));
        m_Sequence.Join(Get<UIImage>("CharImage").rectTransform.DOAnchorPosX(Get<UIImage>("CharImage").OriginPosition.x, 0.5f));
        m_Sequence.Insert(0.2f, Get<UIImage>("CharImageBG").DOFade(1f, 0.5f));
        m_Sequence.Insert(0.2f, Get<UIImage>("CharImageBG").rectTransform.DOAnchorPosX(Get<UIImage>("CharImageBG").OriginPosition.x, 0.5f));
    }

}