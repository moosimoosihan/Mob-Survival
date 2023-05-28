using System;

namespace olimsko
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class OverrideSettingsAttribute : Attribute { }
}