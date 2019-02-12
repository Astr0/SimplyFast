using System.Diagnostics.CodeAnalysis;
using SimplyFast.IoC.Internal;

namespace SimplyFast.IoC
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
    public static class KernelEx
    {        
        public static IKernel Create()
        {
            return new FastKernel();
        }

        public static void Bind<T>(this IKernel kernel, IBinding<T> binding)
        {
            kernel.Bind(typeof(T), binding);
        }

        public static bool TryBind<T>(this IKernel kernel, IBinding<T> binding)
        {
            return kernel.TryBind(typeof(T), binding);
        }

        // For syntax compatibility with ninject
        public static T Inject<T>(this IGetKernel kernel)
        {
            return (T) kernel.Get(typeof(T));
        }

        public static T Get<T>(this IGetKernel kernel)
        {
            return (T) kernel.Get(typeof(T));
        }

        public static T Get<T>(this IGetKernel kernel, params BindArg[] args)
        {
            return (T) kernel.Get(typeof(T), args);
        }

        public static T Arg<T>(this IArgKernel kernel, string name)
        {
            return (T) kernel.Arg(typeof(T), name);
        }
    }
}