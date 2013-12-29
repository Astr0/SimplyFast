using System;
using System.Reflection;
using System.Reflection.Emit;
using SF.Reflection.Emit;

namespace SF.Reflection.DelegateBuilders
{
    internal abstract class FieldAccessorDelegateBuilder : DelegateBuilder
    {
        protected readonly FieldInfo _fieldInfo;

        protected FieldAccessorDelegateBuilder(FieldInfo fieldInfo, Type delegateType)
            : base(delegateType)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("fieldInfo");
            _fieldInfo = fieldInfo;
        }

        #region Overrides of DelegateBuilder

        protected override ParameterInfo GetThisParameterForMethod()
        {
            if (_fieldInfo.IsStatic)
                return null;
            var declaringType = _fieldInfo.DeclaringType;
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
            return _fieldInfo.FieldType;
        }

        protected override ParameterInfo[] GetMethodParameters()
        {
            return new ParameterInfo[0];
        }

        protected override void EmitInvoke(ILGenerator generator)
        {
            if (_fieldInfo.IsLiteral)
            {
                var value = _fieldInfo.GetValue(null);
                generator.EmitLdConst(value);
            }
            else
            {
                generator.EmitFieldGet(_fieldInfo);
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
            return new ParameterInfo[] {new SimpleParameterInfo(_fieldInfo.FieldType)};
        }

        protected override void EmitInvoke(ILGenerator generator)
        {
            generator.EmitFieldSet(_fieldInfo);
        }
    }
}