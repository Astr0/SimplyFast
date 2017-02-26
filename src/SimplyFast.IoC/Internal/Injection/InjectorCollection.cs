using System;
using SimplyFast.Cache;

namespace SimplyFast.IoC.Internal.Injection
{
    internal class InjectorCollection
    {
        private readonly ICache<Type, IInjector> _injectors = CacheEx.ThreadSafe<Type, IInjector>();
        private readonly IGetKernel _kernel;
        private volatile int _lastKernelVersion;

        public InjectorCollection(IGetKernel kernel)
        {
            _kernel = kernel;
            _lastKernelVersion = _kernel.Version;
        }

        public IInjector GetInjector(Type type)
        {
            var version = _kernel.Version;
            if (version == _lastKernelVersion)
                return _injectors.GetOrAdd(type, CreateInjector);
            // new binding available, clear cache...
            _injectors.Clear();
            _lastKernelVersion = version;
            return _injectors.GetOrAdd(type, CreateInjector);
        }

        private IInjector CreateInjector(Type type)
        {
            return DefaultInjectorBuilder.CreateDefaultInjector(type, _kernel);
        }
    }
}