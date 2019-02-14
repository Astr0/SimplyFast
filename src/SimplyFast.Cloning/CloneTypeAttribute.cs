using System;

namespace SimplyFast.Cloning
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property)]
    public class CloneTypeAttribute: Attribute
    {
        public CloneType Type { get; }

        public CloneTypeAttribute(CloneType type)
        {
            Type = type;
        }
    }
}