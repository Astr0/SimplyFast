using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SimplyFast.Reflection;

namespace SimplyFast.IoC
{
    public static class FastModuleEx
    {
        public static void Load(this IKernel kernel, params IFastModule[] modules)
        {
            Load(kernel, (IEnumerable<IFastModule>) modules);
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public static void Load(this IKernel kernel, IEnumerable<IFastModule> modules)
        {
            foreach (var module in modules)
                module.Load(kernel);
        }

        public static void Load(this IKernel kernel, Assembly assembly)
        {
            kernel.Load(GetAssemblyModuleCandidates(assembly)
                .Select(t => t.Constructor())
                .Where(c => c != null)
                .Select(c => c.InvokerAs<Func<IFastModule>>()()));
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
                var module = constructor.InvokerAs<Func<IFastModule>>()();
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
                typeof(IFastModule).IsAssignableFrom(type);
        }
    }
}