using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace SF
{
    /// <summary>
    ///     Utils to work with anonymous objects
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedParameter.Global")]
    public static class AnonymousEx
    {
        /// <summary>
        /// Anonymous action for object
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Action<T> Action<T>(T obj, Action<T> action)
        {
            return action;
        }

        /// <summary>
        /// Anonymous func for object
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<T, TR> Func<T, TR>(T obj, Func<T, TR> func)
        {
            return func;
        }

        #region Lambdas
        /// <summary>
        /// Anonymous Expression action
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expression<Action<T>> ExpressionAction<T>(T obj, Expression<Action<T>> action)
        {
            return action;
        }

        /// <summary>
        /// Anonymous expression func
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expression<Func<T, TR>> ExpressionFunc<T, TR>(T obj, Expression<Func<T, TR>> func)
        {
            return func;
        }

        #endregion

        #region Collections

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> Enumerable<T>(T obj)
        {
            return System.Linq.Enumerable.Empty<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Array<T>(T obj, int count)
        {
            return new T[count];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> List<T>(T obj)
        {
            return new List<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> List<T>(T obj, int capacity)
        {
            return new List<T>(capacity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stack<T> Stack<T>(T obj)
        {
            return new Stack<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stack<T> Stack<T>(T obj, int capacity)
        {
            return new Stack<T>(capacity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Queue<T> Queue<T>(T obj)
        {
            return new Queue<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Queue<T> Queue<T>(T obj, int capacity)
        {
            return new Queue<T>(capacity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HashSet<T> HashSet<T>(T obj)
        {
            return new HashSet<T>();
        }

        #endregion
    }
}