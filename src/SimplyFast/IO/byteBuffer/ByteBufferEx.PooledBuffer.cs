using SF.Pool;

namespace SF.IO
{
    public delegate IPooled<ByteBuffer> GetPooledBuffer(int minSize);


    public static partial class ByteBufferEx
    {
        public static GetPooledBuffer PooledFactory(ReturnToPool<GetPooledBuffer> returnToPool)
        {
            var pooled = new PooledBuffer(returnToPool);
            return pooled.GetFromPool;
        }

        private class PooledBuffer : PooledBase<ByteBuffer, GetPooledBuffer>
        {
            private readonly ByteBuffer _instance;

            public PooledBuffer(ReturnToPool<GetPooledBuffer> returnToPool)
                : base(returnToPool)
            {
                _instance = new ByteBuffer();
            }

            public IPooled<ByteBuffer> GetFromPool(int minSize)
            {
                GotFromPool();
                _instance.Reset(minSize);
                return this;
            }

            protected override void ReturnToPool(ReturnToPool<GetPooledBuffer> returnToPool)
            {
                returnToPool(GetFromPool);
            }

            protected override ByteBuffer GetInstance()
            {
                return _instance;
            }
        }
    }
}