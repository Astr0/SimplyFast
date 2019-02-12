using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SimplyFast.IoC.Internal.Reflection;
using SimplyFast.Reflection;

namespace SimplyFast.IoC.Internal.Bindings
{
    internal static class DefaultBindingBuilder
    {
        private static readonly HashSet<Type> _noDefaultBindings = new HashSet<Type>
        {
            typeof(string),
            typeof(IntPtr)
        };


        private static readonly ConcurrentDictionary<Type,  FastConstructor[]> _constructorCache =
            new ConcurrentDictionary<Type,  FastConstructor[]>();

        //public static IBinding CreateDefaultBinding<T>(IArgKernel kernel)
        //{
        //    return CreateDefaultBinding(typeof(T), kernel);
        //}


        private static FastConstructor ChooseConstructor(FastConstructor[] paramConstructors, IArgKernel kernel,
            Func<FastConstructor, bool> filter = null)
        {
            var constructors = filter != null ? paramConstructors.Where(filter) : paramConstructors;

            foreach (var constructor in constructors)
            {
                var cantBind = constructor.Parameters.CantBindFirst(kernel);
                if (cantBind == null)
                    return constructor;
                Debug.Print("ChooseConstructor: Can't use constructor {0} for building type {1}: Failed to bind parameter {2}",
                    constructor,
                    constructor.ConstructorInfo.DeclaringType,
                    cantBind);
            }

            return null;
            //throw Exception("No suitable constructor found for {0}(implementation for {1})", impl, type);
        }

        private static FastConstructor[] GetConstructors(Type impl)
        {
            return _constructorCache.GetOrAdd(impl, BuildConstructors);
        }

        private static FastConstructor[] BuildConstructors(Type impl)
        {
            if (impl.IsAbstract)
                return null;
            if (impl.IsInterface)
                return null;

            // find good constructor
            var constructors = impl.Constructors();

            if (constructors.Length == 0)
                return null;

            // sort constructors by public then private and descending parameter count
            var sorted = constructors
                .Where(x => !x.IsStatic)
                .Select(x => new FastConstructor(x))
                .OrderByDescending(x => x.ConstructorInfo.IsPublic)
                .ThenByDescending(x => x.Parameters.Length)
                .ToArray();

            return sorted;
        }

        public static IBinding CreateDefaultBinding(Type impl, IArgKernel kernel,
            Func<FastConstructor, bool> filter = null)
        {
            if (_noDefaultBindings.Contains(impl))
                return null;

            var constructors = GetConstructors(impl);

            if (constructors == null)
                return null;

            var constructor = ChooseConstructor(constructors, kernel, filter);

            if (constructor == null && filter != null)
                constructor = ChooseConstructor(constructors, kernel);

            if (constructor == null)
                return null;

            var injector = kernel.GetInjector(impl);
            return new ConstructorWithInjectBinding(constructor, injector);
        }
    }
}