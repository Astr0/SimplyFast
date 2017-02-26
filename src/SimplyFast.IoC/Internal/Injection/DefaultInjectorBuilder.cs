using System;
using System.Linq;
using System.Reflection;
using SimplyFast.Cache;
using SimplyFast.IoC.Internal.Reflection;
using SimplyFast.Reflection;

namespace SimplyFast.IoC.Internal.Injection
{
    internal static class DefaultInjectorBuilder
    {
        private static readonly ICache<Type, FastMethod[]> _methodCache =
            CacheEx.ThreadSafe<Type, FastMethod[]>();

        public static IInjector CreateDefaultInjector(Type type, IGetKernel kernel)
        {
            var methods = GetMethods(type);

            if (methods == null)
                return NullInjector.Instance;

            var method = ChooseMethod(methods, kernel);

            return method != null ? (IInjector) new Injector(method) : NullInjector.Instance;
        }

        private static FastMethod ChooseMethod(FastMethod[] methods, IGetKernel kernel)
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