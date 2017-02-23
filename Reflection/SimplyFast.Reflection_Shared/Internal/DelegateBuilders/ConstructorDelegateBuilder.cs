using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SF.Reflection.DelegateBuilders
{
    internal class ConstructorDelegateBuilder : DelegateBuilder
    {
        private readonly ConstructorInfo _constructorInfo;

        public ConstructorDelegateBuilder(ConstructorInfo constructor, Type delegateType)
            : base(delegateType)
        {
            if (constructor == null)
                throw new ArgumentNullException(nameof(constructor));
            _constructorInfo = constructor;
        }

        protected override ParameterInfo[] GetMethodParameters()
        {
            return _constructorInfo.GetParameters();
        }

        protected override void EmitInvoke(ILGenerator generator)
        {
            generator.Emit(OpCodes.Newobj, _constructorInfo);
        }

        protected override Type GetMethodReturnType()
        {
            return _constructorInfo.DeclaringType;
        }

        protected override ParameterInfo GetThisParameterForMethod()
        {
            return null;
        }
    }
}