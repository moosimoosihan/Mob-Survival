using UnityEngine;

namespace olimsko
{
    public interface IUIViewEntity
    {
        string Name { get; }

        void Show();
        void Hide();
    }
}