using System;

namespace SF.IoC.Bindings.Args
{
    internal class ArgsBinding : IBinding
    {
        private readonly TypeWithArgs _args;

        private ArgsBinding(TypeWithArgs args)
        {
            _args = args;
        }

        public object Get(IArgKernel kernel)
        {
            var argKernel = new Kernel(kernel, _args);
            return argKernel.GetMyObj();
        }

        public static IBinding Create(TypeWithArgs typeWithArgs)
        {
            return new ArgsBinding(typeWithArgs);
            // TODO: Maybe we will need to move this step to actual get and al
            //var argKernel = new Kernel(kernel, typeWithArgs);
            //var binding = argKernel.CreateDefaultBinding();
            //return binding != null ? new ArgsBinding(kernel, binding) : null;
        }

        private class Kernel : KernelBase
        {
            private readonly TypeWithArgs _args;
            private readonly IArgKernel _kernel;
            private readonly IBinding _myBinding;

            public Kernel(IArgKernel kernel, TypeWithArgs args)
            {
                _args = args;
                _kernel = kernel;
                _myBinding = CreateMyBinding();
                if (_myBinding == null)
                    throw new InvalidOperationException("Can't get binding for " + _args);
            }

            public override object Get(Type type)
            {
                object value;
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                // can't redirect to _kernel.Arg here since we need to route Get requests to self
                // need to get named binding
                return _args.TryGetArg(type, null, out value) ? value : base.Get(type);
            }

            public override object Arg(Type type, string name)
            {
                object value;
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                // can't redirect to _kernel.Arg here since we need to route Get requests to self
                // need to get named binding
                return _args.TryGetArg(type, name, out value) ? value : base.Arg(type, name);
            }

            public override bool CanBind(Type type, string argName)
            {
                // just perf optimize to not create arg binding every time
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                return _args.Match(type, argName) || base.CanBind(type, argName);
            }

            private IBinding TryGetArgBinding(Type type, string name)
            {
                object value;
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                return _args.TryGetArg(type, name, out value) ? new ConstBinding(value) : null;
            }

            public override IBinding GetBinding(Type type)
            {
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                var binding = TryGetArgBinding(type, null);
                if (binding != null)
                    return binding;
                binding = _kernel.GetBinding(type);
                if (binding != null && !DefaultBindingBuilder.IsDefaultBinding(binding))
                    return binding;
                return CreateDefaultArgBinding(type);
            }

            public override IBinding GetArgBinding(Type type, string name)
            {
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                var binding = TryGetArgBinding(type, name);
                if (binding != null)
                    return binding;
                binding = _kernel.GetArgBinding(type, name);
                if (binding != null && !DefaultBindingBuilder.IsDefaultBinding(binding))
                    return binding;
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

            public object GetMyObj()
            {
                return _myBinding.Get(this);
            }
        }
    }
}