using System;
using System.Collections.Concurrent;
using System.Threading;

namespace SimplyFast.IoC.Internal
{
    internal class ArgKernel: IGetKernel
    {
        private readonly IGetKernel _kernel;
        private readonly BindArg[] _args;
        private readonly ConcurrentDictionary<Type, Binding> _defaultBindings = new ConcurrentDictionary<Type, Binding>();
        private readonly ConcurrentDictionary<Type, Injector> _injectors = new ConcurrentDictionary<Type, Injector>();
        private long _lastCacheOkVersion;

        public ArgKernel(IGetKernel kernel, BindArg[] args)
        {
            _kernel = kernel;
            _args = args;
            Root = _kernel.Root;
            _lastCacheOkVersion = Root.Version;
        }

        public IRootKernel Root { get; }

        private void CheckCaches()
        {
            var v = Root.Version;
            if (Interlocked.Exchange(ref _lastCacheOkVersion, v) == v)
                return;
            _defaultBindings.Clear();
            _injectors.Clear();
        }

        public Binding GetDefaultBinding(Type type)
        {
            var binding = GetOverrideBinding(type, null);
            if (binding != null)
                return binding;

            CheckCaches();
            return _defaultBindings.GetOrAdd(type, Root.CreateDefaultBinding(type, this));
        }

        public Binding GetOverrideBinding(Type type, string name)
        {
            // TODO: Root create
            foreach (var arg in _args)
            {
                if (arg.Match(type, name))
                    return c => arg.Value;
            }

            return _kernel.GetOverrideBinding(type, name);
        }

        public Injector GetInjector(Type type)
        {
            CheckCaches();
            return _injectors.GetOrAdd(type, Root.CreateDefaultInjector(type, this));
        }
    }
}