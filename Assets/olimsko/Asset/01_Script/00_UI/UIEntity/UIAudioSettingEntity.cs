using UnityEngine;
using olimsko;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIAudioSettingEntity : UIViewEntity
{
    SettingManager SettingManager => OSManager.GetService<SettingManager>();

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
        InitMasterVolumeSlider();
        InitBGMVolumeSlider();
        InitEffectVolumeSlider();
    }

    public void RevertSetting()
    {
        SetSettingData();
    }

    private void InitMasterVolumeSlider()
    {
        Get<UISlider>("MasterVolumeSlider").onValueChanged.RemoveAllListeners();
        Get<UISlider>("MasterVolumeSlider").onValueChanged.AddListener(OnMasterVolumeSliderValueChanged);
        Get<UISlider>("MasterVolumeSlider").value = SettingManager.AudioSetting.MasterVolume * 100;
    }

    private void OnMasterVolumeSliderValueChanged(float value)
    {
        SettingManager.AudioSetting.MasterVolume = value * 0.01f;
    }

    private void InitBGMVolumeSlider()
    {
        Get<UISlider>("BGMVolumeSlider").onValueChanged.RemoveAllListeners();
        Get<UISlider>("BGMVolumeSlider").onValueChanged.AddListener(OnBGMVolumeSliderValueChanged);
        Get<UISlider>("BGMVolumeSlider").value = SettingManager.AudioSetting.BGMVolume * 100;
    }

    private void OnBGMVolumeSliderValueChanged(float value)
    {
        SettingManager.AudioSetting.BGMVolume = value * 0.01f;
    }

    private void InitEffectVolumeSlider()
    {
        Get<UISlider>("EffectVolumeSlider").onValueChanged.RemoveAllListeners();
        Get<UISlider>("EffectVolumeSlider").onValueChanged.AddListener(OnEffectVolumeSliderValueChanged);
        Get<UISlider>("EffectVolumeSlider").value = SettingManager.AudioSetting.EffectVolume * 100;
    }

    private void OnEffectVolumeSliderValueChanged(float value)
    {
        SettingManager.AudioSetting.EffectVolume = value * 0.01f;
    }
}