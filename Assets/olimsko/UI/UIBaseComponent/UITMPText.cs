using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace olimsko
{
    public class UITMPText : TextMeshProUGUI, IUIBindable
    {
        [SerializeField] private bool m_IsBind = false;
        [SerializeField] private bool m_IsUseCustomBindPath = false;
        [SerializeField] private string m_BindPath;

        public bool IsBind => m_IsBind;
        public bool IsUseCustomBindPath => m_IsUseCustomBindPath;
        public string BindPath => m_BindPath;

        public void SetText(int value)
        {
            text = value.ToString();
        }

        public void SetText(float value)
        {
            text = value.ToString();
        }
    }
}
