using System;
using System.Collections.Generic;
using System.Reflection;

namespace SimplyFast.Reflection
{
    public static class AssemblyEx
    {
        static AssemblyEx()
        {
            SetAssemblyLocator(DefaultLocator);
        }

        private static IEnumerable<Assembly> DefaultLocator()
        {
#if NET
            return AppDomain.CurrentDomain.GetAssemblies();
#elif ASSEMBLIES
            var entry = Assembly.GetEntryAssembly();
            yield return entry;

            var references = entry.GetReferencedAssemblies();
            foreach (var reference in references)
            {
                yield return Assembly.Load(reference);
            }
#else
            throw new InvalidOperationException("Assembly locator not set. Use SetAssemblyLocator first.");
#endif
        }

#if ASSEMBLIES || NET
        public static bool NeedsAssemblyLocator => false;
#else
        public static bool NeedsAssemblyLocator => true;
#endif

        private static Func<IEnumerable<Assembly>> _assemblyLocator;

        public static void SetAssemblyLocator(Func<IEnumerable<Assembly>> locator)
        {
            _assemblyLocator = locator ?? DefaultLocator;
        }

        public static IEnumerable<Assembly> GetAllAssemblies()
        {
            return _assemblyLocator();
        }
    }
}
