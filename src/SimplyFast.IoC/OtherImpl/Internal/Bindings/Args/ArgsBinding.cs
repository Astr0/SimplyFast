using System;

namespace SimplyFast.IoC.Internal.Bindings.Args
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

        internal static IBinding Create(TypeWithArgs typeWithArgs)
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
                if (_args.TryGetArg(type, null, out value))
                    return value;

                return base.Get(type);
            }

            public override object Arg(Type type, string name)
            {
                object value;
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                // can't redirect to _kernel.Arg here since we need to route Get requests to self
                // need to get named binding
                if (_args.TryGetArg(type, name, out value))
                    return value;

                return base.Arg(type, name);
            }

            public override bool CanBind(Type type, string argName)
            {
                // just perf optimize to not create arg binding every time
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                return _args.Match(type, argName) || _kernel.CanBind(type, argName);
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
                return TryGetArgBinding(type, null) ?? _kernel.GetBinding(type);
            }

            public override IBinding GetArgBinding(Type type, string name)
            {
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                return TryGetArgBinding(type, name) ?? _kernel.GetArgBinding(type, name);
            }

            public override IBinding GetBinding(Type type, params BindArg[] args)
            {
                // TODO: return my binding here?
                return _kernel.GetBinding(type, args);
            }

            public override IInjector GetInjector(Type type)
            {
                return _kernel.GetInjector(type);
            }

            private IBinding CreateMyBinding()
            {
                return DefaultBindingBuilder.CreateDefaultBinding(_args.Type, this, _args.ConstructorHaveAllArgs) ??
                       _kernel.GetBinding(_args.Type);
            }

            public object GetMyObj()
            {
                return _myBinding.Get(this);
            }
        }
    }
}