// Copyright 2017-2021 Elringus (Artyom Sovetnikov). All rights reserved.

using System;
using JetBrains.Annotations;
using UnityEngine;

namespace olimsko
{
    /// <summary>
    /// Represents a UI managed by <see cref="UIManager"/>.
    /// </summary>
    public readonly struct ManagedUIView : IEquatable<ManagedUIView>
    {
        public readonly string Name;
        public readonly GameObject GameObject;
        public readonly IUIView UIComponent;
        public readonly Type ComponentType;

        public ManagedUIView([NotNull] string name, [NotNull] GameObject gameObject, [NotNull] IUIView uiComponent)
        {
            Name = name;
            GameObject = gameObject;
            UIComponent = uiComponent;
            ComponentType = UIComponent.GetType();
        }

        public bool Equals(ManagedUIView other) => Equals(UIComponent, other.UIComponent);
        public override bool Equals(object obj) => obj is ManagedUIView other && Equals(other);
        public override int GetHashCode() => UIComponent != null ? UIComponent.GetHashCode() : 0;
        public static bool operator ==(ManagedUIView left, ManagedUIView right) => left.Equals(right);
        public static bool operator !=(ManagedUIView left, ManagedUIView right) => !left.Equals(right);
    }
}
