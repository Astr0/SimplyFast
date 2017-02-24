using System;
using System.Reflection;
using SimplyFast.Reflection.Internal.DelegateBuilders.Parameters;

#if EMIT
using System.Reflection.Emit;
#else
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
#endif

namespace SimplyFast.Reflection.Internal.DelegateBuilders
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
            Init(null, SimpleParameterInfo.FromParameters(_constructorInfo.GetParameters()), constructor.DeclaringType);
        }

        
#if EMIT
        protected override void EmitInvoke(ILGenerator generator)
        {
            generator.Emit(OpCodes.Newobj, _constructorInfo);
        }
#else
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        protected override Expression Invoke(Expression[] parameters)
        {
            return Expression.New(_constructorInfo, parameters);
        }
#endif
    }
}