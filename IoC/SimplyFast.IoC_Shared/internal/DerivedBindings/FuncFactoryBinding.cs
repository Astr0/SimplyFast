using System;
using SF.IoC.Bindings;

namespace SF.IoC.DerivedBindings
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