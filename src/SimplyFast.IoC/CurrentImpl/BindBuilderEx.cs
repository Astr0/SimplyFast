using System;
using SimplyFast.IoC.Internal.Bindings;

namespace SimplyFast.IoC
{
    public static class BindBuilderEx
    {
        public static BindingBuilder<T> Bind<T>(this IKernel kernel)
        {
            var builder = new BindingBuilder<T>();
            kernel.Bind(builder);
            return builder;
        }

        private static BindingBuilder<T> SetBinding<T>(this BindingBuilder<T> builder, IBinding<T> binding)
        {
            builder.Binding = binding;
            return builder;
        }

        public static void ToConstant<T>(this BindingBuilder<T> builder, T value)
        {
            builder.CheckBound(false);
            builder.SetBinding(new ConstBinding<T>(value));
        }

        public static BindingBuilder<T> ToSelf<T>(this BindingBuilder<T> builder)
        {
            return builder.To<T>();
        }

        public static BindingBuilder<T> ToMethod<T>(this BindingBuilder<T> builder, Func<IGetKernel, T> method)
        {
            builder.CheckBound(false);
            return builder.SetBinding(new MethodBinding<T>(method));
        }

        // For syntax compatibility with ninject
        public static BindingBuilder<T> ToConstructor<T>(this BindingBuilder<T> builder, Func<IGetKernel, T> method)
        {
            return ToMethod(builder, method);
        }

        public static BindingBuilder<T> InSingletonScope<T>(this BindingBuilder<T> builder)
            //where T : class
        {
            builder.CheckBound(true);
            return builder.SetBinding(new SingletonBinding<T>(builder.Binding));
        }
    }
}