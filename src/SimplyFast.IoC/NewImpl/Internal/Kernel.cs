using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SimplyFast.Collections;
using SimplyFast.Collections.Concurrent;
using SimplyFast.IoC.Internal.DefaultBuilders;

namespace SimplyFast.IoC.Internal
{
    internal class Kernel: IKernel
    {
        private readonly ConcurrentDictionary<Type, ConcurrentGrowList<Binding>> _allBindings = new ConcurrentDictionary<Type, ConcurrentGrowList<Binding>>();
        private readonly ConcurrentDictionary<Type, Binding> _bindings = new ConcurrentDictionary<Type, Binding>();
        private readonly ConcurrentDictionary<Type, Binding> _defaultBindings = new ConcurrentDictionary<Type, Binding>();
        private readonly ConcurrentDictionary<Type, Injector> _injectors = new ConcurrentDictionary<Type, Injector>();
        private readonly ConcurrentGrowList<DefaultBuilder> _defaultBuilders = new ConcurrentGrowList<DefaultBuilder>();
        private readonly GenericDerivedBuilders _genericDerivedBuilders = new GenericDerivedBuilders();

        public Kernel()
        {
            Bind(typeof(IGetKernel), c => c);
            Bind(typeof(IKernel), c => this);
            _defaultBuilders.Add(_genericDerivedBuilders.TryBuild);
        }

        public void Bind(Type type, Binding binding)
        {
            var allBindings = _allBindings.GetOrAdd(type, t => new ConcurrentGrowList<Binding>(), out var added);
            allBindings.Add(binding);
            _bindings[type] = binding;

            if (!added)
                return;

            // we have new type, possibly better constructors, injections, etc
            ClearCaches();
        }

        private void ClearCaches()
        {
            _defaultBindings.Clear();
            _injectors.Clear();
        }

        public void BindDefault(DefaultBuilder builder)
        {
            _defaultBuilders.Add(builder);

            // new types available - clear caches
            ClearCaches();
        }

        public void BindDefault(Type genericTypeDefinition, IGenericDefaultBuilder builder)
        {
            _genericDerivedBuilders.Add(genericTypeDefinition, builder);

            // new types available - clear caches
            ClearCaches();
        }

        public Binding GetUserBinding(Type type)
        {
            return _bindings.GetOrDefault(type);
        }

        public IReadOnlyCollection<Binding> GetUserBindings(Type type)
        {
            return _allBindings.TryGetValue(type, out var bindings)
                ? (IReadOnlyCollection<Binding>) bindings.GetSnapshot()
                : ArrayEx.Empty<Binding>();
        }

        public IRootKernel Root => this;

        public Binding GetDefaultBinding(Type type)
        {
            return _defaultBindings.GetOrAdd(type, t => CreateDefaultBinding(t, this));
        }

        public Binding CreateDefaultBinding(Type type, IGetKernel kernel)
        {
            foreach (var defaultBuilder in _defaultBuilders)
            {
                var binding = defaultBuilder(type, kernel);
                if (binding != null)
                    return binding;
            }
            return DefaultBindingBuilder.CreateDefaultBinding(type, kernel);
        }

        public Injector CreateDefaultInjector(Type type, IGetKernel kernel)
        {
            return DefaultInjectorBuilder.CreateDefaultInjector(type, kernel);
        }

        public Binding GetOverrideBinding(Type type, string name)
        {
            // no specific arg bindings
            return null;
        }

        public Injector GetInjector(Type type)
        {
            return _injectors.GetOrAdd(type, t => CreateDefaultInjector(t, this));
        }
    }
}