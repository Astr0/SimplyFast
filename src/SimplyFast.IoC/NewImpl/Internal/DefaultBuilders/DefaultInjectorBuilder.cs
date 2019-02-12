using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SimplyFast.IoC.Internal.Reflection;
using SimplyFast.Reflection;

namespace SimplyFast.IoC.Internal.DefaultBuilders
{
    internal static class DefaultInjectorBuilder
    {
        private static readonly ConcurrentDictionary<Type, FastMethod[]> _methodCache =
            new ConcurrentDictionary<Type, FastMethod[]>();

        private static void NullInjector(IGetKernel kernel, object o)
        {
        }

        public static Injector CreateDefaultInjector(Type type, IGetKernel kernel)
        {
            var methods = GetMethods(type);

            if (methods == null || methods.Length == 0)
                return NullInjector;

            var method = ChooseMethod(methods, kernel);

            if (method == null)
                return NullInjector;

            return (c, instance) => method.Invoke(instance, c);
        }

        private static FastMethod ChooseMethod(FastMethod[] methods, IGetKernel kernel)
        {
            foreach (var method in methods)
            {
                var cantBind = method.Parameters.CantBindFirst(kernel);
                if (cantBind == null)
                    return method;

                Debug.Print("ChooseMethod: Can't use method {0} for injecting type {1}: Failed to bind parameter {2}",
                    method,
                    method.MethodInfo.DeclaringType,
                    cantBind);
            }

            return null;
        }

        private static FastMethod[] GetMethods(Type type)
        {
            return _methodCache.GetOrAdd(type, BuildMethods);
        }

        private static FastMethod[] BuildMethods(Type type)
        {
            // find good constructor
            var methods = type.Methods()
                .Where(IsInjectMethod)
                .Select(x => new FastMethod(x))
                .OrderByDescending(x => x.Parameters.Length)
                .ToArray();

            return methods;
        }

        private static bool IsInjectMethod(MethodInfo method)
        {
            return method.GetCustomAttribute<InjectAttribute>(true) != null && !method.IsStatic;
        }
    }
}