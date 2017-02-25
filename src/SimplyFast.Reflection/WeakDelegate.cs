using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using SimplyFast.Collections;

#if EMIT
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using SimplyFast.Reflection.Emit;
#endif

namespace SimplyFast.Reflection
{
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    public class WeakDelegate<T> : WeakCollection<T>
        where T : class
    {
        private WeakDelegate()
        {
            Invoker = CreateInvoke(this);
        }

        public readonly T Invoker;

        public static WeakDelegate<T> Create()
        {
            return new WeakDelegate<T>();
        }

#if EMIT
        private static readonly DynamicMethod _buildInvoke = BuildInvoke();

        private static T CreateInvoke(WeakDelegate<T> target)
        {
            return _buildInvoke.CreateDelegate<T>(target);
        }

        private static DynamicMethod BuildInvoke()
        {
            var invokeMethod = MethodInfoEx.GetInvokeMethod(typeof(T));
            var parameters = invokeMethod.GetParameters();
            var args = new Type[parameters.Length + 1];
            args[0] = typeof(WeakDelegate<T>);
            for (var i = 0; i < parameters.Length; i++)
            {
                args[i + 1] = parameters[i].ParameterType;
            }
            var returnType = invokeMethod.ReturnType;
            var invoke = new DynamicMethod(string.Empty,
                returnType, args,
                typeof(WeakDelegate<T>).Module,
                MemberInfoEx.PrivateAccess);

            var il = invoke.GetILGenerator();
            //var del = il.DeclareLocal(typeof(T));
            var hasReturn = returnType != typeof(void);
            var returnValue = hasReturn ? il.DeclareLocal(returnType) : null;

            // get enumerator
            il.Emit(OpCodes.Ldarg_0);
            il.EmitForEach(typeof(IEnumerable<T>), l =>
            {
                for (var i = 1; i < args.Length; i++)
                {
                    il.EmitLdarg(i);
                }
                il.EmitCall(OpCodes.Callvirt, invokeMethod, null);
                if (hasReturn)
                    il.EmitStloc(returnValue);
            });

            if (hasReturn)
                il.EmitLdloc(returnValue);
            il.Emit(OpCodes.Ret);

            return invoke;
        }
#else
        private static readonly ParameterExpression[] _invokeParameters;
        private static readonly ParameterExpression _enumerator;
        private static readonly MethodInfo _getEnumerator;
        private static readonly Expression _invokeLoop;

        static WeakDelegate()
        {
            var invokeMethod = MethodInfoEx.GetInvokeMethod(typeof(T));
            _invokeParameters = invokeMethod.GetParameters().ConvertAll((x, i) => Expression.Parameter(x.ParameterType, "p" + i));
            _getEnumerator = typeof(IEnumerable<T>).Method("GetEnumerator");
            _enumerator = Expression.Variable(typeof(IEnumerator<T>), "e");

            var destroyEnumerator = Expression.Call(_enumerator, typeof(IDisposable).Method("Dispose"));

            var returnType = invokeMethod.ReturnType;
            var hasReturn = returnType != typeof(void);
            var breakLabel = hasReturn ? Expression.Label(returnType, "br") : Expression.Label("br");
            var result = hasReturn ? Expression.Variable(returnType, "r") : null;
            var breakExpr = hasReturn ? Expression.Break(breakLabel, result) : Expression.Break(breakLabel);
            var moveNext = Expression.Call(_enumerator, typeof(IEnumerator).Method("MoveNext"));
            var current = Expression.Property(_enumerator, typeof(IEnumerator<T>).Property("Current"));
            // ReSharper disable once CoVariantArrayConversion
            var invoke = (Expression)Expression.Invoke(current, _invokeParameters);
            if (hasReturn)
                invoke = Expression.Assign(result, invoke);
            var ifNotMoveNext = Expression.IfThenElse(moveNext, invoke, breakExpr);
            var loop = (Expression)Expression.Loop(ifNotMoveNext, breakLabel);
            if (hasReturn)
                loop = Expression.Block(new[] {result},
                    Expression.Assign(result, Expression.Default(returnType)),
                    loop);
           
            _invokeLoop = Expression.TryFinally(loop, destroyEnumerator);
        }
        private static T CreateInvoke(WeakDelegate<T> target)
        {
            var getEnumerator = Expression.Call(Expression.Constant(target), _getEnumerator);
            var assignEnumerator = Expression.Assign(_enumerator, getEnumerator);
            var body = Expression.Block(new[]{ _enumerator }, assignEnumerator, _invokeLoop);
            var lambda = Expression.Lambda<T>(body, _invokeParameters);
            return lambda.Compile();
        }
#endif
    }
}
