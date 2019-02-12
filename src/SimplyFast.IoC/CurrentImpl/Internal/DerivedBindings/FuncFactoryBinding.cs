using System;
using SimplyFast.IoC.Internal.Bindings;

namespace SimplyFast.IoC.Internal.DerivedBindings
{
    internal class FuncFactoryBinding : IGenericDerivedBinding
    {
        private FuncFactoryBinding()
        {
        }

        public IBinding TryBind<TInner>()
        {
            return new MethodBinding<Func<TInner>>(c => () => c.Get<TInner>());
        }

        public static void Register(IDerivedKernel kernel)
        {
            kernel.BindDerived(typeof(Func<>), new FuncFactoryBinding());
        }
    }
}