using System;
using System.Reflection;
using SimplyFast.Collections;

namespace SimplyFast.IoC.Internal.Reflection
{
    internal static class ReflectionHelper
    {
        public static ParameterInfo CantBindFirst(this ParameterInfo[] parameters, IGetKernel kernel)
        {
            return Array.Find(parameters, p => !kernel.CanBind(p.ParameterType, p.Name));
        }

        public static object[] GetValues(this ParameterInfo[] parameters, IGetKernel kernel)
        {
            return parameters.ConvertAll(p => kernel.Arg(p.ParameterType, p.Name));
            //var pi = parameters.ParameterInfo;
            //return pi.Length <= 2 ? 
            //    Array.ConvertAll(pi, p => kernel.Arg(p.ParameterType, p.Name)) 
            //    : pi.AsParallel().Select(p => kernel.Arg(p.ParameterType, p.Name)).ToArray();
        }

        public static object Invoke(this FastConstructor constructor, IGetKernel kernel)
        {
            var args = constructor.Parameters.GetValues(kernel);
            return constructor.Invoke(args);
        }

        public static void Invoke(this FastMethod method, object instance, IGetKernel kernel)
        {
            var args = method.Parameters.GetValues(kernel);
            method.Invoke(instance, args);
        }
    }
}