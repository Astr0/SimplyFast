using System;
using System.Reflection;
using SimplyFast.Reflection.Internal.DelegateBuilders.Parameters;

#if EMIT
using System.Reflection.Emit;
using SimplyFast.Reflection.Emit;
#else
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
#endif

namespace SimplyFast.Reflection.Internal.DelegateBuilders
{
    internal class MethodDelegateBuilder : DelegateBuilder
    {
        private readonly MethodInfo _methodInfo;

        public MethodDelegateBuilder(MethodInfo methodInfo, Type delegateType) : base(delegateType)
        {
            if (methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));
            _methodInfo = methodInfo;
            Init(GetThisParameter(), SimpleParameterInfo.FromParameters(_methodInfo.GetParameters()), _methodInfo.ReturnType);
        }

#if EMIT
        public override Delegate CreateDelegate()
        {
            var declaring = _methodInfo.DeclaringType;
            // avoid stupid issue in .Net framework...
            if (declaring != null && declaring.IsInterface && _methodInfo.IsGenericMethod)
                return base.CreateDelegate();

            var delegateExcatlyMatch = IsDelegateExcactlyMatchMethod();
            return delegateExcatlyMatch ? Delegate.CreateDelegate(_delegateType, _methodInfo) : base.CreateDelegate();
        }

        protected override void EmitInvoke(ILGenerator generator)
        {
            generator.EmitMethodCall(_methodInfo);
        }
#else
        protected override Expression Invoke(Expression[] parameters)
        {
            Expression instance;
            IEnumerable<Expression> invokeParams;
            if (_methodInfo.IsStatic)
            {
                instance = null;
                invokeParams = parameters;
            }
            else
            {
                instance = parameters[0];
                invokeParams = parameters.Skip(1);
            }

            return Expression.Call(instance, _methodInfo, invokeParams);
        }
#endif

        private Type GetThisParameter()
        {
            if (_methodInfo.IsStatic)
                return null;
            var declaringType = _methodInfo.DeclaringType;
            // ReSharper disable PossibleNullReferenceException
            return declaringType.IsValueType() ? declaringType.MakeByRefType() : declaringType;
            // ReSharper restore PossibleNullReferenceException
        }
    }
}