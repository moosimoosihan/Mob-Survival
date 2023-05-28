using System.Collections.Generic;
using UnityEngine;

namespace olimsko
{
    public class UITransition : MonoBehaviour, IUITransition
    {
        [SerializeField] private bool m_IgnoreTimeScale = true;
        [SerializeField] private float m_TransitionTime = 0.3f;

        private List<UITransitionComp> m_Transitions = new List<UITransitionComp>();

        public bool IgnoreTimeScale { get => m_IgnoreTimeScale; set => m_IgnoreTimeScale = value; }
        public float TransitionTime { get => m_TransitionTime; set => m_TransitionTime = value; }

        public void AddTransition(UITransitionComp transition)
        {
            m_Transitions.Add(transition);
        }

        public void OnPointerClick()
        {
            foreach (var transition in m_Transitions)
            {
                transition.OnPointerEnter();
            }
        }

        public void OnPointerDown()
        {
            foreach (var transition in m_Transitions)
            {
                transition.OnPointerDown();
            }
        }

        public void OnPointerEnter()
        {
            foreach (var transition in m_Transitions)
            {
                transition.OnPointerEnter();
            }
        }

        public void OnPointerExit()
        {
            foreach (var transition in m_Transitions)
            {
                transition.OnPointerExit();
            }
        }

        public void OnInteractableStateChanged(bool isInteractable)
        {
            foreach (var transition in m_Transitions)
            {
                transition.OnInteractableStateChanged(isInteractable);
            }
        }
    }
}