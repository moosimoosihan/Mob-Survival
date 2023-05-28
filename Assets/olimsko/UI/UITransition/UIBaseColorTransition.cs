using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace olimsko
{
    public class UIBaseColorTransition : UITransitionComp
    {
        [SerializeField] private Color m_DefaultColor = new Color(1, 1, 1, 1);
        [SerializeField] private Color m_HoverColor = new Color(1, 1, 1, 1);
        [SerializeField] private Color m_PressedColor = new Color(1, 1, 1, 1);
        [SerializeField] private Color m_SelectedColor = new Color(1, 1, 1, 1);
        [SerializeField] private Color m_DisableColor = new Color(1, 1, 1, 1);

        protected override void Awake()
        {
            base.Awake();
            Graphic.DOColor(m_DefaultColor, 0);
        }

        public override void OnPointerClick()
        {
            base.OnPointerClick();
            if (IsInteractable)
                Graphic.DOColor(m_SelectedColor, TransitionParent.TransitionTime).SetUpdate(TransitionParent.IgnoreTimeScale);
        }

        public override void OnPointerEnter()
        {
            base.OnPointerEnter();
            if (IsInteractable)
                Graphic.DOColor(m_HoverColor, TransitionParent.TransitionTime).SetUpdate(TransitionParent.IgnoreTimeScale);
        }

        public override void OnPointerExit()
        {
            base.OnPointerExit();
            if (IsInteractable)
                Graphic.DOColor(m_DefaultColor, TransitionParent.TransitionTime).SetUpdate(TransitionParent.IgnoreTimeScale);
        }

        public override void OnPointerDown()
        {
            base.OnPointerDown();
            if (IsInteractable)
                Graphic.DOColor(m_PressedColor, TransitionParent.TransitionTime).SetUpdate(TransitionParent.IgnoreTimeScale);
        }

        public override void OnInteractableStateChanged(bool isInteractable)
        {
            base.OnInteractableStateChanged(isInteractable);
            Graphic.DOColor(isInteractable ? m_DefaultColor : m_DisableColor, TransitionParent.TransitionTime).SetUpdate(TransitionParent.IgnoreTimeScale);
        }
    }
}