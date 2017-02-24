using System;
using System.Reflection;
using SimplyFast.Reflection.Internal.DelegateBuilders.Parameters;

#if EMIT
using System.Reflection.Emit;
using SimplyFast.Reflection.Emit;
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

        protected override Type GetThisParameterForMethod()
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
        }

        protected override Type GetMethodReturnType()
        {
            return FieldInfo.FieldType;
        }

        protected override SimpleParameterInfo[] GetMethodParameters()
        {
            return new SimpleParameterInfo[0];
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
#endif
    }

    internal class FieldSetDelegateBuilder : FieldAccessorDelegateBuilder
    {
        public FieldSetDelegateBuilder(FieldInfo fieldInfo, Type delegateType) : base(fieldInfo, delegateType)
        {
        }

        protected override Type GetMethodReturnType()
        {
            return typeof (void);
        }

        protected override SimpleParameterInfo[] GetMethodParameters()
        {
            return new [] {new SimpleParameterInfo(FieldInfo.FieldType)};
        }

#if EMIT
        protected override void EmitInvoke(ILGenerator generator)
        {
            generator.EmitFieldSet(FieldInfo);
        }
#endif
    }
}