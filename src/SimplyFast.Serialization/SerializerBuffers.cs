using SimplyFast.IO;
using SimplyFast.Pool;

namespace SimplyFast.Serialization
{
    internal static class SerializerBuffers
    {
        private static readonly IPool<GetPooledBuffer> _bufferPool =
            PoolEx.ThreadSafe<GetPooledBuffer>(ByteBufferEx.PooledFactory);

        public static IPooled<ByteBuffer> Get(int minSize)
        {
            return _bufferPool.Get(minSize);
        }
    }
}