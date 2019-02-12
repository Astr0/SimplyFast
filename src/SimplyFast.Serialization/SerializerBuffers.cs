using SimplyFast.Cache;
using SimplyFast.IO;
using SimplyFast.Pool;

namespace SimplyFast.Serialization
{
    public static class SerializerBuffers
    {
        private static readonly IPool<GetPooledBuffer> _bufferPool =
            PoolEx.ThreadSafe<GetPooledBuffer>(ByteBufferEx.PooledFactory);

        public static CacheStat CacheStat => _bufferPool.CacheStat;

        public static IPooled<ByteBuffer> Get(int minSize)
        {
            return _bufferPool.Get(minSize);
        }
    }
}