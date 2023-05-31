using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace olimsko
{
    [System.Serializable]
    public struct UISelectableSprite : IEquatable<UISelectableSprite>
    {
        [SerializeField] private bool m_UseSelectableSprite;
        [SerializeField] private Sprite m_HoverSprite;
        [SerializeField] private Sprite m_PressedSprite;
        [SerializeField] private Sprite m_DisableSprite;

        public UISelectableSprite(bool useSelectableSprite, Sprite hoverSprite, Sprite pressedSprite, Sprite disableSprite)
        {
            m_UseSelectableSprite = useSelectableSprite;
            m_HoverSprite = hoverSprite;
            m_PressedSprite = pressedSprite;
            m_DisableSprite = disableSprite;
        }

        public bool UseSelectableSprite { get => m_UseSelectableSprite; set => m_UseSelectableSprite = value; }
        public Sprite HoverSprite { get => m_HoverSprite; set => m_HoverSprite = value; }
        public Sprite PressedSprite { get => m_PressedSprite; set => m_PressedSprite = value; }
        public Sprite DisableSprite { get => m_DisableSprite; set => m_DisableSprite = value; }

        public override bool Equals(object obj)
        {
            return obj is UISelectableSprite variable && Equals(variable);
        }

        public bool Equals(UISelectableSprite other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return EqualityComparer<bool>.Default.GetHashCode(m_HoverSprite);
        }

        public static bool operator ==(UISelectableSprite left, UISelectableSprite right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(UISelectableSprite left, UISelectableSprite right)
        {
            return !(left == right);
        }
    }
}

