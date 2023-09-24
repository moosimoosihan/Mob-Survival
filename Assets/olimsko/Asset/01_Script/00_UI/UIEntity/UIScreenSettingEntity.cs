using UnityEngine;
using olimsko;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;
using TMPro;

public class UIScreenSettingEntity : UIViewEntity
{
    SettingManager SettingManager => OSManager.GetService<SettingManager>();

    [SerializeField] private TMP_Dropdown m_ScreenModeDropdown;
    [SerializeField] private TMP_Dropdown m_ResolutionDropdown;

    private List<Resolution> m_ListResolutions;

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
        InitScreenModeDropdown();
        InitResolutionDropdown();
    }

    public void RevertSetting()
    {
        // SetSettingData();
    }

    private void InitScreenModeDropdown()
    {
        m_ScreenModeDropdown.ClearOptions();
        List<string> options = Enum.GetNames(typeof(FullScreenMode)).ToList();

        m_ScreenModeDropdown.AddOptions(options);
        m_ScreenModeDropdown.value = (int)SettingManager.VideoSetting.ScreenMode;
        m_ScreenModeDropdown.onValueChanged.RemoveAllListeners();
        m_ScreenModeDropdown.onValueChanged.AddListener(OnScreenModeDropdownValueChanged);
    }

    private void OnScreenModeDropdownValueChanged(int value)
    {
        SettingManager.VideoSetting.ScreenMode = (FullScreenMode)value;
        Screen.fullScreenMode = SettingManager.VideoSetting.ScreenMode;
    }

    private void InitResolutionDropdown()
    {
        m_ResolutionDropdown.ClearOptions();

        m_ListResolutions = Screen.resolutions.Where(res => res.width >= 1280 && res.height >= 720).ToList();
        List<string> options = Screen.resolutions
            .Where(res => res.width >= 1280 && res.height >= 720)
            .Select(res => res.width + "x" + res.height)
            .ToList();
        m_ResolutionDropdown.AddOptions(options);
        m_ResolutionDropdown.value = m_ListResolutions.FindIndex(res => res.width == Screen.width && res.height == Screen.height);
        m_ResolutionDropdown.onValueChanged.RemoveAllListeners();
        m_ResolutionDropdown.onValueChanged.AddListener(OnResolutionDropdownValueChanged);
    }

    private void OnResolutionDropdownValueChanged(int value)
    {
        SettingManager.VideoSetting.Resolution = new Vector2Int(m_ListResolutions[value].width, m_ListResolutions[value].height);
        Screen.SetResolution(SettingManager.VideoSetting.Resolution.x, SettingManager.VideoSetting.Resolution.y, SettingManager.VideoSetting.ScreenMode);
    }

    private int GetResolutionIndex()
    {
        return Screen.resolutions.ToList().FindIndex(res => res.width == Screen.width && res.height == Screen.height);
    }

}