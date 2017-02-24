using System.Reflection;

namespace SimplyFast.Collections
{
    public static class TypeHelper<T>
    {
        public static readonly T[] EmptyArray = new T[0];
        public static readonly bool IsReferenceType = !typeof(T).GetTypeInfo().IsValueType;
    }
}