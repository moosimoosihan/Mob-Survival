using UnityEngine;
using UnityEngine.UI;
using olimsko;
using System.Collections.Generic;
using System.Linq;

public class UISelectHeroView : UIView
{
    [SerializeField] private List<UIHeroSelectItem> m_ListSelectableHeroItem = new List<UIHeroSelectItem>();
    [SerializeField] private List<UIHeroSelectItem> m_ListSelectedHeroItem = new List<UIHeroSelectItem>();

    [SerializeField] private GameObject m_StageInfoIconItemPrefab;

    private StageContext StageContext => OSManager.GetService<ContextManager>().GetContext<StageContext>();

    private int m_SelectedHeroIndex = 0;

    protected override void Awake()
    {
        base.Awake();

        StageContext.OnSelectedHeroChanged += OnSelectedHeroChanged;

        Get<UIButton>("StageStartButton").onClick.AddListener(() =>
        {
            OSManager.GetService<ContextManager>().GetContext<LoadingContext>().LoadSceneAsyncTrigger("main");
        });
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        StageContext.OnSelectedHeroChanged -= OnSelectedHeroChanged;

    }

    protected override void OnShow()
    {
        base.OnShow();
        InitializeData();

        Debug.Log("SelectedChapter : " + OSManager.GetService<ContextManager>().GetContext<StageContext>().SelectedChapter);
        Debug.Log("SelectedStage : " + OSManager.GetService<ContextManager>().GetContext<StageContext>().SelectedStage);
        Debug.Log("SelectedDifficulty : " + OSManager.GetService<ContextManager>().GetContext<StageContext>().SelectedDifficulty);
    }

    protected override void OnHide()
    {
        base.OnHide();
    }

    public void OnClickHeroSelect()
    {
        if (m_SelectedHeroIndex >= 0)
        {
            StageContext.AddSelectedHero(m_SelectedHeroIndex);
            CheckButtonState();
        }
    }

    public void OnClickHeroCancel()
    {
        if (m_SelectedHeroIndex >= 0)
        {
            StageContext.RemoveSelectedHero(m_SelectedHeroIndex);
            CheckButtonState();
        }
    }

    public void OnToggleSelectableHero(bool isOn, int index)
    {
        if (isOn)
        {
            m_SelectedHeroIndex = index;
            CheckButtonState();
            SetCharInfo();

        }
    }

    private void CheckButtonState()
    {
        Get<UIButton>("SelectButton").interactable = !StageContext.ListSelectedHero.Contains(m_SelectedHeroIndex);
        Get<UIButton>("CancelButton").interactable = StageContext.ListSelectedHero.Contains(m_SelectedHeroIndex);

        if (StageContext.ListSelectedHero.Count >= m_ListSelectedHeroItem.Count)
        {
            Get<UIButton>("SelectButton").interactable = false;
        }

        Get<UIButton>("StageStartButton").interactable = StageContext.ListSelectedHero.Count >= 1;
    }

    private void InitializeData()
    {
        m_SelectedHeroIndex = 0;

        StageContext.ResetSelectedHero();

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

        for (int i = 0; i < m_ListSelectedHeroItem.Count; i++)
        {
            m_ListSelectedHeroItem[i].SetData(null, isOnlyShow: true);
        }

        CheckButtonState();
        SetCharInfo();
    }

    private async void SetCharInfo()
    {
        if (m_SelectedHeroIndex < 0)
        {
            Get<UIImage>("CharInfoImage").sprite = null;
            Get<UITMPText>("CharInfoName").text = "";
            return;
        }

        Get<UIImage>("CharInfoImage").sprite = await StageContext.GetCharacterTable(StageContext.ListSelectableHero[m_SelectedHeroIndex]).GetSDSprite();
        Get<UITMPText>("CharInfoName").text = StageContext.GetCharacterTable(StageContext.ListSelectableHero[m_SelectedHeroIndex]).Name;
    }

    private void OnSelectedHeroChanged()
    {
        for (int i = 0; i < m_ListSelectedHeroItem.Count; i++)
        {
            if (StageContext.ListSelectedHero.Count > i)
                m_ListSelectedHeroItem[i].SetData(StageContext.GetCharacterTable(StageContext.ListSelectedHero[i]), isOnlyShow: true);
            else
                m_ListSelectedHeroItem[i].SetData(null, isOnlyShow: true);
        }
    }
}