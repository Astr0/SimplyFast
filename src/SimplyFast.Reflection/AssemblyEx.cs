using System.Collections.Generic;
using System.Reflection;
#if EMIT
using System;
using System.Reflection.Emit;
#endif

namespace SimplyFast.Reflection
{
    public static class AssemblyEx
    {
        public static IEnumerable<Assembly> GetAllAssemblies()
        {
#if NET
            return AppDomain.CurrentDomain.GetAssemblies();
#else
            var entry = Assembly.GetEntryAssembly();
            yield return entry;

            var references = entry.GetReferencedAssemblies();
            foreach (var reference in references)
            {
                yield return Assembly.Load(reference);
            }
#endif
        }
#if EMIT
        private static readonly object _dynamicAssemblyLock = new object();
        private static volatile AssemblyBuilder _dynamicAssembly;

        public static AssemblyBuilder DynamicAssembly
        {
            get
            {
                if (_dynamicAssembly != null)
                    return _dynamicAssembly;
                lock (_dynamicAssemblyLock)
                {
                    if (_dynamicAssembly != null)
                        return _dynamicAssembly;
                    var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("SimplyFast.Reflection.Dynamic"), AssemblyBuilderAccess.Run);
                    _dynamicAssembly = assembly;
                    return assembly;
                }
            }
        }
#endif
    }
}
