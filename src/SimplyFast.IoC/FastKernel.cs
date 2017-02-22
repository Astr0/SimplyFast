using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SF.IoC.Bindings;
using SF.IoC.Bindings.Args;
using SF.IoC.Bindings.Derived;
using SF.IoC.Injection;

namespace SF.IoC
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class FastKernel : IKernel
    {
        private readonly BindingCollection _bindings;
        private readonly DerivedBindingCollection _derivedBindings;
        private readonly InjectorCollection _injectors;

        public FastKernel()
        {
            _bindings = new BindingCollection(this);
            _injectors = new InjectorCollection(this);
            _derivedBindings = new DerivedBindingCollection(this);

            // Bind kernel to self
            this.Bind(new MethodBinding<IGetKernel>(c => c));
            this.Bind(new ConstBinding<IKernel>(this));
        }

        public IInjector GetInjector(Type type)
        {
            return _injectors.GetInjector(type);
        }

        public IBinding GetBinding(Type type, params BindArg[] args)
        {
            return DefaultArgBindingBuilder.GetBinding(this, type, args);
        }

        public IBinding GetBinding(Type type)
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

        public IBinding GetArgBinding(Type type, string name)
        {
            return GetBinding(type);
        }

        public int Version => _bindings.Version;
        public IReadOnlyList<IBinding> GetAllBindings(Type type)
        {
            return _bindings.GetAllBindings(type);
        }
    }
}