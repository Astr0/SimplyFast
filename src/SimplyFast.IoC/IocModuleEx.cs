using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SimplyFast.Reflection;

namespace SimplyFast.IoC
{
    public static class IocModuleEx
    {
        public static void Load(this IKernel kernel, params IIocModule[] modules)
        {
            Load(kernel, (IEnumerable<IIocModule>) modules);
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public static void Load(this IKernel kernel, IEnumerable<IIocModule> modules)
        {
            foreach (var module in modules)
                module.Load(kernel);
        }

        public static void Load(this IKernel kernel, Assembly assembly)
        {
            kernel.Load(GetAssemblyModuleCandidates(assembly)
                .Select(t => t.Constructor())
                .Where(c => c != null)
                .Select(c => c.InvokerAs<Func<IIocModule>>()()));
        }

        private static IEnumerable<Type> GetAssemblyModuleCandidates(Assembly assembly)
        {
            return assembly.GetExportedTypes().Where(IsModuleCandidate);
        }

        public static void LoadParallel(this IKernel kernel, Assembly assembly)
        {
            Parallel.ForEach(GetAssemblyModuleCandidates(assembly), t =>
            {
                var constructor = t.Constructor();
                if (constructor == null)
                    return;
                var module = constructor.InvokerAs<Func<IIocModule>>()();
                module.Load(kernel);
            });
        }

        public static void LoadParallel(this IKernel kernel, IEnumerable<Assembly> assembly)
        {
            Parallel.ForEach(assembly, kernel.LoadParallel);
        }

        private static bool IsModuleCandidate(Type type)
        {
            return
                !type.IsAbstract &&
                !type.IsInterface &&
                !type.IsGenericTypeDefinition &&
                typeof(IIocModule).IsAssignableFrom(type);
        }
    }
}