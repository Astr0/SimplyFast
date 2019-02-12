using System;
using System.Diagnostics.CodeAnalysis;

namespace SimplyFast.IoC
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
    public static class KernelBindEx
    {
        public static BindingBuilder<T> Bind<T>(this IKernel kernel)
        {
            var builder = new BindingBuilder<T>();
            kernel.Bind(typeof(T), builder.Get);
            return builder;
        }

        private static BindingBuilder<T> SetBinding<T>(this BindingBuilder<T> builder, Binding binding)
        {
            builder.Binding = binding;
            return builder;
        }

        public static void ToConstant<T>(this BindingBuilder<T> builder, T value)
        {
            builder.CheckBound(false);
            builder.SetBinding(c => value);
        }

        public static BindingBuilder<T> ToSelf<T>(this BindingBuilder<T> builder)
        {
            return builder.To<T>();
        }

        public static BindingBuilder<T> ToMethod<T>(this BindingBuilder<T> builder, Func<IGetKernel, T> method)
        {
            builder.CheckBound(false);
            return builder.SetBinding(c => method(c));
        }

        // For syntax compatibility with NInject
        public static BindingBuilder<T> ToConstructor<T>(this BindingBuilder<T> builder, Func<IGetKernel, T> method)
        {
            return ToMethod(builder, method);
        }

        public static BindingBuilder<T> InSingletonScope<T>(this BindingBuilder<T> builder)
        {
            builder.CheckBound(true);
            var binding = new SingletonBinding(builder.Binding);
            return builder.SetBinding(binding.Get);
        }

        private class SingletonBinding
            //where T : class
        {
            private readonly Binding _firstCall;
            private volatile bool _firstCalled;
            private volatile object _value;

            public SingletonBinding(Binding firstCall)
            {
                _firstCall = firstCall;
            }

            public object Get(IGetKernel kernel)
            {
                if (_firstCalled)
                    return _value;
                lock (_firstCall)
                {
                    if (_firstCalled)
                        return _value;
                    // execute on root kernel!
                    _value = _firstCall(kernel.Root);
                    _firstCalled = true;
                }
                return _value;
            }
        }
    }
}