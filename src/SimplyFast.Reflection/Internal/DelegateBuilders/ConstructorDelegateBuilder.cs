using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using SimplyFast.Reflection.Internal.DelegateBuilders.Parameters;

#if EMIT
using System.Reflection.Emit;
#else
using System.Collections.Generic;
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
        }

        protected override SimpleParameterInfo[] GetMethodParameters()
        {
            return SimpleParameterInfo.FromParameters(_constructorInfo.GetParameters());
        }

#if EMIT
        protected override void EmitInvoke(ILGenerator generator)
        {
            generator.Emit(OpCodes.Newobj, _constructorInfo);
        }
#else
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        protected override Expression Invoke(List<Expression> block, Expression[] parameters)
        {
            var local = Expression.Variable(_methodReturn);
            var create = Expression.New(_constructorInfo, parameters);
            var assign = Expression.Assign(local, create);
            block.Add(assign);
            return local;
        }
#endif

        protected override Type GetMethodReturnType()
        {
            return _constructorInfo.DeclaringType;
        }

        protected override Type GetThisParameterForMethod()
        {
            return null;
        }
    }
}