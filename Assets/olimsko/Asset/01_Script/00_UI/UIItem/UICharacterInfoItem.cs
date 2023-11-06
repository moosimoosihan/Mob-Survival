using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using olimsko;

public class UICharacterInfoItem : MonoBehaviour
{
    [SerializeField] private UIImage m_CharacterImage;
    [SerializeField] private UITMPText m_NameText;

    CharacterTableSO CharacterTableSO => OSManager.GetService<DataManager>().GetData<CharacterTableSO>();
    private CharacterTable m_CharacterTable = null;

    public async void SetCharacter(int id)
    {
        m_CharacterTable = CharacterTableSO.CharacterTable[id];

        m_NameText.SetText(LocalizationSettings.StringDatabase.GetTable("LanguageTable").GetEntry($"Str_CharacterNameKey_{m_CharacterTable.Idx}").GetLocalizedString());
        m_CharacterImage.sprite = await m_CharacterTable.GetLDSprite();
    }

    public void ShowStatus()
    {
        // OSManager.GetService<UIManager>().ShowPopup<UICharacterStatusPopup>(m_CharacterTable);
    }

    public void ShowSkill()
    {
        OSManager.GetService<UIManager>().GetUI<UICharSkillView>().Show(m_CharacterTable.Idx);
    }
}
