using System;
using System.Reflection;
using SimplyFast.Collections;

namespace SimplyFast.Reflection.Internal.DelegateBuilders.Maps
{
    internal class DelegateMap
    {
        public readonly Type DelegateType;
        public readonly ArgMap[] ParametersMap;
        public readonly RetValMap RetValMap;

        private DelegateMap(Type delegateType, Type thisParameter, SimpleParameterInfo[] methodParameters, Type methodReturn)
        {
            if (delegateType == null)
                throw new ArgumentNullException(nameof(delegateType));
            DelegateType = delegateType;
            
            // delegate info
            var invokeMethod = GetDelegateInvokeMethod();
            var delegateParams = SimpleParameterInfo.FromParameters(invokeMethod.GetParameters());
            var delegateReturn = invokeMethod.ReturnType;

            // map
            try
            {
                var hasThis = thisParameter != null;
                // Check param count
                if (delegateParams.Length != methodParameters.Length + (hasThis ? 1 : 0))
                    throw new Exception("Invalid parameters count.");

                ParametersMap = new ArgMap[delegateParams.Length];
                for (var i = 0; i < ParametersMap.Length; i++)
                {
                    var methodParam = hasThis
                        ? i == 0
                            ? new SimpleParameterInfo(thisParameter)
                            : methodParameters[i - 1]
                        : methodParameters[i];

                    ParametersMap[i] = new ArgMap(i, delegateParams[i], methodParam);
                }

                RetValMap = new RetValMap(delegateReturn, methodReturn);
            }
            catch (Exception ex)
            {
                throw InvalidSignatureException(ex);
            }
        }

        private MethodInfo GetDelegateInvokeMethod()
        {
            if (DelegateType.BaseType != typeof(MulticastDelegate))
                throw NotDelegateException();
            var invokeMethod = DelegateType.Method("Invoke");
            if (invokeMethod == null)
                throw NotDelegateException();
            return invokeMethod;
        }

        private Exception NotDelegateException()
        {
            return new InvalidOperationException(DelegateType + " is not delegate type.");
        }

        private Exception InvalidSignatureException(Exception innerException)
        {
            return new InvalidOperationException(DelegateType + " has invalid signature.", innerException);
        }


        public static DelegateMap Constructor(Type delegateType, ConstructorInfo constructor) =>
            new DelegateMap(delegateType, null, SimpleParameterInfo.FromParameters(constructor.GetParameters()),
                constructor.DeclaringType);

        private static Type GetThisParameter(FieldInfo fieldInfo)
        {
            if (fieldInfo.IsStatic)
                return null;
            var declaringType = fieldInfo.DeclaringType;
            // ReSharper disable PossibleNullReferenceException
            return declaringType.IsValueType() ? declaringType.MakeByRefType() : declaringType;
            // ReSharper restore PossibleNullReferenceException
        }

        public static DelegateMap FieldGet(Type delegateType, FieldInfo fieldInfo) =>
            new DelegateMap(delegateType, GetThisParameter(fieldInfo), TypeHelper<SimpleParameterInfo>.EmptyArray, fieldInfo.FieldType);

        public static DelegateMap FieldSet(Type delegateType, FieldInfo fieldInfo) =>
            new DelegateMap(delegateType, GetThisParameter(fieldInfo), new[] { new SimpleParameterInfo(fieldInfo.FieldType) }, fieldInfo.FieldType);

        private static Type GetThisParameter(MethodBase methodInfo)
        {
            if (methodInfo.IsStatic)
                return null;
            var declaringType = methodInfo.DeclaringType;
            // ReSharper disable PossibleNullReferenceException
            return declaringType.IsValueType() ? declaringType.MakeByRefType() : declaringType;
            // ReSharper restore PossibleNullReferenceException
        }


        public static DelegateMap Method(Type delegateType, MethodInfo methodInfo) =>
            new DelegateMap(delegateType, GetThisParameter(methodInfo), SimpleParameterInfo.FromParameters(methodInfo.GetParameters()), methodInfo.ReturnType);
    }
}