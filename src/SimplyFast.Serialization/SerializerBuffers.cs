using System.Diagnostics.CodeAnalysis;
using SimplyFast.Cache;
using SimplyFast.IO;
using SimplyFast.Pool;

namespace SimplyFast.Serialization
{
    public static class SerializerBuffers
    {
        private static readonly IPool<ByteBuffer, int> _bufferPool =
            PoolEx.ThreadSafe<ByteBuffer, int>((x, size) =>
            {
                if (x == null)
                    x = new ByteBuffer();
                x.Reset(size);
                return x;
            });

        [SuppressMessage("ReSharper", "UnusedMember.Global")] 
        public static CacheStat CacheStat => _bufferPool.CacheStat;

        public static Pooled<ByteBuffer> Get(int minSize)
        {
            return _bufferPool.Get(minSize);
        }
    }
}