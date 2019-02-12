using System;
using System.Collections.Concurrent;

namespace SimplyFast.IoC.Internal.Bindings
{
    internal class BindingCollection
    {
        private readonly ConcurrentDictionary<Type, IBinding> _bindings = new ConcurrentDictionary<Type, IBinding>();
        private readonly IKernel _kernel;

        public BindingCollection(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IBinding GetBinding(Type type)
        {
            return _bindings.GetOrAdd(type, CreateDefaultBinding);
        }

        public void Bind(Type type, IBinding binding)
        {
            _bindings[type] = binding;
        }

        public bool TryBind(Type type, IBinding binding)
        {
            if (_bindings.TryAdd(type, binding))
                return true;
            if (_bindings.TryUpdate(type, binding, null))
                return true;
            return false;
        }

        private IBinding CreateDefaultBinding(Type type)
        {
            // TODO: we can stackoverflow on cicular reference here, handle them
            // Beware of multithreading
            return DefaultBindingBuilder.CreateDefaultBinding(type, _kernel);
        }
    }
}