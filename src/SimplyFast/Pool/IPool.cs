using SimplyFast.Cache;

namespace SimplyFast.Pool
{
    public delegate T InitPooled<T, in TParam>(T instance, TParam param);

    public delegate void ReturnToPoll<in T>(T item);

    public interface IPoolBase<in T> : IHasCacheStat
    {
        void Return(T item);
    }

    public interface IPool<T, in TParam> : IPoolBase<T>
    {
        Pooled<T> Get(TParam param = default);
    }
}