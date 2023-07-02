using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class InputSetting
{
    private Dictionary<string, KeyCode> m_KeyCodeMap = new Dictionary<string, KeyCode>();

    public InputSetting()
    {
        if (m_KeyCodeMap == null)
            m_KeyCodeMap = new Dictionary<string, KeyCode>();
        else m_KeyCodeMap.Clear();
    }

    public InputSetting(Dictionary<string, KeyCode> keyCodeMap)
    {
        m_KeyCodeMap = keyCodeMap;
    }

    public Dictionary<string, KeyCode> KeyCodeMap { get => m_KeyCodeMap; set => m_KeyCodeMap = value; }
}