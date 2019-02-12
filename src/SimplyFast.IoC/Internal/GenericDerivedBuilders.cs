using System;
using System.Collections.Concurrent;
using System.Reflection;
using SimplyFast.Reflection;

namespace SimplyFast.IoC.Internal
{
    internal class GenericDerivedBuilders
    {
        private readonly ConcurrentDictionary<Type, IGenericDefaultBuilder> _builders = new ConcurrentDictionary<Type, IGenericDefaultBuilder>();

        public void Add(Type genericTypeDefinition, IGenericDefaultBuilder builder)
        {
            _builders[genericTypeDefinition] = builder;
        }

        public Binding TryBuild(Type type, IGetKernel kernel)
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
            IGenericDefaultBuilder builder;
            if (!_builders.TryGetValue(definition, out builder))
                return null;
            var invoker = GetInvoker(arg);
            return invoker(builder, kernel);
        }

        private static readonly ConcurrentDictionary<Type, Func<IGenericDefaultBuilder, IGetKernel, Binding>> _invokers 
            = new ConcurrentDictionary<Type, Func<IGenericDefaultBuilder, IGetKernel, Binding>>();
        private static Func<IGenericDefaultBuilder, IGetKernel, Binding> GetInvoker(Type type)
        {
            return _invokers.GetOrAdd(type, CreateInvoker);
        }

        private static readonly MethodInfo TryBindGeneric = typeof(IGenericDefaultBuilder).Methods()[0];

        private static Func<IGenericDefaultBuilder, IGetKernel, Binding> CreateInvoker(Type type)
        {
            var generic = TryBindGeneric.MakeGeneric(type);
            return generic.InvokerAs<Func<IGenericDefaultBuilder, IGetKernel, Binding>>();
        }
    }
}