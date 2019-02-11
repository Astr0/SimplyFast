using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq.Expressions;
using SimplyFast.Collections;

namespace SimplyFast.Reflection.Internal
{
    internal class ExpressionsInvokerDelegateBuilder: IInvokerDelegateBuilder
    {
        private static readonly ParameterExpression _instance = Expression.Parameter(typeof(object), "instance");
        private static readonly ParameterExpression _args = Expression.Parameter(typeof(object[]), "args");
        private static readonly Expression _voidToNull = Expression.Default(typeof(object));
        private static volatile Expression[] _argIndex = TypeHelper<Expression>.EmptyArray;

        private static Expression[] GetArgs(int length)
        {
            var arr = _argIndex;
            if (arr.Length >= length)
                return arr;
            lock (_args)
            {
                arr = _argIndex;
                var currLen = arr.Length;
                if (currLen >= length)
                    return arr;
                Array.Resize(ref arr, length);
                var args = _args;
                for (; currLen < length; ++currLen)
                {
                    arr[currLen] = Expression.ArrayIndex(args, Expression.Constant(currLen));
                }
                _argIndex = arr;
                return arr;
            }
        }
        
        private static IEnumerable<Expression> GetInvokeParameters(ParameterInfo[] parameters)
        {
            var args = GetArgs(parameters.Length);
            for (var i = 0; i < parameters.Length; i++)
            {
                var arg = args[i];
                var type = parameters[i].ParameterType;
                yield return Convert(arg, type);
            }
        }

        private static Expression Convert(Expression expression, Type target)
        {
            return target != expression.Type ? Expression.Convert(expression, target) : expression;
        }

        public MethodInvoker BuildMethodInvoker(MethodInfo methodInfo)
        {
            var invokeParameters = GetInvokeParameters(methodInfo.GetParameters());
            var callInstance = !methodInfo.IsStatic
                ? (Convert(_instance, methodInfo.DeclaringType))
                : null;
            var call = Expression.Call(callInstance, methodInfo, invokeParameters);
            var result = call.Type != typeof(void) ? Convert(call, typeof(object)) : Expression.Block(call, _voidToNull);
            var lambda = Expression.Lambda<MethodInvoker>(result, _instance, _args);
            return lambda.Compile();
        }

        public ConstructorInvoker BuildConstructorInvoker(ConstructorInfo constructorInfo)
        {
            var invokeParameters = GetInvokeParameters(constructorInfo.GetParameters());
            var call = Expression.New(constructorInfo, invokeParameters);
            var result = Convert(call, typeof(object));
            var lambda = Expression.Lambda<ConstructorInvoker>(result, _args);
            return lambda.Compile();
        }
    }
}