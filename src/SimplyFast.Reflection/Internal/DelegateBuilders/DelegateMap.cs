using System;
using System.Reflection;
using SimplyFast.Collections;
using SimplyFast.Reflection.Internal.DelegateBuilders.Parameters;

namespace SimplyFast.Reflection.Internal.DelegateBuilders
{
    internal class DelegateMap
    {
        public readonly SimpleParameterInfo[] _delegateParams;
        public readonly Type _delegateType;
        public readonly ArgParameterMap[] _parametersMap;
        public readonly RetValMap _retValMap;

        public DelegateMap(Type delegateType, Type thisParameter, SimpleParameterInfo[] methodParameters, Type methodReturn)
        {
            if (delegateType == null)
                throw new ArgumentNullException(nameof(delegateType));
            _delegateType = delegateType;
            
            // delegate info
            var invokeMethod = GetDelegateInvokeMethod();
            _delegateParams = SimpleParameterInfo.FromParameters(invokeMethod.GetParameters());
            var delegateReturn = invokeMethod.ReturnType;


            SimpleParameterInfo[] fullMethodParameters;
            // Check return
            if (thisParameter == null)
            {
                fullMethodParameters = methodParameters;
            }
            else
            {
                var fullParameters = new SimpleParameterInfo[methodParameters.Length + 1];
                fullParameters[0] = new SimpleParameterInfo(thisParameter);
                Array.Copy(methodParameters, 0, fullParameters, 1, methodParameters.Length);
                fullMethodParameters = fullParameters;
            }
            
            // map
            try
            {
                // Check param count
                if (_delegateParams.Length != fullMethodParameters.Length)
                    throw new Exception("Invalid parameters count.");

                _parametersMap = _delegateParams
                    .ConvertAll((delegateParam, i) => 
                        ArgParameterMap.CreateParameterMap(delegateParam, i, fullMethodParameters[i]));

                _retValMap = new RetValMap(delegateReturn, methodReturn);
            }
            catch (Exception ex)
            {
                throw InvalidSignatureException(ex);
            }
        }

        private MethodInfo GetDelegateInvokeMethod()
        {
            if (_delegateType.TypeInfo().BaseType != typeof(MulticastDelegate))
                throw NotDelegateException();
            var invokeMethod = _delegateType.Method("Invoke");
            if (invokeMethod == null)
                throw NotDelegateException();
            return invokeMethod;
        }

        private Exception NotDelegateException()
        {
            return new InvalidOperationException(_delegateType + " is not delegate type.");
        }

        private Exception InvalidSignatureException(Exception innerException)
        {
            return new InvalidOperationException(_delegateType + " has invalid signature.", innerException);
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