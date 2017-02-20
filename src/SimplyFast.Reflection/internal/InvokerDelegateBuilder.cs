using System;
using System.Reflection;
using System.Reflection.Emit;
using SF.Reflection.Emit;

namespace SF.Reflection
{
    internal static class InvokerDelegateBuilder
    {
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
            var locals = EmitParamsToLocals(il, parameters, OpCodes.Ldarg_1);
            if (!methodInfo.IsStatic)
                il.Emit(OpCodes.Ldarg_0);
            EmitLoadInvokeLocals(il, locals);

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
            var locals = EmitParamsToLocals(il, parameters, OpCodes.Ldarg_0);
            EmitLoadInvokeLocals(il, locals);

            il.Emit(OpCodes.Newobj, constructorInfo);
            il.EmitBox(constructorInfo.DeclaringType);
            il.Emit(OpCodes.Ret);
            var invoker =
                (ConstructorInvoker)dynamicMethod.CreateDelegate(
                    typeof(ConstructorInvoker));
            return invoker;
        }

        private static void EmitLoadInvokeLocals(ILGenerator il, LocalBuilder[] locals)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < locals.Length; i++)
                il.EmitLdloc(locals[i]);
        }

        private static LocalBuilder[] EmitParamsToLocals(ILGenerator il, ParameterInfo[] parameters, OpCode ldParams)
        {
            var paramTypes = new Type[parameters.Length];
            for (var i = 0; i < paramTypes.Length; i++)
                paramTypes[i] = parameters[i].ParameterType;
            var locals = new LocalBuilder[paramTypes.Length];
            for (var i = 0; i < locals.Length; i++)
                locals[i] = il.DeclareLocal(paramTypes[i]);
            for (var i = 0; i < paramTypes.Length; i++)
            {
                il.Emit(ldParams);
                il.EmitLdcI4(i);
                il.Emit(OpCodes.Ldelem_Ref);
                il.EmitUnBoxAnyOrCastClass(paramTypes[i]);
                il.Emit(OpCodes.Stloc, locals[i]);
            }
            return locals;
        }
    }
}