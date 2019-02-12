using System;
using System.Collections.Concurrent;

namespace SimplyFast.IoC.Internal.Injection
{
    internal class InjectorCollection
    {
        private readonly ConcurrentDictionary<Type, IInjector> _injectors = new ConcurrentDictionary<Type, IInjector>();
        private readonly IKernel _kernel;

        public InjectorCollection(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IInjector GetInjector(Type type)
        {
            return _injectors.GetOrAdd(type, CreateInjector);
        }

        private IInjector CreateInjector(Type type)
        {
            return DefaultInjectorBuilder.CreateDefaultInjector(type, _kernel);
        }
    }
}