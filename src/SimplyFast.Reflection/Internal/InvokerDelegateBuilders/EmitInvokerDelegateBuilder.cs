using System;
using System.Reflection;
using System.Reflection.Emit;
using SimplyFast.Reflection.Emit;

namespace SimplyFast.Reflection.Internal
{
    internal class EmitInvokerDelegateBuilder : IInvokerDelegateBuilder
    {
        private static DynamicMethod CreateDynamicMethod(MemberInfo memberInfo, Type returnType, Type[] parameterTypes)
        {
            if (memberInfo.DeclaringType == null)
            {
                return new DynamicMethod(string.Empty,
                    returnType, parameterTypes,
                    typeof(EmitInvokerDelegateBuilder),
                    MemberInfoEx.PrivateAccess);
            }
            return new DynamicMethod(string.Empty,
                returnType, parameterTypes,
                memberInfo.DeclaringType.Module,
                MemberInfoEx.PrivateAccess);
        }

        public MethodInvoker BuildMethodInvoker(MethodInfo methodInfo)
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

        public ConstructorInvoker BuildConstructorInvoker(ConstructorInfo constructorInfo)
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
    }
}