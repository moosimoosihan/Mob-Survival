using UnityEngine;

namespace olimsko
{
    public class RequireInterfaceAttribute : PropertyAttribute
    {
        // Interface type.
        public System.Type requiredType { get; private set; }

        public RequireInterfaceAttribute(System.Type type)
        {
            this.requiredType = type;
        }
    }
}