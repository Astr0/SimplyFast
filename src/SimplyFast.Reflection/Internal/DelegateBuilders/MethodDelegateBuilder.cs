using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimplyFast.Reflection.Internal.DelegateBuilders.Parameters;

#if EMIT
using SimplyFast.Comparers;
using System.Reflection.Emit;
using SimplyFast.Reflection.Emit;
#else
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
        }

#if EMIT
        private static readonly EqualityComparer<SimpleParameterInfo[]> ParametersComparer =
            EqualityComparerEx.Array<SimpleParameterInfo>();


        public override Delegate CreateDelegate()
        {
            var declaring = _methodInfo.DeclaringType;
            // avoid stupid issue in .Net framework...
            if (declaring != null && declaring.IsInterface && _methodInfo.IsGenericMethod)
                return base.CreateDelegate();

            var delegateExcatlyMatch = _delegateReturn == _methodReturn &&
                                        ParametersComparer.Equals(_delegateParams, _methodParameters);

            return delegateExcatlyMatch ? Delegate.CreateDelegate(_delegateType, _methodInfo) : base.CreateDelegate();
        }

        protected override void EmitInvoke(ILGenerator generator)
        {
            generator.EmitMethodCall(_methodInfo);
        }
#else
        protected override Expression Invoke(List<Expression> block, Expression[] parameters)
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

            var invoke = Expression.Call(instance, _methodInfo, invokeParams);
            if (_methodReturn == typeof(void))
            {
                block.Add(invoke);
                return null;
            }

            var local = Expression.Variable(_methodReturn);
            var assign = Expression.Assign(local, invoke);
            block.Add(assign);
            return local;
        }
#endif

        protected override Type GetMethodReturnType()
        {
            return _methodInfo.ReturnType;
        }

        protected override SimpleParameterInfo[] GetMethodParameters()
        {
            return SimpleParameterInfo.FromParameters(_methodInfo.GetParameters());
        }

        protected override Type GetThisParameterForMethod()
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