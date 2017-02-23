namespace SF.IoC.Bindings
{
    internal class ConstBinding : IBinding
    {
        private readonly object _value;

        public ConstBinding(object value)
        {
            _value = value;
        }

        public object Get(IGetKernel kernel)
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