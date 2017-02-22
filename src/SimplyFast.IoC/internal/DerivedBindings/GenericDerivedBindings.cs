﻿using System;
using System.Collections.Concurrent;
using System.Reflection;
using SF.Reflection;

namespace SF.IoC.DerivedBindings
{
    public class GenericDerivedBindings: IDerivedBinding
    {
        private readonly ConcurrentDictionary<Type, IGenericDerivedBinding> _bindings = new ConcurrentDictionary<Type, IGenericDerivedBinding>();

        public void Add(Type genericTypeDefinition, IGenericDerivedBinding binding)
        {
            _bindings[genericTypeDefinition] = binding;
        }

        public IBinding TryBind(Type type)
        {
            Type definition;
            Type arg;
            if (type.IsGenericType)
            {
                var args = type.GenericTypeArguments;
                if (args.Length != 1)
                    return null;
                definition = type.GetGenericTypeDefinition();
                arg = args[0];
            }
            else if (type.IsArray)
            {
                definition = typeof(Array);
                arg = type.GetElementType();
                if (arg == null)
                    return null;
            }
            else
            {
                return null;
            }
            IGenericDerivedBinding binding;
            if (!_bindings.TryGetValue(definition, out binding))
                return null;
            var invoker = GetInvoker(arg);
            return invoker(binding);
        }

        private static readonly ConcurrentDictionary<Type, Func<IGenericDerivedBinding, IBinding>> _invokers = new ConcurrentDictionary<Type, Func<IGenericDerivedBinding, IBinding>>();
        private static Func<IGenericDerivedBinding, IBinding> GetInvoker(Type type)
        {
            return _invokers.GetOrAdd(type, CreateInvoker);
        }

        private static readonly MethodInfo TryBindGeneric = typeof(IGenericDerivedBinding).Methods()[0];

        private static Func<IGenericDerivedBinding, IBinding> CreateInvoker(Type type)
        {
            var generic = TryBindGeneric.MakeGeneric(type);
            return generic.InvokerAs<Func<IGenericDerivedBinding, IBinding>>();
        }
    }
}