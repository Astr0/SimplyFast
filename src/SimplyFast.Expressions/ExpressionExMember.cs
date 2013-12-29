using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SF.Expressions
{
    public static partial class ExpressionEx
    {
        /// <summary>
        /// Returns MemberInfo from passed lambda
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MemberInfo Member<T, TR>(Expression<Func<T, TR>> expression)
        {
            return Member((LambdaExpression)expression);
        }

        /// <summary>
        /// Returns MemberInfo from passed lambda
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MemberInfo Member<T>(Expression<Func<T>> expression)
        {
            return Member((LambdaExpression)expression);
        }

        private static MemberInfo Member(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.ArrayLength:
                    return ((UnaryExpression)expression).Operand.Type.Property("Length");
                case ExpressionType.Call:
                    return ((MethodCallExpression)expression).Method;
                case ExpressionType.Index:
                    return ((IndexExpression)expression).Indexer;
                case ExpressionType.MemberAccess:
                    return ((MemberExpression)expression).Member;
                case ExpressionType.MemberInit:
                    return ((MemberInitExpression)expression).NewExpression.Constructor;
                case ExpressionType.New:
                    return ((NewExpression)expression).Constructor;
                default:
                    var unary = expression as UnaryExpression;
                    if (unary != null)
                        return Member(unary.Operand);
                    throw new ArgumentException("Not a member {0}", expression.ToString());
            }
        }

        /// <summary>
        /// Returns MemberInfo from passed lambda
        /// </summary>
        public static MemberInfo Member(LambdaExpression lambda)
        {
            return Member(Normalize(lambda.Body));
        }

        /// <summary>
        ///     Returns FieldInfo from lambda
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldInfo Field<T, TR>(Expression<Func<T, TR>> expression)
        {
            return (FieldInfo)Member(expression);
        }

        /// <summary>
        ///     Returns FieldInfo from lambda
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldInfo Field<T>(Expression<Func<T>> expression)
        {
            return (FieldInfo)Member(expression);
        }

        /// <summary>
        ///     Returns constructors from passed lambda
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConstructorInfo Constructor<T>(Expression<Func<T>> lambda)
        {
            return (ConstructorInfo)Member(lambda);
        }

        /// <summary>
        ///     Gets method from lambda
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo Method<T, TR>(Expression<Func<T, TR>> expression)
        {
            return (MethodInfo)Member(expression);
        }

        /// <summary>
        ///     Gets method from lambda
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo Method<T>(Expression<Action<T>> expression)
        {
            return (MethodInfo)Member(expression);
        }

        /// <summary>
        ///     Gets property from lambda
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyInfo Property<T, TR>(Expression<Func<T, TR>> expression)
        {
            return (PropertyInfo)Member(expression);
        }

        /// <summary>
        ///     Gets property from lambda
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyInfo Property<T>(Expression<Func<T>> expression)
        {
            return (PropertyInfo)Member(expression);
        }

    }
}