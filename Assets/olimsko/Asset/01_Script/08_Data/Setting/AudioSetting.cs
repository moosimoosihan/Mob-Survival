using UnityEngine;

public class AudioSetting
{
    private bool m_Mute;
    private float m_MasterVolume;
    private float m_BGMVolume;
    private float m_EffectVolume;
    private float m_UIVolume;

    public AudioSetting()
    {
        Mute = false;
        MasterVolume = 1f;
        BGMVolume = 1f;
        EffectVolume = 1f;
        UIVolume = 1f;
    }

    public AudioSetting(bool mute, float masterVolume, float bGMVolume, float effectVolume, float uIVolume)
    {
        m_Mute = mute;
        m_MasterVolume = masterVolume;
        m_BGMVolume = bGMVolume;
        m_EffectVolume = effectVolume;
        m_UIVolume = uIVolume;
    }

    public bool Mute { get => m_Mute; set => m_Mute = value; }
    public float MasterVolume { get => m_MasterVolume; set => m_MasterVolume = value; }
    public float BGMVolume { get => m_BGMVolume; set => m_BGMVolume = value; }
    public float EffectVolume { get => m_EffectVolume; set => m_EffectVolume = value; }
    public float UIVolume { get => m_UIVolume; set => m_UIVolume = value; }
}