using System;

namespace SF.IoC.Bindings
{
    internal class DefaultBinding : IBinding
    {
        private readonly Type _type;
        private IBinding _defaultBinding;

        protected DefaultBinding(Type type)
        {
            _type = type;
        }

        public object Get(IGetKernel kernel)
        {
            if (_defaultBinding != null)
                return _defaultBinding.Get(kernel);

            _defaultBinding = DefaultBindingBuilder.CreateDefaultBinding(_type, kernel);
            if (_defaultBinding != null)
                return _defaultBinding.Get(kernel);
            var message = $"Can't create default binding for {_type.FullName}.";
            throw new InvalidOperationException(message);
        }
    }

    internal class DefaultBinding<TDerived, T> : DefaultBinding, IBinding<T>
        where TDerived : T
    {
        public DefaultBinding() : base(typeof(TDerived))
        {
        }
    }
}