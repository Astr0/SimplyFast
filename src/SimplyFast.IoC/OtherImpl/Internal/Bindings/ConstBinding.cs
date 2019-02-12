namespace SimplyFast.IoC.Internal.Bindings
{
    internal class ConstBinding : IBinding
    {
        private readonly object _value;

        public ConstBinding(object value)
        {
            _value = value;
        }

        public object Get(IArgKernel kernel)
        {
            return _value;
        }
    }

    internal class ConstBinding<T> : ConstBinding, IBinding<T>
    {
        public ConstBinding(T value) : base(value)
        {
        }
    }
}