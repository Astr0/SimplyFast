namespace SF.Pool
{
    /// <summary>
    /// Object pool interface
    /// </summary>
    public interface IPool<T>
    {
        /// <summary>
        /// Gets instance of an object
        /// </summary>
        T Get();
        /// <summary>
        /// Returns instance of an object to a pool
        /// </summary>
        bool Return(T instance);
    }
}