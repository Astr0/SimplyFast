using System;
using System.Collections.Concurrent;
using SF.Collections;

namespace SF.IoC.Bindings.Derived
{
    internal class DerivedBindingCollection
    {
        private static readonly Type _derivedBindingType = typeof(DerivedBindings<>);

        private readonly ConcurrentDictionary<Type, IDerivedBinding> _bindings =
            new ConcurrentDictionary<Type, IDerivedBinding>();

        private readonly IKernel _kernel;

        public DerivedBindingCollection(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Add(Type type, IBinding binding)
        {
            bool added;
            var derivedBinding = _bindings.GetOrAdd(type, CreateDerivedRoot, out added);
            if (added)
                derivedBinding.RegisterDerivedTypes(_kernel);
            //derivedBinding.Add(binding);
        }

        private static IDerivedBinding CreateDerivedRoot(Type type)
        {
            return DerivedBindingEx.Create(_derivedBindingType, type);
        }
    }
}