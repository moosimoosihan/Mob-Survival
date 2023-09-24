using Cysharp.Threading.Tasks;
using UnityEngine;
// using UnityEngine.Localization;
// using UnityEngine.Localization.Settings;

public class GamePlaySetting
{
    private int m_Language;

    public GamePlaySetting()
    {
        Language = 0;
        // Language = LocalizationSettings.SelectedLocale.Identifier.Code;
    }

    public GamePlaySetting(int language)
    {
        Language = language;
    }

    public int Language { get => m_Language; set => m_Language = value; }
}