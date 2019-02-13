using System;

namespace Blink.Common.TinyClone
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
    public class CloneOverrideAttribute: Attribute
    {
        public CloneType Type { get; }

        public CloneOverrideAttribute(CloneType type)
        {
            Type = type;
        }
    }
}