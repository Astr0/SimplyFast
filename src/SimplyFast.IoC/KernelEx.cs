using System;
using SimplyFast.IoC.Internal;
using SimplyFast.IoC.Internal.DefaultBuilders;

namespace SimplyFast.IoC
{
    public static class KernelEx
    {
        public static IKernel Create()
        {
            var kernel = new Kernel();
            FuncFactoryDefaultBuilder.Register(kernel);
            CollectionDefaultBuilder.Register(kernel);
            return kernel;
        }

        public static void Inject(this IGetKernel kernel, object instance)
        {
            if (instance == null)
                return;
            kernel.GetInjector(instance.GetType())?.Invoke(kernel, instance);
        }

        public static Binding GetBinding(this IGetKernel kernel, Type type)
        {
            // user binding has priority
            return kernel.Root.GetUserBinding(type) ?? kernel.GetDefaultBinding(type);
        }

        public static bool CanBind(this IGetKernel kernel, Type type)
        {
            return kernel.GetBinding(type) != null;
        }

        public static object Get(this IGetKernel kernel, Type type)
        {
            // Binding should inject - for singletons, consts, etc.
            var binding = kernel.GetBinding(type);
            if (binding == null)
                throw new InvalidOperationException("Can't Get: " + type + " no binding found");
            return binding(kernel);
        }

        public static object GetDefault(this IGetKernel kernel, Type type)
        {
            var binding = kernel.GetDefaultBinding(type);
            if (binding == null)
                throw new InvalidOperationException("Can't GetDefault: " + type + " no default binding found");
            return binding(kernel);
        }

        public static T Get<T>(this IGetKernel kernel)
        {
            return (T) kernel.Get(typeof(T));
        }

        // For syntax compatibility with NInject
        public static T Inject<T>(this IGetKernel kernel)
        {
            return kernel.Get<T>();
        }

        public static object Get(this IGetKernel kernel, Type type, params BindArg[] args)
        {
            if (args == null || args.Length == 0)
                return kernel.Get(type);
            var argKernel = new ArgKernel(kernel, args);
            return argKernel.Get(type);
        }

        public static T Get<T>(this IGetKernel kernel, params BindArg[] args)
        {
            return (T) kernel.Get(typeof(T), args);
        }

        /// <summary>
        /// Returns binding for argument
        /// </summary>
        private static Binding GetArgBinding(this IGetKernel kernel, Type type, string name)
        {
            return kernel.GetOverrideBinding(type, name) ?? kernel.GetBinding(type);
        }

        public static bool CanBindArg(this IGetKernel kernel, Type type, string name)
        {
            return kernel.GetArgBinding(type, name) != null;
        }

        public static object GetArg(this IGetKernel kernel, Type type, string name)
        {
            var binding = kernel.GetArgBinding(type, name);
            if (binding == null)
                throw new InvalidOperationException("Can't GetArg: " + type + " for arg " + name + " no binding found");
            return binding(kernel);
        }
    }
}