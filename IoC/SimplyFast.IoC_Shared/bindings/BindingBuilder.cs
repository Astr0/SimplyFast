using System;
using System.Diagnostics.CodeAnalysis;
using SF.IoC.Bindings;

namespace SF.IoC
{
    public class BindingBuilder<T> : IBinding<T>
    {
        private IBinding<T> _binding;
        private bool _called;

        public IBinding<T> Binding
        {
            get { return _binding; }
            set
            {
                if (_called)
                    throw new InvalidOperationException("Can't change binding after it's been called for type: " +
                                                        typeof(T).FullName);
                _binding = value;
            }
        }

        public object Get(IGetKernel kernel)
        {
            if (_called)
                return _binding.Get(kernel);

            if (_binding == null)
                throw new InvalidOperationException("No specific binding defined for type: " + typeof(T).FullName);
            _called = true;
            return _binding.Get(kernel);
        }

        public BindingBuilder<T> To<TImpl>()
            where TImpl : T
        {
            CheckBound(false);
            if (typeof(TImpl) == typeof(T))
                Binding = new DefaultBinding<TImpl, T>();
            else
                Binding = new MethodBinding<T>(c => c.Get<TImpl>());
            return this;
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        internal void CheckBound(bool bound)
        {
            var hasBinding = _binding != null;
            if (!bound && hasBinding)
                throw new InvalidOperationException("Already bound");
            if (bound && !hasBinding)
                throw new InvalidOperationException("Not bound yet");
        }
    }
}