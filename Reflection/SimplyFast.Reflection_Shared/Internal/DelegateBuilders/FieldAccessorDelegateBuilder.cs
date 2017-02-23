using System;
using System.Reflection;
using System.Reflection.Emit;
using SF.Reflection.Emit;

namespace SF.Reflection.DelegateBuilders
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

        #region Overrides of DelegateBuilder

        protected override ParameterInfo GetThisParameterForMethod()
        {
            if (FieldInfo.IsStatic)
                return null;
            var declaringType = FieldInfo.DeclaringType;
            // ReSharper disable PossibleNullReferenceException
            return new SimpleParameterInfo(declaringType.IsValueType ? declaringType.MakeByRefType() : declaringType);
            // ReSharper restore PossibleNullReferenceException
        }

        #endregion
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

        protected override ParameterInfo[] GetMethodParameters()
        {
            return new ParameterInfo[0];
        }

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

        protected override ParameterInfo[] GetMethodParameters()
        {
            return new ParameterInfo[] {new SimpleParameterInfo(FieldInfo.FieldType)};
        }

        protected override void EmitInvoke(ILGenerator generator)
        {
            generator.EmitFieldSet(FieldInfo);
        }
    }
}