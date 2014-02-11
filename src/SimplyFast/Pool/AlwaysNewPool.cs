namespace SF.Pool
{
    public class AlwaysNewPool<T> : IPool<T>
        where T : new()
    {
        #region IPool<T> Members

        public T Get()
        {
            return new T();
        }

        public void Return(T instance)
        {
        }

        #endregion
    }
}