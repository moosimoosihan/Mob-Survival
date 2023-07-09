using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace olimsko
{
    public class UITMPMatTransition : UITransitionComp
    {
        private UITMPText m_TextMeshProUGUI;
        [SerializeField] private Material m_DefaultMat;
        [SerializeField] private Material m_HoverMat;
        [SerializeField] private Material m_PressedMat;
        [SerializeField] private Material m_SelectedMat;
        [SerializeField] private Material m_DisableMat;

        protected override void Awake()
        {
            m_TextMeshProUGUI = GetComponent<UITMPText>();
            base.Awake();
            m_TextMeshProUGUI.fontMaterial = m_DefaultMat;
        }

        public override void OnPointerClick()
        {
            base.OnPointerClick();
            m_TextMeshProUGUI.fontMaterial = m_SelectedMat ?? m_DefaultMat;
        }

        public override void OnPointerEnter()
        {
            base.OnPointerEnter();
            m_TextMeshProUGUI.fontMaterial = m_HoverMat ?? m_DefaultMat;
        }

        public override void OnPointerExit()
        {
            base.OnPointerExit();
            m_TextMeshProUGUI.fontMaterial = m_DefaultMat;
        }

        public override void OnPointerDown()
        {
            base.OnPointerDown();
            m_TextMeshProUGUI.fontMaterial = m_PressedMat ?? m_DefaultMat;
        }

        public override void OnInteractableStateChanged(bool isInteractable)
        {
            base.OnInteractableStateChanged(isInteractable);
            m_TextMeshProUGUI.fontMaterial = isInteractable ? m_DefaultMat : m_DisableMat ?? m_DefaultMat;
        }
    }
}