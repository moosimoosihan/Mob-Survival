using UnityEngine;

namespace olimsko
{
    public interface IUITransitionComp
    {
        void OnPointerClick();
        void OnPointerDown();
        void OnPointerEnter();
        void OnPointerExit();
        void OnInteractableStateChanged(bool isInteractable);
    }
}