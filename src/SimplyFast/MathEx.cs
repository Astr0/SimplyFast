using System;

namespace SimplyFast
{
    public static class MathEx
    {
        public static T Min<T>(T value1, T value2) where T : IComparable<T>
        {
            return value1.CompareTo(value2) > 0 ? value2 : value1;
        }

        public static T Max<T>(T value1, T value2) where T : IComparable<T>
        {
            return value1.CompareTo(value2) < 0 ? value2 : value1;
        }

        public static T Clip<T>(this T value, T min, T max) where T : IComparable<T>
        {
            if (min.CompareTo(max) > 0)
                throw new ArgumentException("min should be less then max.", nameof(min));
            return Min(Max(value, min), max);
        }

        public static bool InRange<T>(this T value, T lower, T upper) where T : IComparable<T>
        {
            if (lower.CompareTo(upper) > 0)
                throw new ArgumentException("lower should be less then upper.", nameof(lower));
            return (value.CompareTo(lower) >= 0) && (value.CompareTo(upper) <= 0);
        }
    }

}