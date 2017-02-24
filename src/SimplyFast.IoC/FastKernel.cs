using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SimplyFast.IoC.Internal.ArgBindings;
using SimplyFast.IoC.Internal.Bindings;
using SimplyFast.IoC.Internal.DerivedBindings;
using SimplyFast.IoC.Internal.Injection;

namespace SimplyFast.IoC
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class FastKernel : IDerivedKernel
    {
        private readonly BindingCollection _bindings;
        private readonly DerivedBindingCollection _derivedBindings;
        private readonly GenericDerivedBindings _genericDerived;
        private readonly InjectorCollection _injectors;

        public FastKernel()
        {
            _derivedBindings = new DerivedBindingCollection();
            _genericDerived = new GenericDerivedBindings();
            _derivedBindings.Add(_genericDerived);
            _bindings = new BindingCollection(this, _derivedBindings.TryCreateBinding);
            _injectors = new InjectorCollection(this);

            AddDefaultBindings();
        }

        private void AddDefaultBindings()
        {
            // Bind kernel to self
            this.Bind(new MethodBinding<IGetKernel>(c => c));
            this.Bind(new ConstBinding<IKernel>(this));

            // nice derived bindings
            FuncFactoryBinding.Register(this);
            CollectionBinding.Register(this);
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

        public void BindDerived(IDerivedBinding binding)
        {
            _derivedBindings.Add(binding);
        }

        public void BindDerived(Type genericTypeDefinition, IGenericDerivedBinding binding)
        {
            _genericDerived.Add(genericTypeDefinition, binding);
        }
    }
}