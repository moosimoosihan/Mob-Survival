using UnityEngine;
using UnityEngine.UI;

namespace olimsko
{
    public abstract class UITransitionComp : MonoBehaviour, IUITransitionComp
    {
        [SerializeField] private UITransition m_TransitionParent;
        [SerializeField] private Graphic m_Graphic;

        public Graphic Graphic { get => m_Graphic; set => m_Graphic = value; }
        public UITransition TransitionParent { get => m_TransitionParent; set => m_TransitionParent = value; }

        protected bool IsInteractable { get; private set; } = true;

        protected virtual void Awake()
        {
            if (TransitionParent == null)
                TransitionParent = GetComponent<UITransition>();
            if (m_Graphic == null)
                m_Graphic = GetComponent<Graphic>();

            if (TransitionParent != null)
                TransitionParent.AddTransition(this);
        }

        public virtual void OnPointerClick()
        {

        }

        public virtual void OnPointerDown()
        {

        }

        public virtual void OnPointerEnter()
        {

        }

        public virtual void OnPointerExit()
        {

        }

        public virtual void OnInteractableStateChanged(bool isInteractable)
        {
            IsInteractable = isInteractable;
        }
    }
}