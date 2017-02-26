﻿using System;
using System.Collections.Generic;
using SimplyFast.Cache;
using SimplyFast.Collections;
using SimplyFast.Collections.Concurrent;

namespace SimplyFast.IoC.Internal.Bindings
{
    internal class BindingCollection
    {
        private readonly ICache<Type, ConcurrentGrowList<IBinding>> _allBindings = CacheEx.ThreadSafe<Type, ConcurrentGrowList<IBinding>>();
        private readonly ICache<Type, IBinding> _bindings = CacheEx.ThreadSafe<Type, IBinding>();
        private readonly IKernel _kernel;
        private readonly Func<Type, IBinding> _customDefaultBinding;
        private readonly ICache<Type, IBinding> _defaultBindings = CacheEx.ThreadSafe<Type, IBinding>();
        private volatile int _version;

        public BindingCollection(IKernel kernel, Func<Type, IBinding> customDefaultBinding = null)
        {
            _kernel = kernel;
            _customDefaultBinding = customDefaultBinding ?? NoCustomBinding;
        }

        private static IBinding NoCustomBinding(Type type)
        {
            return null;
        }

        public int Version => _version;

        public IBinding GetBinding(Type type)
        {
            IBinding binding;
            // try to get existing or create default
            return _bindings.TryGetValue(type, out binding) ? binding : _defaultBindings.GetOrAdd(type, CreateDefaultBinding);
        }

        private IBinding CreateDefaultBinding(Type type)
        {
            return _customDefaultBinding(type) ?? DefaultBindingBuilder.CreateDefaultBinding(type, _kernel);
        }

        public void Bind(Type type, IBinding binding)
        {
            var allBindings = _allBindings.GetOrAdd(type, t => new ConcurrentGrowList<IBinding>(), out bool addedNewType);
            allBindings.Add(binding);
             _bindings.Upsert(type, binding);
            if (!addedNewType)
                return;
            _version++;
            // type was added, so clear default cache
            _defaultBindings.Clear();
        }

        public bool TryBind(Type type, IBinding binding)
        {
            if (!_bindings.TryAdd(type, binding))
                return false;
            // add to all binding
            _allBindings.GetOrAdd(type, t => new ConcurrentGrowList<IBinding>()).Add(binding);
            // clear default cache
            _defaultBindings.Clear();
            _version++;
            return true;
        }

        public IReadOnlyList<IBinding> GetAllBindings(Type type)
        {
            ConcurrentGrowList<IBinding> bindings;
            if (_allBindings.TryGetValue(type, out bindings))
                return bindings.GetSnapshot();
            // return default
            var defaultBinding = _defaultBindings.GetOrAdd(type, CreateDefaultBinding);
            return defaultBinding != null ? new[] {defaultBinding} : TypeHelper<IBinding>.EmptyArray;
        }
    }
}