using System;
using System.Linq;
using SimplyFast.Collections.Concurrent;

namespace SimplyFast.IoC.Internal.DerivedBindings
{
    internal class DerivedBindingCollection
    {
        private readonly ConcurrentGrowList<IDerivedBinding> _bindings = new ConcurrentGrowList<IDerivedBinding>();

        public IBinding TryCreateBinding(Type type)
        {
            return _bindings
                .Select(x => x.TryBind(type))
                .FirstOrDefault(x => x != null);
        }

        public void Add(IDerivedBinding binding)
        {
            _bindings.Add(binding);
        }
    }
}