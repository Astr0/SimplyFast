using System;

namespace SimplyFast.Pool.Internal
{
    internal class BasicPooled<T, TGetter>: PooledBase<T, TGetter>
    {
        private readonly Action<T> _cleanup;
        private TGetter _getter;
        private readonly T _instance;

        public BasicPooled(T instance, Action<T> cleanup, ReturnToPool<TGetter> returnToPool): base(returnToPool)
        {
            _instance = instance;
            _cleanup = cleanup;
        }

        public void SetGetter(TGetter getter)
        {
            _getter = getter;
        }

        public IPooled<T> Get()
        {
            GotFromPool();
            return this;
        }

        protected override T GetInstance()
        {
            return _instance;
        }

        protected override void ReturnToPool(ReturnToPool<TGetter> returnToPool)
        {
            _cleanup(_instance);
            returnToPool(_getter);
        }
    }
}