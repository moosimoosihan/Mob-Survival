using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace olimsko
{
    public class UISlider : Slider, IUIBindable
    {
        [SerializeField] private bool m_IsBind = false;
        [SerializeField] private bool m_IsUseCustomBindPath = false;
        [SerializeField] private string m_BindPath;

        public bool IsBind => m_IsBind;
        public bool IsUseCustomBindPath => m_IsUseCustomBindPath;
        public string BindPath => m_BindPath;

        public void SetValue(float value, float maxValue = -1)
        {
            if (maxValue != -1)
            {
                this.maxValue = maxValue;
            }
            this.DOValue(value, 0.5f).SetUpdate(true);
        }
    }
}

