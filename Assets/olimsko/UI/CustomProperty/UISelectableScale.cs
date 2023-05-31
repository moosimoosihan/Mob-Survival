using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace olimsko
{
    [System.Serializable]
    public struct UISelectableScale : IEquatable<UISelectableScale>
    {
        [SerializeField] private bool m_UseSelectableScale;
        [SerializeField] private float m_HoverScaleValue;
        [SerializeField] private AnimationCurve m_HoverCurve;
        [SerializeField] private float m_PressedScaleValue;
        [SerializeField] private AnimationCurve m_PressedCurve;

        public UISelectableScale(bool useSelectableScale, float hoverScaleValue, AnimationCurve hoverCurve, float pressedScaleValue, AnimationCurve pressedCurve)
        {
            m_UseSelectableScale = useSelectableScale;
            m_HoverScaleValue = hoverScaleValue;
            m_HoverCurve = hoverCurve;
            m_PressedScaleValue = pressedScaleValue;
            m_PressedCurve = pressedCurve;
        }

        public bool UseSelectableScale { get => m_UseSelectableScale; set => m_UseSelectableScale = value; }
        public float HoverScaleValue { get => m_HoverScaleValue; set => m_HoverScaleValue = value; }
        public AnimationCurve HoverCurve { get => m_HoverCurve; set => m_HoverCurve = value; }
        public float PressedScaleValue { get => m_PressedScaleValue; set => m_PressedScaleValue = value; }
        public AnimationCurve PressedCurve { get => m_PressedCurve; set => m_PressedCurve = value; }

        public override bool Equals(object obj)
        {
            return obj is UISelectableScale variable && Equals(variable);
        }

        public bool Equals(UISelectableScale other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return EqualityComparer<bool>.Default.GetHashCode(m_UseSelectableScale);
        }

        public static bool operator ==(UISelectableScale left, UISelectableScale right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(UISelectableScale left, UISelectableScale right)
        {
            return !(left == right);
        }
    }
}

