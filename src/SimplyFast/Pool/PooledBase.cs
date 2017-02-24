using System.Threading;

#pragma warning disable 420

namespace SimplyFast.Pool
{
    public abstract class PooledBase<T, TGetter>: IPooled<T>
    {
        private readonly ReturnToPool<TGetter> _returnToPool;
        private volatile int _inPool;

        protected PooledBase(ReturnToPool<TGetter> returnToPool)
        {
            _returnToPool = returnToPool;
            _inPool = 1;
        }

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _inPool, 1, 0) == 1)
                return;
            ReturnToPool(_returnToPool);
        }

        protected void GotFromPool()
        {
            if (Interlocked.CompareExchange(ref _inPool, 0, 1) == 0)
                throw PooledEx.NotInPoolException();
        }

        protected abstract void ReturnToPool(ReturnToPool<TGetter> returnToPool);

        public T Instance
        {
            get
            {
                if (_inPool == 1)
                    throw PooledEx.InPoolException();
                return GetInstance();
            }
        }

        protected abstract T GetInstance();
    }
}