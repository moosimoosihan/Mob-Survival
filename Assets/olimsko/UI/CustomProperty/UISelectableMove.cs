using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace olimsko
{
    [System.Serializable]
    public struct UISelectableMove : IEquatable<UISelectableMove>
    {
        [SerializeField] private bool m_UseSelectableMove;
        [SerializeField] private Vector2 m_HoverMoveLocal;
        [SerializeField] private Vector2 m_PressedMoveLocal;

        public UISelectableMove(bool useSelectableMove, Vector2 hoverMoveLocal, Vector2 pressedMoveLocal)
        {
            m_UseSelectableMove = useSelectableMove;
            m_HoverMoveLocal = hoverMoveLocal;
            m_PressedMoveLocal = pressedMoveLocal;
        }

        public bool UseSelectableMove { get => m_UseSelectableMove; set => m_UseSelectableMove = value; }
        public Vector2 HoverMoveLocal { get => m_HoverMoveLocal; set => m_HoverMoveLocal = value; }
        public Vector2 PressedMoveLocal { get => m_PressedMoveLocal; set => m_PressedMoveLocal = value; }

        public override bool Equals(object obj)
        {
            return obj is UISelectableMove variable && Equals(variable);
        }

        public bool Equals(UISelectableMove other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return EqualityComparer<bool>.Default.GetHashCode(UseSelectableMove);
        }

        public static bool operator ==(UISelectableMove left, UISelectableMove right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(UISelectableMove left, UISelectableMove right)
        {
            return !(left == right);
        }
    }
}

