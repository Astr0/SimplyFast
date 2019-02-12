using System;
using System.Linq;
using System.Reflection;

namespace SimplyFast.IoC.Internal.Reflection
{
    internal static class ReflectionHelper
    {
        public static ParameterInfo CantBindFirst(this ParameterInfo[] parameters, IArgKernel kernel)
        {
            return parameters.FirstOrDefault(p => !kernel.CanBind(p.ParameterType, p.Name));
        }

        public static object[] GetValues(this ParameterInfo[] parameters, IArgKernel kernel)
        {
            return Array.ConvertAll(parameters, p => kernel.Arg(p.ParameterType, p.Name));
        }

        public static object Invoke(this FastConstructor constructor, IArgKernel kernel)
        {
            var args = constructor.Parameters.GetValues(kernel);
            return constructor.Invoke(args);
        }

        public static void Invoke(this FastMethod method, object instance, IArgKernel kernel)
        {
            var args = method.Parameters.GetValues(kernel);
            method.Invoke(instance, args);
        }
    }
}