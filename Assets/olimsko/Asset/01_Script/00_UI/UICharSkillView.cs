using UnityEngine;
using UnityEngine.UI;
using olimsko;
using UnityEngine.Localization.Settings;

public class UICharSkillView : UIView
{
    private int m_PlayerID = 0;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnShow()
    {
        base.OnShow();

        SetSkillInfo();
    }

    public void Show(int playerID)
    {
        m_PlayerID = playerID;
        base.Show();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }

    public async void SetSkillInfo()
    {
        CharacterTable characterTable = OSManager.GetService<DataManager>().GetData<CharacterTableSO>().CharacterTable[m_PlayerID];
        SkillTableSO skillTableSO = OSManager.GetService<DataManager>().GetData<SkillTableSO>();

        Get<UIImage>("UICharIcon").sprite = await characterTable.GetSDSprite();
        Get<UITMPText>("UICharName").SetText(LocalizationSettings.StringDatabase.GetTable("LanguageTable").GetEntry($"Str_CharacterNameKey_{characterTable.Idx}").GetLocalizedString());

        for (int i = 1; i <= 8; i++)
        {
            Get<UIImage>($"BasicSkillIcon_{i}").sprite = OSManager.GetService<UIManager>().GetTransparentImage(); ;
            Get<UITMPText>($"BasicSkillLevel_{i}").SetText("");
            Get<UITMPText>($"BasicSkillName_{i}").SetText("");
            Get<UIImage>("UniqueSkillIcon").sprite = OSManager.GetService<UIManager>().GetTransparentImage(); ;
            if (i <= 4)
            {
                Get<UIImage>($"LevelSkillIcon_{i}").sprite = OSManager.GetService<UIManager>().GetTransparentImage(); ;
                Get<UITMPText>($"LevelSkillLevel_{i}").SetText("");
                Get<UITMPText>($"LevelSkillName_{i}").SetText("");
            }
        }

        EquipedSkillData equipedSkillData = OSManager.GetService<ContextManager>().GetContext<PlayerContext>().DicPlayerEquipedSkill[m_PlayerID];

        int basicSkillCount = 0;
        int levelSkillCount = 0;

        foreach (var skill in equipedSkillData.DicEquipedSkill)
        {
            if (skillTableSO.SkillTable[skill.Value.Idx].Type == SkillType.BasicSkill)
            {
                basicSkillCount++;
                Get<UIImage>($"BasicSkillIcon_{basicSkillCount}").sprite = await skillTableSO.SkillTable[skill.Value.Idx].GetIcon();
                Get<UITMPText>($"BasicSkillLevel_{basicSkillCount}").SetText(skill.Value.Level.ToString());
                Get<UITMPText>($"BasicSkillName_{basicSkillCount}").SetText(LocalizationSettings.StringDatabase.GetTable("LanguageTable").GetEntry($"Str_SkillNameKey_{skill.Value.Idx}").GetLocalizedString());
            }
            else if (skillTableSO.SkillTable[skill.Value.Idx].Type == SkillType.LevelSkill)
            {
                levelSkillCount++;
                Get<UIImage>($"LevelSkillIcon_{levelSkillCount}").sprite = await skillTableSO.SkillTable[skill.Value.Idx].GetIcon();
                Get<UITMPText>($"LevelSkillLevel_{levelSkillCount}").SetText(skill.Value.Level.ToString());
                Get<UITMPText>($"LevelSkillName_{levelSkillCount}").SetText(LocalizationSettings.StringDatabase.GetTable("LanguageTable").GetEntry($"Str_SkillNameKey_{skill.Value.Idx}").GetLocalizedString());
            }
            else
            {
                Get<UIImage>("UniqueSkillIcon").sprite = await skillTableSO.SkillTable[skill.Value.Idx].GetIcon();
            }
        }
    }

}