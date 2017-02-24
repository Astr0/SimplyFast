using System;
using SimplyFast.Pool.Internal;

namespace SimplyFast.Pool
{
    public static class PooledEx
    {
        private static Func<IPooled<T>> NoInit<T>(Func<IPooled<T>> get)
        {
            return get;
        }

        private static void NoCleanup<T>(T instance)
        {
        }

        public static PooledFactory<TInit> Factory<T, TInit>(Func<T> activator, Func<Func<IPooled<T>>, TInit> makeInit, Action<T> cleanup = null)
        {
            return returnToPool =>
            {
                var instance = activator();
                var pooled = new BasicPooled<T, TInit>(instance, cleanup ?? NoCleanup, returnToPool);
                var init = makeInit(pooled.Get);
                pooled.SetGetter(init);
                return init;
            };
        }

        public static PooledFactory<TInit> Factory<T, TInit>(Func<Func<IPooled<T>>, TInit> makeInit,
            Action<T> cleanup = null) where T: new()
        {
            return Factory(() => new T(), makeInit, cleanup);
        }

        public static PooledFactory<Func<IPooled<T>>> Factory<T>(Func<T> activator, Action<T> cleanup = null)
        {
            return Factory(activator, NoInit, cleanup);
        }

        public static PooledFactory<Func<IPooled<T>>> Factory<T>(Action<T> cleanup = null) where T: new()
        {
            return Factory(() => new T(), cleanup);
        }

        public static IPooled<T> NotPooled<T>(T instance)
        {
            return new NotPooledWrapper<T>(instance);
        }

        public static InvalidOperationException InPoolException()
        {
            return new InvalidOperationException("In pool");
        }

        public static InvalidOperationException NotInPoolException()
        {
            return new InvalidOperationException("Not In pool");
        }
    }
}