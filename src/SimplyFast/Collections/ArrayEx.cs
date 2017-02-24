using System;

namespace SimplyFast.Collections
{
    public static class ArrayEx
    {
#if NET
        public static TR[] ConvertAll<T, TR>(this T[] array, Converter<T, TR> convert)
        {
            return Array.ConvertAll(array, convert);
        }

        public static void ForEach<T>(this T[] array, Action<T> action)
        {
            Array.ForEach(array, action);
        }
#else
        public static TR[] ConvertAll<T, TR>(this T[] array, Func<T, TR> convert)
        {
            if (array == null)
                return null;
            if (array.Length == 0)
                return TypeHelper<TR>.EmptyArray;
            var result = new TR[array.Length];
            for (var i = 0; i < array.Length; ++i)
                result[i] = convert(array[i]);
            return result;
        }

        public static void ForEach<T>(this T[] array, Action<T> action)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            foreach (var item in array)
            {
                action(item);
            }
        }
#endif
    }
}
