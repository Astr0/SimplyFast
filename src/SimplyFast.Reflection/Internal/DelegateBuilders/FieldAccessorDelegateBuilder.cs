using System;
using System.Reflection;
using SimplyFast.Collections;
using SimplyFast.Reflection.Internal.DelegateBuilders.Parameters;

#if EMIT
using System.Reflection.Emit;
using SimplyFast.Reflection.Emit;
#else
using System.Linq.Expressions;
#endif

namespace SimplyFast.Reflection.Internal.DelegateBuilders
{
    internal abstract class FieldAccessorDelegateBuilder : DelegateBuilder
    {
        protected readonly FieldInfo FieldInfo;

        protected FieldAccessorDelegateBuilder(FieldInfo fieldInfo, Type delegateType)
            : base(delegateType)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException(nameof(fieldInfo));
            FieldInfo = fieldInfo;
        }

        protected Type GetThisParameter()
        {
            if (FieldInfo.IsStatic)
                return null;
            var declaringType = FieldInfo.DeclaringType;
            // ReSharper disable PossibleNullReferenceException
            return declaringType.IsValueType() ? declaringType.MakeByRefType() : declaringType;
            // ReSharper restore PossibleNullReferenceException
        }
    }

    internal class FieldGetDelegateBuilder : FieldAccessorDelegateBuilder
    {
        public FieldGetDelegateBuilder(FieldInfo fieldInfo, Type delegateType) : base(fieldInfo, delegateType)
        {
            Init(GetThisParameter(), TypeHelper<SimpleParameterInfo>.EmptyArray, fieldInfo.FieldType);
        }

#if EMIT
        protected override void EmitInvoke(ILGenerator generator)
        {
            if (FieldInfo.IsLiteral)
            {
                var value = FieldInfo.GetValue(null);
                generator.EmitLdConst(value);
            }
            else
            {
                generator.EmitFieldGet(FieldInfo);
            }
        }
#else
        protected override Expression Invoke(Expression[] parameters)
        {
            var instance = parameters.Length != 0 ? parameters[0] : null;
            return Expression.Field(instance, FieldInfo);
        }
#endif
    }

    internal class FieldSetDelegateBuilder : FieldAccessorDelegateBuilder
    {
        public FieldSetDelegateBuilder(FieldInfo fieldInfo, Type delegateType) : base(fieldInfo, delegateType)
        {
            var fieldType = FieldInfo.FieldType;
            Init(GetThisParameter(), new[] { new SimpleParameterInfo(fieldType) }, fieldType);
        }

#if EMIT
        protected override void EmitInvoke(ILGenerator generator)
        {
            generator.EmitFieldSet(FieldInfo);
        }
#else
        protected override Expression Invoke(Expression[] parameters)
        {
            var instance = parameters.Length > 1 ? parameters[0] : null;
            var value = parameters.Length > 1 ? parameters[1] : parameters[0];
            var field = Expression.Field(instance, FieldInfo);
            return Expression.Assign(field, value);
        }
#endif
    }
}