// Copyright 2017-2021 Elringus (Artyom Sovetnikov). All rights reserved.

using System;

namespace olimsko
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class InitializeAtRuntimeAttribute : Attribute
    {
        public int InitializationPriority { get; }
        public Type Override { get; }

        public InitializeAtRuntimeAttribute(int initializationPriority = 0, Type @override = null)
        {
            InitializationPriority = initializationPriority;
            Override = @override;

            if (@override != null && @override.IsInterface)
                throw new ArgumentException("To override a built-in OSM Entity provide type of that service, not an interface.", nameof(@override));
        }
    }
}
