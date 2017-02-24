#if EMIT
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SimplyFast.Reflection
{
    public static class AssemblyEx
    {
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
    }
}
#endif