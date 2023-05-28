using UnityEngine;
using UnityEngine.UI;

namespace olimsko
{
    public interface IUITransition
    {
        bool IgnoreTimeScale { get; set; }
        float TransitionTime { get; set; }
        void OnPointerClick();
        void OnPointerDown();
        void OnPointerEnter();
        void OnPointerExit();
        void OnInteractableStateChanged(bool isInteractable);
    }
}