using System;
using System.Diagnostics.CodeAnalysis;

namespace SimplyFast.IoC
{
    public class BindingBuilder<T> 
    {
        private Binding _binding;
        private bool _called;

        internal BindingBuilder()
        {
        }

        public Binding Binding
        {
            get => _binding;
            set
            {
                if (_called)
                    throw new InvalidOperationException("Can't change binding after it's been called for type: " +
                                                        typeof(T).FullName);
                _binding = value;
            }
        }

        internal object Get(IGetKernel kernel)
        {
            if (_called)
                return _binding(kernel);

            if (_binding == null)
                throw new InvalidOperationException("No specific binding defined for type: " + typeof(T).FullName);
            _called = true;
            return _binding(kernel);
        }

        public BindingBuilder<T> To<TImpl>()
            where TImpl : T
        {
            CheckBound(false);
            if (typeof(TImpl) == typeof(T))
            {
                Binding = k => k.GetDefault(typeof(T));
            }
            else
            {
                Binding = k => k.Get(typeof(TImpl));
            }

            return this;
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Global")]
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