using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using SF.IoC.Reflection;
using SF.Reflection;

namespace SF.IoC.Injection
{
    internal static class DefaultInjectorBuilder
    {
        private static readonly ConcurrentDictionary<Type, FastMethod[]> _methodCache =
            new ConcurrentDictionary<Type, FastMethod[]>();

        public static IInjector CreateDefaultInjector(Type type, IArgKernel kernel)
        {
            var methods = GetMethods(type);

            if (methods == null)
                return NullInjector.Instance;

            var method = ChooseMethod(methods, kernel);

            return method != null ? (IInjector) new Injector(method) : NullInjector.Instance;
        }

        private static FastMethod ChooseMethod(FastMethod[] methods, IArgKernel kernel)
        {
            foreach (var method in methods)
            {
                var cantBind = method.Parameters.CantBindFirst(kernel);
                if (cantBind == null)
                    return method;

                //Log.Debug("TinyIoc: Can't use method {0} for injecting type {1}: Failed to bind parameter {2}",
                //    method,
                //    method.FastType,
                //    cantBind);
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
            return method.GetCustomAttribute<FastInjectAttribute>(true) != null && !method.IsStatic;
        }
    }
}