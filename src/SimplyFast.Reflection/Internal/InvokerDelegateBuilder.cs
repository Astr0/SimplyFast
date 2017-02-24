using System;
using System.Reflection;
#if EMIT
using System.Reflection.Emit;
using SimplyFast.Reflection.Emit;
#else
using System.Collections.Generic;
using System.Linq.Expressions;
using SimplyFast.Collections;
#endif

namespace SimplyFast.Reflection.Internal
{
    internal static class InvokerDelegateBuilder
    {
#if EMIT
        private static DynamicMethod CreateDynamicMethod(MemberInfo memberInfo, Type returnType, Type[] parameterTypes)
        {
            if (memberInfo.DeclaringType == null)
            {
                return new DynamicMethod(string.Empty,
                    returnType, parameterTypes,
                    typeof(InvokerDelegateBuilder),
                    MemberInfoEx.PrivateAccess);
            }
            return new DynamicMethod(string.Empty,
                returnType, parameterTypes,
                memberInfo.DeclaringType.Module,
                MemberInfoEx.PrivateAccess);
        }

        public static MethodInvoker BuildMethodInvoker(MethodInfo methodInfo)
        {
            var dynamicMethod = CreateDynamicMethod(methodInfo, typeof(object), new[]
            {
                typeof(object),
                typeof(object[])
            });
            if (dynamicMethod == null)
            {
                // fallback
                return methodInfo.Invoke;
            }
            var il = dynamicMethod.GetILGenerator();
            var parameters = methodInfo.GetParameters();
            //var locals = EmitParamsToLocals(il, parameters, OpCodes.Ldarg_1);
            if (!methodInfo.IsStatic)
                il.Emit(OpCodes.Ldarg_0);
            EmitLoadParams(il, parameters, OpCodes.Ldarg_1);
            //EmitLoadInvokeLocals(il, locals);

            il.EmitCall(OpCodes.Call, methodInfo, null);
            if (methodInfo.ReturnType == typeof(void))
                il.Emit(OpCodes.Ldnull);
            else
                il.EmitBox(methodInfo.ReturnType);
            il.Emit(OpCodes.Ret);
            var invoker =
                (MethodInvoker)dynamicMethod.CreateDelegate(
                    typeof(MethodInvoker));
            return invoker;
        }

        public static ConstructorInvoker BuildConstructorInvoker(ConstructorInfo constructorInfo)
        {
            var dynamicMethod = CreateDynamicMethod(constructorInfo, typeof(object), new[]
{
                typeof(object[])
            });
            if (dynamicMethod == null)
            {
                // fallback
                return constructorInfo.Invoke;
            }
            var il = dynamicMethod.GetILGenerator();
            var parameters = constructorInfo.GetParameters();
            EmitLoadParams(il, parameters, OpCodes.Ldarg_0);

            il.Emit(OpCodes.Newobj, constructorInfo);
            il.EmitBox(constructorInfo.DeclaringType);
            il.Emit(OpCodes.Ret);
            var invoker =
                (ConstructorInvoker)dynamicMethod.CreateDelegate(
                    typeof(ConstructorInvoker));
            return invoker;
        }

        private static void EmitLoadParams(ILGenerator il, ParameterInfo[] parameters, OpCode ldParams)
        {
            for (var i = 0; i < parameters.Length; i++)
            {
                il.Emit(ldParams);
                il.EmitLdcI4(i);
                il.Emit(OpCodes.Ldelem_Ref);
                il.EmitUnBoxAnyOrCastClass(parameters[i].ParameterType);
            }
        }
#else
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

        public static MethodInvoker BuildMethodInvoker(MethodInfo methodInfo)
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

        public static ConstructorInvoker BuildConstructorInvoker(ConstructorInfo constructorInfo)
        {
            var invokeParameters = GetInvokeParameters(constructorInfo.GetParameters());
            var call = Expression.New(constructorInfo, invokeParameters);
            var result = Convert(call, typeof(object));
            var lambda = Expression.Lambda<ConstructorInvoker>(result, _args);
            return lambda.Compile();
        }
#endif
    }
}