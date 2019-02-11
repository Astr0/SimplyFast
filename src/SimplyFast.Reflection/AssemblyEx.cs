using System;
using System.Collections.Generic;
using System.Reflection;

namespace SimplyFast.Reflection
{
    public static class AssemblyEx
    {
        public static IEnumerable<Assembly> GetAllAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}
