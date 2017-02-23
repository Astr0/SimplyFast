namespace SF.IoC.Bindings
{
    internal class SingletonBinding<T> : IBinding<T>
    {
        private readonly IBinding<T> _firstCall;
        private volatile bool _firstCalled;
        private volatile object _value;

        public SingletonBinding(IBinding<T> firstCall)
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
                _value = _firstCall.Get(kernel);
                _firstCalled = true;
            }
            return _value;
        }
    }
}