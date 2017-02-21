using System;

namespace SF.IoC.Bindings
{
    internal class MethodBinding<T> : IBinding<T>
    {
        private readonly Func<IArgKernel, T> _method;

        public MethodBinding(Func<IArgKernel, T> method)
        {
            _method = method;
        }

        public object Get(IArgKernel kernel)
        {
            return _method(kernel);
        }
    }
}