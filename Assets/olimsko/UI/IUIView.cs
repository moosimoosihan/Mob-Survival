using System;
using UnityEngine;

namespace olimsko
{
    public interface IUIView
    {
        bool IsDependenceInScene { get; }
        event Action<bool> OnVisibilityChanged;

        bool IsVisible { get; set; }

        Camera RenderCamera { get; set; }

        bool IsInteractable { get; set; }

        GameObject GameObject { get; }

        void DestroyThis();
    }
}
