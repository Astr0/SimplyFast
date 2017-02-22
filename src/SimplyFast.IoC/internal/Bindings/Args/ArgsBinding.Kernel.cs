using System;
using System.Collections.Concurrent;

namespace SF.IoC.Bindings.Args
{
    internal partial class ArgsBinding 
    {
        private class Kernel : KernelBase
        {
            private readonly TypeWithArgs _args;
            private readonly IArgKernel _kernel;

            public Kernel(IArgKernel kernel, TypeWithArgs args)
            {
                _args = args;
                _kernel = kernel;
            }

            private IBinding TryGetArgBinding(Type type, string name)
            {
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                return _args.TryGetArg(type, name, out object value) ? new ConstBinding(value) : null;
            }

            public override IBinding GetBinding(Type type)
            {
                return GetArgBinding(type, null);
            }

            private readonly ConcurrentDictionary<Tuple<Type, string>, IBinding> _cache = new ConcurrentDictionary<Tuple<Type, string>, IBinding>();
            private volatile int _lastKernelVersion;

            public override IBinding GetArgBinding(Type type, string name)
            {
                // check if we can bind type with name
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                if (name != null && !_args.HasNamedArg(type, name))
                    return GetBinding(type);

                // check if we have to clear cache
                var version = _kernel.Version;
                if (version == _lastKernelVersion)
                    return _cache.GetOrAdd(Tuple.Create(type, name), FindBinding);

                _cache.Clear();
                _lastKernelVersion = version;

                return _cache.GetOrAdd(Tuple.Create(type, name), FindBinding);
            }

            private IBinding FindBinding(Tuple<Type, string> key)
            {
                return FindBinding(key.Item1, key.Item2);

            }

            private IBinding FindBinding(Type type, string name)
            {
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

            public override IBinding GetBinding(Type type, params BindArg[] args)
            {
                return DefaultArgBindingBuilder.GetBinding(this, type, args);
            }

            public override IInjector GetInjector(Type type)
            {
                return _kernel.GetInjector(type);
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

            public override int Version => _kernel.Version;
        }
    }
}