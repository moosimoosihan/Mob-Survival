using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using olimsko;

public class UILevelUpSlotItem : MonoBehaviour
{
    [SerializeField] private UITMPText m_NameText;
    [SerializeField] private UITMPText m_DescText;
    [SerializeField] private UITMPText m_TypeText;
    [SerializeField] private UIButton m_Button;

    private SkillTable m_skillTable = null;

    SkillTableSO SkillTableSO => OSManager.GetService<DataManager>().GetData<SkillTableSO>();

    public void SetSkill(int idx)
    {
        if (idx < 0)
        {
            m_NameText.SetText("");
            m_DescText.SetText("");
            m_TypeText.SetText("");
            m_Button.enabled = false;
            return;
        }

        m_Button.enabled = true;
        SkillTable m_SkillTable = SkillTableSO.SkillTable[idx];
        m_NameText.SetText(LocalizationSettings.StringDatabase.GetTable("LanguageTable").GetEntry(m_SkillTable.Name).GetLocalizedString());
        m_DescText.SetText(LocalizationSettings.StringDatabase.GetTable("LanguageTable").GetEntry(m_SkillTable.Desc).GetLocalizedString());
        m_TypeText.SetText(LocalizationSettings.StringDatabase.GetTable("LanguageTable").GetEntry($"Str_{m_SkillTable.Type.ToString()}").GetLocalizedString());

        m_skillTable = m_SkillTable;
    }

    public void OnClickLevelUp()
    {
        if (m_skillTable == null) return;

        OSManager.GetService<ContextManager>().GetContext<PlayerContext>().AddEquipedSkill(m_skillTable.CharacterID, m_skillTable.Idx);
        OSManager.GetService<UIManager>().GetUI<UILevelUpView>().Hide();
    }
}
