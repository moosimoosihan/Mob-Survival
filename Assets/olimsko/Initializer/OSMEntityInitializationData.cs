// Copyright 2017-2021 Elringus (Artyom Sovetnikov). All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace olimsko
{
    public readonly struct OSMEntityInitializationData : IEquatable<OSMEntityInitializationData>
    {
        public readonly Type Type;
        public readonly int Priority;
        public readonly Type[] CtorArgs;
        public readonly Type Override;

        public OSMEntityInitializationData(Type type, InitializeAtRuntimeAttribute attr)
        {
            Type = type;
            Priority = attr.InitializationPriority;
            CtorArgs = Type.GetConstructors().First().GetParameters().Select(p => p.ParameterType).ToArray();
            Override = attr.Override;
        }

        public override bool Equals(object obj) => obj is OSMEntityInitializationData data && Equals(data);
        public bool Equals(OSMEntityInitializationData other) => EqualityComparer<Type>.Default.Equals(Type, other.Type);
        public override int GetHashCode() => 2049151605 + EqualityComparer<Type>.Default.GetHashCode(Type);
        public static bool operator ==(OSMEntityInitializationData left, OSMEntityInitializationData right) => left.Equals(right);
        public static bool operator !=(OSMEntityInitializationData left, OSMEntityInitializationData right) => !(left == right);
    }
}
