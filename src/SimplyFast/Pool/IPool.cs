namespace SF.Pool
{
    public interface IPool<T>
    {
        T Get();
        void Return(T instance);
    }
}