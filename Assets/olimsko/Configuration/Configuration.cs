// Copyright 2017-2021 Elringus (Artyom Sovetnikov). All rights reserved.

using System;
using UnityEngine;

namespace olimsko
{
    public abstract class Configuration : ScriptableObject
    {
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
        public sealed class EditInProjectSettingsAttribute : Attribute { }

        public static T GetOrDefault<T>() where T : Configuration
        {
            if (OSManager.IsInitialized) return OSManager.GetConfiguration<T>();
            else return ConfigurationProvider.LoadOrDefault<T>();
        }
    }
}
