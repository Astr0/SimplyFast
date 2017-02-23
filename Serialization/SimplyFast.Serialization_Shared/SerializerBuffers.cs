using SF.IO;
using SF.Pool;

namespace SF.Serialization
{
    internal static class SerializerBuffers
    {
        private static readonly IPool<GetPooledBuffer> _bufferPool =
            PoolEx.Basic<GetPooledBuffer>(ByteBufferEx.PooledFactory);

        public static IPooled<ByteBuffer> Get(int minSize)
        {
            return _bufferPool.Get(minSize);
        }
    }
}