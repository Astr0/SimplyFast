using System;
using System.Collections.Concurrent;

namespace SF.IoC.Bindings
{
    internal class BindingCollection
    {
        private readonly ConcurrentDictionary<Type, IBinding> _bindings = new ConcurrentDictionary<Type, IBinding>();
        private readonly IKernel _kernel;
        private readonly ConcurrentDictionary<Type, IBinding> _defaultBindings = new ConcurrentDictionary<Type, IBinding>();
        private volatile int _version;

        public BindingCollection(IKernel kernel)
        {
            _kernel = kernel;
        }

        public int Version => _version;

        public IBinding GetBinding(Type type)
        {
            IBinding binding;
            // try to get existing or create default
            return _bindings.TryGetValue(type, out binding) ? binding : _defaultBindings.GetOrAdd(type, CreateDefaultBinding);
        }

        private IBinding CreateDefaultBinding(Type type)
        {
            return DefaultBindingBuilder.CreateDefaultBinding(type, _kernel);
        }

        public void Bind(Type type, IBinding binding)
        {
            var hadType = _bindings.ContainsKey(type);
             _bindings[type] = binding;
            if (!hadType)
            {
                _version++;
                // type was added, so clear default cache
                _defaultBindings.Clear();
            }
        }

        public bool TryBind(Type type, IBinding binding)
        {
            if (!_bindings.TryAdd(type, binding))
                return false;
            // clear default cache
            _version++;
            _defaultBindings.Clear();
            return true;
        }
    }
}