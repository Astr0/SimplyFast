using System;
using System.Collections.Concurrent;
using SimplyFast.Collections;

namespace SimplyFast.IoC.Internal.Bindings
{
    internal class BindingCollection
    {
        private readonly ConcurrentDictionary<Type, IBinding> _bindings = new ConcurrentDictionary<Type, IBinding>();
        private readonly ConcurrentDictionary<Type, IBinding> _defaultBindings = new ConcurrentDictionary<Type, IBinding>();
        private readonly IKernel _kernel;

        public BindingCollection(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IBinding GetBinding(Type type)
        {
            return _bindings.GetOrDefault(type) ?? _defaultBindings.GetOrAdd(type, CreateDefaultBinding);
        }

        public void Bind(Type type, IBinding binding)
        {
            if (!TryBind(type, binding))
                _bindings[type] = binding;
        }

        public bool TryBind(Type type, IBinding binding)
        {
            if (!_bindings.TryAdd(type, binding))
                if (!_bindings.TryUpdate(type, binding, null))
                    return false;
            // new binding were added
            _defaultBindings.Clear();
            return true;
        }

        private IBinding CreateDefaultBinding(Type type)
        {
            // TODO: we can stackoverflow on cicular reference here, handle them
            // Beware of multithreading
            return DefaultBindingBuilder.CreateDefaultBinding(type, _kernel);
        }
    }
}