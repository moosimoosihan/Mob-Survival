using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace olimsko
{
    [System.Serializable]
    public struct UISelectableColor : IEquatable<UISelectableColor>
    {
        [SerializeField] private bool m_UseSelectableColor;
        [SerializeField] private bool m_UseSelected;
        [SerializeField] private Color m_DefaultColor;
        [SerializeField] private Color m_HoverColor;
        [SerializeField] private Color m_PressedColor;
        [SerializeField] private Color m_SelectedColor;
        [SerializeField] private Color m_DisableColor;

        public UISelectableColor(bool useSelectableColor = default, bool useSelected = default, Color defaultColor = default, Color hoverColor = default, Color pressedColor = default, Color selectedColor = default, Color disableColor = default)
        {
            m_UseSelectableColor = useSelectableColor;
            m_UseSelected = useSelected;
            m_DefaultColor = defaultColor;
            m_HoverColor = hoverColor;
            m_PressedColor = pressedColor;
            m_SelectedColor = selectedColor;
            m_DisableColor = disableColor;
        }

        public bool UseSelectableColor { get => m_UseSelectableColor; set => m_UseSelectableColor = value; }
        public bool UseSelected { get => m_UseSelected; set => m_UseSelected = value; }
        public Color DefaultColor { get => m_DefaultColor; set => m_DefaultColor = value; }
        public Color HoverColor { get => m_HoverColor; set => m_HoverColor = value; }
        public Color PressedColor { get => m_PressedColor; set => m_PressedColor = value; }
        public Color SelectedColor { get => m_SelectedColor; set => m_SelectedColor = value; }
        public Color DisableColor { get => m_DisableColor; set => m_DisableColor = value; }

        public override bool Equals(object obj)
        {
            return obj is UISelectableColor variable && Equals(variable);
        }

        public bool Equals(UISelectableColor other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return EqualityComparer<Color>.Default.GetHashCode(DefaultColor);
        }

        public static bool operator ==(UISelectableColor left, UISelectableColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(UISelectableColor left, UISelectableColor right)
        {
            return !(left == right);
        }
    }
}

