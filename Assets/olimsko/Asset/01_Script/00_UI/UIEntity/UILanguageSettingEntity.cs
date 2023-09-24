using UnityEngine;
using olimsko;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;

public class UILanguageSettingEntity : UIViewEntity
{
    SettingManager SettingManager => OSManager.GetService<SettingManager>();

    [SerializeField] private TMP_Dropdown m_LanguageDropdown;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        SetSettingData();
    }

    protected override void OnShow()
    {
        base.OnShow();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }

    private void SetSettingData()
    {
        InitLanguageDropdown();

    }

    public void RevertSetting()
    {
        // SetSettingData();
    }

    private async void InitLanguageDropdown()
    {
        await LocalizationSettings.InitializationOperation;

        List<string> options = new List<string>();

        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            options.Add(locale.name);
        }

        m_LanguageDropdown.ClearOptions();
        m_LanguageDropdown.AddOptions(options);
        m_LanguageDropdown.value = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        m_LanguageDropdown.onValueChanged.AddListener(OnScreenModeDropdownValueChanged);
    }

    private void OnScreenModeDropdownValueChanged(int value)
    {
        SettingManager.GamePlaySetting.Language = value;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[value];
    }


}