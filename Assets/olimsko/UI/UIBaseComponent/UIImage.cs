using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace olimsko
{
    public class UIImage : Image, IUIBindable
    {
        [SerializeField] private bool m_IsBind = false;
        [SerializeField] private bool m_IsUseCustomBindPath = false;
        [SerializeField] private string m_BindPath;

        private Vector3 m_OriginPosition;

        protected override void Awake()
        {
            base.Awake();
            OriginPosition = rectTransform.anchoredPosition;
        }

        public bool IsBind => m_IsBind;
        public bool IsUseCustomBindPath => m_IsUseCustomBindPath;
        public string BindPath => m_BindPath;

        public Vector3 OriginPosition { get => m_OriginPosition; set => m_OriginPosition = value; }
    }

}
