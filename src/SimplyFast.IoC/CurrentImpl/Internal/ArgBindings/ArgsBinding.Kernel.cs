using System;
using System.Collections.Generic;
using System.Linq;
using SimplyFast.Cache;
using SimplyFast.Collections;
using SimplyFast.IoC.Internal.Bindings;
using SimplyFast.IoC.Internal.Injection;

namespace SimplyFast.IoC.Internal.ArgBindings
{
    internal partial class ArgsBinding
    {
        private partial class Kernel : IGetKernel
        {
            private readonly TypeWithArgs _args;
            private readonly IGetKernel _kernel;
            private readonly InjectorCollection _injectors;

            public Kernel(IGetKernel kernel, TypeWithArgs args)
            {
                _args = args;
                _kernel = kernel;
                _injectors = new InjectorCollection(this);
            }

            private IBinding TryGetArgBinding(Type type, string name)
            {
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                return _args.TryGetArg(type, name, out object value) ? new ConstBinding(value) : null;
            }

            public IBinding GetBinding(Type type)
            {
                return GetArgBinding(type, null);
            }

            private readonly ICache<ArgKey, IBinding> _cache = CacheEx.ThreadSafe<ArgKey, IBinding>();
            private volatile int _lastKernelVersion;

            public IBinding GetArgBinding(Type type, string name)
            {
                // check if we can bind type with name
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                if (name != null && !_args.HasNamedArg(type, name))
                    return GetBinding(type);

                // check if we have to clear cache
                var version = _kernel.Version;
                if (version == _lastKernelVersion)
                    return _cache.GetOrAdd(new ArgKey(type, name), FindBinding);

                _cache.Clear();
                _lastKernelVersion = version;

                return _cache.GetOrAdd(new ArgKey(type, name), FindBinding);
            }

            private IBinding FindBinding(ArgKey key)
            {
                var type = key.Type;
                var name = key.Name;

                if (type == _args.Type)
                    return CreateMyBinding();

                // do we have arg?
                var binding = TryGetArgBinding(type, name);
                if (binding != null)
                    return binding;

                // does kernel have binding?
                binding = name != null ? _kernel.GetArgBinding(type, name) : _kernel.GetBinding(type);
                if (binding != null && !DefaultBindingBuilder.IsDefaultBinding(binding))
                    return binding;

                // ok, create arg binding...
                return CreateDefaultArgBinding(type);
            }

            public IBinding GetBinding(Type type, params BindArg[] args)
            {
                return DefaultArgBindingBuilder.GetBinding(this, type, args);
            }

            public IInjector GetInjector(Type type)
            {
                return _injectors.GetInjector(type);
            }

            private IBinding CreateMyBinding()
            {
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                return TryGetArgBinding(_args.Type, null) ??
                        CreateDefaultArgBinding(_args.Type) ??
                       _kernel.GetBinding(_args.Type);
            }

            private IBinding CreateDefaultArgBinding(Type type)
            {
                return DefaultBindingBuilder.CreateDefaultBinding(type, this, _args.ConstructorHaveAllArgs);
            }

            public int Version => _kernel.Version;
            public IReadOnlyList<IBinding> GetAllBindings(Type type)
            {
                var baseBindings = _kernel.GetAllBindings(type);
                if (baseBindings.Count > 1)
                    return baseBindings;
                if (baseBindings.Count == 1 && !DefaultBindingBuilder.IsDefaultBinding(baseBindings.First()))
                    return baseBindings;
                // use our binding
                var binding = GetBinding(type);
                return binding != null ? new[] { binding } : TypeHelper<IBinding>.EmptyArray;
            }
        }
    }
}