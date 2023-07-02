using Cysharp.Threading.Tasks;
using UnityEngine;
// using UnityEngine.Localization;
// using UnityEngine.Localization.Settings;

public class GamePlaySetting
{
    private string m_Language;

    public GamePlaySetting()
    {
        Language = "en";
        // Language = LocalizationSettings.SelectedLocale.Identifier.Code;
    }

    public GamePlaySetting(string language)
    {
        Language = language;
    }

    public string Language { get => m_Language; set => m_Language = value; }
}