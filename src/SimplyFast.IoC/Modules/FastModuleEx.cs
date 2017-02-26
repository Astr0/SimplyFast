using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using SimplyFast.Reflection;

#if PARALLEL
using System.Threading.Tasks;
#endif

namespace SimplyFast.IoC.Modules
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
            return assembly.ExportedTypes.Where(IsModuleCandidate);
        }

#if PARALLEL
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
#endif

        private static readonly TypeInfo _iFastModule = typeof(IFastModule).TypeInfo();

        private static bool IsModuleCandidate(Type type)
        {
            var ti = type.TypeInfo();
            return
                !ti.IsAbstract &&
                !ti.IsInterface &&
                !ti.IsGenericTypeDefinition &&
                _iFastModule.IsAssignableFrom(ti);
        }
    }
}