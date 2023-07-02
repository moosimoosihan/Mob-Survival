using UnityEngine;
using System;

[System.Serializable]
public class VideoSetting
{
    private FullScreenMode m_ScreenMode;
    private Vector2Int m_Resolution;
    private int m_RefreshRate;
    private int m_QualityLevel;
    private bool m_VSync;
    private int m_ShadowQuality;
    private int m_AntiAliasing;

    public VideoSetting()
    {
        m_ScreenMode = Screen.fullScreenMode;
        m_Resolution = new Vector2Int(Screen.width, Screen.height);
        m_RefreshRate = 60;
        m_QualityLevel = QualitySettings.GetQualityLevel();
        m_VSync = QualitySettings.vSyncCount == 1;
        ShadowQuality = QualitySettings.shadows == UnityEngine.ShadowQuality.Disable ? 0 : QualitySettings.shadows == UnityEngine.ShadowQuality.HardOnly ? 1 : 2;
        AntiAliasing = QualitySettings.antiAliasing;
    }

    public VideoSetting(FullScreenMode screenMode, Vector2Int resolution, int refreshRate, int qualityLevel, bool vSync, int shadowQuality, int antiAliasing)
    {
        m_ScreenMode = screenMode;
        m_Resolution = resolution;
        m_RefreshRate = refreshRate;
        m_QualityLevel = qualityLevel;
        m_VSync = vSync;
        m_ShadowQuality = shadowQuality;
        m_AntiAliasing = antiAliasing;
    }

    public FullScreenMode ScreenMode { get => m_ScreenMode; set => m_ScreenMode = value; }
    public Vector2Int Resolution { get => m_Resolution; set => m_Resolution = value; }
    public int RefreshRate { get => m_RefreshRate; set => m_RefreshRate = value; }
    public int QualityLevel { get => m_QualityLevel; set => m_QualityLevel = value; }
    public bool VSync { get => m_VSync; set => m_VSync = value; }
    public int ShadowQuality { get => m_ShadowQuality; set => m_ShadowQuality = value; }
    public int AntiAliasing { get => m_AntiAliasing; set => m_AntiAliasing = value; }
}