using System;
using SF.IoC.Bindings.Args;

namespace SF.IoC
{
    public static class KernelEx
    {
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

        public static object Get(this IGetKernel kernel, Type type)
        {
            var binding = kernel.GetBinding(type);
            if (binding == null)
                throw new InvalidOperationException("Can't create: " + type + " no binding found");
            return binding.Get(kernel);
        }

        public static object Get(this IGetKernel kernel, Type type, params BindArg[] args)
        {
            var binding = kernel.GetBinding(type, args);
            if (binding == null)
                throw new InvalidOperationException("Can't create: " + TypeWithArgs.ToString(type, args) +
                                                    " no binding found");
            return binding.Get(kernel);
        }

        public static void Inject(this IGetKernel kernel, object instance)
        {
            if (instance == null)
                return;
            var injector = kernel.GetInjector(instance.GetType());
            injector?.Inject(kernel, instance);
        }

        public static object Arg(this IGetKernel kernel, Type type, string name)
        {
            var binding = kernel.GetArgBinding(type, name);
            if (binding == null)
                throw new InvalidOperationException("Can't create: " + type + " for arg " + name + " no binding found");
            return binding.Get(kernel);
        }

        public static bool CanBind(this IGetKernel kernel, Type type, string argName)
        {
            var binding = kernel.GetArgBinding(type, argName);
            return binding != null;
        }
    }
}