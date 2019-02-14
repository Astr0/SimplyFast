using System;
using System.Diagnostics.CodeAnalysis;

namespace SimplyFast.Cloning.Internal.Deep
{
    internal static class DeepCloneArray<T>
    {
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        // It's used by reflection from CloneObjectEx
        public static object Clone(ICloneContext context, object src)
        {
            return Array.ConvertAll((T[]) src, x => (T) context.Clone(x));
        }
    }
}