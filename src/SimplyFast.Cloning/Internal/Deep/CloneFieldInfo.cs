using System.Reflection;

namespace SimplyFast.Cloning.Internal.Deep
{
    internal struct CloneFieldInfo
    {
        public readonly FieldInfo Field;
        public readonly CloneType CloneType;

        public CloneFieldInfo(FieldInfo field, CloneType cloneType)
        {
            Field = field;
            CloneType = cloneType;
        }
    }
}