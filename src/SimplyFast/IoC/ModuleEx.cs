using System;
using System.Linq;
using System.Reflection;

namespace SF.IoC
{
    public static class ModuleEx
    {
        public static IModule[] AllInAssembly(Assembly assembly)
        {
            var moduleTypes = assembly.GetTypes()
                .Where(t => typeof (IModule).IsAssignableFrom(t) && !t.IsGenericTypeDefinition);
            var modules = moduleTypes
                .Select(type =>
                {
                    try
                    {
                        return (IModule) Activator.CreateInstance(type);
                    }
                    catch
                    {
                        return null;
                    }
                })
                .Where(x => x != null)
                .ToArray();
            return modules;
        }

        public static IModule[] All()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var modules = assemblies.SelectMany(AllInAssembly).ToArray();
            return modules;
        }
    }
}