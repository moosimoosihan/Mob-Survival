using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace olimsko
{
    public class UIBaseScaleTransition : UITransitionComp
    {
        [SerializeField] private Vector3 m_DefaultScale = Vector3.one;
        [SerializeField] private Vector3 m_HoverScale = Vector3.one;
        [SerializeField] private Vector3 m_PressedScale = Vector3.one;
        [SerializeField] private Vector3 m_SelectedScale = Vector3.one;
        [SerializeField] private Vector3 m_DisableScale = Vector3.one;

        protected override void Awake()
        {
            base.Awake();
            Graphic.rectTransform.DOScale(m_DefaultScale, 0);
        }

        public override void OnPointerClick()
        {
            base.OnPointerClick();
            Graphic.rectTransform.DOScale(m_SelectedScale, TransitionParent.TransitionTime).SetUpdate(TransitionParent.IgnoreTimeScale);
        }

        public override void OnPointerEnter()
        {
            base.OnPointerEnter();
            Graphic.rectTransform.DOScale(m_HoverScale, TransitionParent.TransitionTime).SetUpdate(TransitionParent.IgnoreTimeScale);
        }

        public override void OnPointerExit()
        {
            base.OnPointerExit();
            Graphic.rectTransform.DOScale(m_DefaultScale, TransitionParent.TransitionTime).SetUpdate(TransitionParent.IgnoreTimeScale);
        }

        public override void OnPointerDown()
        {
            base.OnPointerDown();
            Graphic.rectTransform.DOScale(m_PressedScale, TransitionParent.TransitionTime).SetUpdate(TransitionParent.IgnoreTimeScale);
        }

        public override void OnInteractableStateChanged(bool isInteractable)
        {
            base.OnInteractableStateChanged(isInteractable);
            Graphic.rectTransform.DOScale(isInteractable ? m_DefaultScale : m_DisableScale, TransitionParent.TransitionTime).SetUpdate(TransitionParent.IgnoreTimeScale);
        }
    }
}