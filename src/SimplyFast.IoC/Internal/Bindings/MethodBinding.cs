using System;

namespace SimplyFast.IoC.Internal.Bindings
{
    internal class MethodBinding<T> : IBinding<T>
    {
        private readonly Func<IGetKernel, T> _method;

        public MethodBinding(Func<IGetKernel, T> method)
        {
            _method = method;
        }

        public object Get(IGetKernel kernel)
        {
            return _method(kernel);
        }
    }
}