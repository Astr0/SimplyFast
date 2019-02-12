using System;
using System.Diagnostics.CodeAnalysis;
using SimplyFast.IoC.Internal.Bindings;
using SimplyFast.IoC.Internal.Bindings.Args;
using SimplyFast.IoC.Internal.Bindings.Derived;
using SimplyFast.IoC.Internal.Injection;

namespace SimplyFast.IoC.Internal
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal class FastKernel : KernelBase, IKernel
    {
        private readonly ArgBindingCollection _argsBindings;

        private readonly BindingCollection _bindings;
        private readonly DerivedBindingCollection _derivedBindings;
        private readonly InjectorCollection _injectors;

        public FastKernel()
        {
            _bindings = new BindingCollection(this);
            _injectors = new InjectorCollection(this);
            _argsBindings = new ArgBindingCollection(this);
            _derivedBindings = new DerivedBindingCollection(this);

            // Add ignore bindings
            _bindings.Bind(typeof(string), null);

            // Bind kernel to self
            // TODO: Kernel base should "bind" those too, or else factories won't get derived kernels
            // Better not bind, but return in GetBinding
            this.Bind(new ConstBinding<IGetKernel>(this));
            this.Bind(new ConstBinding<IArgKernel>(this));
            this.Bind(new ConstBinding<IKernel>(this));
        }

        public override IInjector GetInjector(Type type)
        {
            return _injectors.GetInjector(type);
        }

        public override IBinding GetBinding(Type type, params BindArg[] args)
        {
            return _argsBindings.GetBinding(type, args);
        }

        public override IBinding GetBinding(Type type)
        {
            return _bindings.GetBinding(type);
        }

        public void Bind(Type type, IBinding binding)
        {
            _bindings.Bind(type, binding);
            _derivedBindings.Add(type, binding);
        }

        public bool TryBind(Type type, IBinding binding)
        {
            return _bindings.TryBind(type, binding);
        }

        public override IBinding GetArgBinding(Type type, string name)
        {
            return GetBinding(type);
        }
    }
}