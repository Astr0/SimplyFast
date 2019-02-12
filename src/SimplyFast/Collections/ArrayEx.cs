using System;

namespace SimplyFast.Collections
{
    public static class ArrayEx
    {
        public static TR[] ConvertAll<T, TR>(this T[] array, Func<T, int, TR> convert)
        {
            if (array == null)
                return null;
            if (array.Length == 0)
                return TypeHelper<TR>.EmptyArray;
            var result = new TR[array.Length];
            for (var i = 0; i < array.Length; ++i)
                result[i] = convert(array[i], i);
            return result;
        }

        public static TR[] ConvertAll<T, TR>(this T[] array, Converter<T, TR> convert)
        {
            return Array.ConvertAll(array, convert);
        }

        public static void ForEach<T>(this T[] array, Action<T> action)
        {
            Array.ForEach(array, action);
        }

        public static T[] Concat<T>(T[] first, T[] second)
        {
            if (first == null || first.Length == 0)
                return second;
            if (second == null || second.Length == 0)
                return first;
            var result = new T[first.Length + second.Length];
            Array.Copy(first, result, first.Length);
            Array.Copy(second, 0, result, first.Length, second.Length);
            return result;
        }

        public static T[] Empty<T>()
        {
            return TypeHelper<T>.EmptyArray;
        }
    }
}
