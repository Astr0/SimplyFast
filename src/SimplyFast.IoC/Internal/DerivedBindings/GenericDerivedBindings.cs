using System;
using System.Reflection;
using SimplyFast.Cache;
using SimplyFast.Reflection;

namespace SimplyFast.IoC.Internal.DerivedBindings
{
    public class GenericDerivedBindings: IDerivedBinding
    {
        private readonly ICache<Type, IGenericDerivedBinding> _bindings = CacheEx.ThreadSafe<Type, IGenericDerivedBinding>();

        public void Add(Type genericTypeDefinition, IGenericDerivedBinding binding)
        {
            _bindings.Upsert(genericTypeDefinition, binding);
        }

        public IBinding TryBind(Type type)
        {
            Type definition;
            Type arg;
            if (type.TypeInfo().IsGenericType)
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

        private static readonly ICache<Type, Func<IGenericDerivedBinding, IBinding>> _invokers = CacheEx.ThreadSafe<Type, Func<IGenericDerivedBinding, IBinding>>();
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