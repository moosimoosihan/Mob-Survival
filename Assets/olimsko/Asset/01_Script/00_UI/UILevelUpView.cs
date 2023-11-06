using UnityEngine;
using UnityEngine.UI;
using olimsko;
using System.Collections.Generic;
using System.Linq;

public class UILevelUpView : UIView
{
    private UICharacterInfoItem[] m_ListCharacterInfoItems;
    private UILevelUpSlotItem[] m_ListLevelUpSlotItems;

    private ContextManager ContextManager => OSManager.GetService<ContextManager>();
    private DataManager DataManager => OSManager.GetService<DataManager>();

    protected override void Awake()
    {
        base.Awake();
        m_ListCharacterInfoItems = GetComponentsInChildren<UICharacterInfoItem>();
        m_ListLevelUpSlotItems = GetComponentsInChildren<UILevelUpSlotItem>();

        SetCharacterInfo();
    }

    protected override void OnShow()
    {
        base.OnShow();
        SetLevelUpSlot();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }

    public void SetCharacterInfo()
    {
        List<int> listSelectedHero = ContextManager.GetContext<StageContext>().ListSelectedHero;
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

    public async void SetLevelUpSlot()
    {
        int[] listSkillIdxs = ContextManager.GetContext<PlayerContext>().GetRandomSkill();

        for (int i = 0; i < m_ListLevelUpSlotItems.Length; i++)
        {
            if (i == 0)
            {
                int playerID = DataManager.GetData<SkillTableSO>().SkillTable[listSkillIdxs[i]].CharacterID;
                Get<UIImage>("CharThumbnail1").sprite = await DataManager.GetData<CharacterTableSO>().CharacterTable[playerID].GetLDSprite();
                Get<UITMPText>("CharName1").SetText(DataManager.GetData<CharacterTableSO>().CharacterTable[playerID].Name);
            }

            m_ListLevelUpSlotItems[i].SetSkill(i < listSkillIdxs.Length ? listSkillIdxs[i] : -1);

            if (i == 2)
            {
                int playerID = DataManager.GetData<SkillTableSO>().SkillTable[listSkillIdxs[i]].CharacterID;
                Get<UIImage>("CharThumbnail2").sprite = await DataManager.GetData<CharacterTableSO>().CharacterTable[playerID].GetLDSprite();
                Get<UITMPText>("CharName2").SetText(DataManager.GetData<CharacterTableSO>().CharacterTable[playerID].Name);
            }
        }
    }

}