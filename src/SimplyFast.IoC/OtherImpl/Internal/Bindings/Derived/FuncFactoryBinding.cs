using System;

namespace SimplyFast.IoC.Internal.Bindings.Derived
{
    internal class FuncFactoryBinding<T> : IDerivedBinding
    {
        public void Add(IBinding binding)
        {
        }

        public void RegisterDerivedTypes(IKernel kernel)
        {
            DerivedBindingEx.TryAddDerivedType<Func<T>>(kernel, c => () => c.Get<T>());
        }
    }
}