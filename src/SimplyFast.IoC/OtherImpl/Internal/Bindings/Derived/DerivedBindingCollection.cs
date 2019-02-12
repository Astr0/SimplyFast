using System;
using System.Collections.Concurrent;
using SimplyFast.Collections;

namespace SimplyFast.IoC.Internal.Bindings.Derived
{
    internal class DerivedBindingCollection
    {
        private static readonly Type _derivedBindingType = typeof(FastDerivedBindings<>);

        private readonly ConcurrentDictionary<Type, IDerivedBinding> _bindings =
            new ConcurrentDictionary<Type, IDerivedBinding>();

        private readonly IKernel _kernel;

        public DerivedBindingCollection(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Add(Type type, IBinding binding)
        {
            var derivedBinding = _bindings.GetOrAdd(type, CreateDerivedRoot, out var added);
            if (added)
                derivedBinding.RegisterDerivedTypes(_kernel);
            if (binding != null)
                derivedBinding.Add(binding);
        }

        private static IDerivedBinding CreateDerivedRoot(Type type)
        {
            return DerivedBindingEx.Create(_derivedBindingType, type);
        }
    }
}