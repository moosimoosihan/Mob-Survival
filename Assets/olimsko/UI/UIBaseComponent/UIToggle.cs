using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace olimsko
{
    public class UIToggle : Toggle, IUIBindable
    {
        [SerializeField] private UITransition m_UITransition;
        [SerializeField] private bool m_IsBind = false;
        [SerializeField] private bool m_IsUseCustomBindPath = false;
        [SerializeField] private string m_BindPath;

        private SelectionState prevState;

        public bool IsBind => m_IsBind;
        public bool IsUseCustomBindPath => m_IsUseCustomBindPath;
        public string BindPath => m_BindPath;

        public RectTransform RectTransform => targetGraphic.rectTransform;

        protected override void Awake()
        {
            base.Awake();
            if (!Application.isPlaying) return;

            if (m_UITransition == null)
                m_UITransition = GetComponent<UITransition>();
        }

        protected override void Start()
        {
            base.Start();
            if (m_UITransition != null)
            {
                transition = Transition.None;
                OnInteractableChanged(interactable);
            }
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            if (state != prevState)
            {
                if (state.Equals(SelectionState.Disabled))
                    OnInteractableChanged(false);

                if (!state.Equals(SelectionState.Disabled) && prevState.Equals(SelectionState.Disabled))
                {
                    OnInteractableChanged(true);
                }

                prevState = state;
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (m_UITransition != null)
                m_UITransition.OnPointerEnter();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (m_UITransition != null)
                m_UITransition.OnPointerDown();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            if (m_UITransition != null)
                m_UITransition.OnPointerExit();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (m_UITransition != null)
                m_UITransition.OnPointerClick();
        }

        public virtual void OnInteractableChanged(bool isEnable)
        {
            if (m_UITransition != null)
                m_UITransition.OnInteractableStateChanged(isEnable);
        }
    }
}

