using System;
using System.IO;
using SimplyFast.IO;
using SimplyFast.Pool;
using SimplyFast.Reflection;
using SimplyFast.Serialization.Protobuf;

namespace SimplyFast.Serialization
{
    public static class ProtoSerializer
    {
        private const int DefaultBufferSize = 4096;

        private static Pooled<ByteBuffer> ToBuffer(this Stream stream)
        {
            // TODO .Net 4.6
            //var ms = stream as MemoryStream;
            //if (ms != null)
            //{
            //    ArraySegment<byte> msBuffer;
            //    if (ms.TryGetBuffer(out msBuffer))
            //    {
            //        return new PooledBuffer(msBuffer.Array, msBuffer.Offset, msBuffer.Count);
            //    }
            //}

            int count;
            Pooled<ByteBuffer> pooled;
            bool hasLength;
            if (stream.CanSeek)
            {
                hasLength = true;
                count = (int)(stream.Length - stream.Position);
                pooled = SerializerBuffers.Get(count);
            }
            else
            {
                hasLength = false;
                pooled = SerializerBuffers.Get(DefaultBufferSize);
                count = pooled.Instance.BufferLength;
            }
            var buf = pooled.Instance;
            var offset = 0;
            while (true)
            {
                var read = stream.Read(buf.Buffer, offset, count);
                // exit on EOF
                if (read == 0)
                    break;
                offset += read;
                count -= read;
                // we have more to read
                if (count != 0)
                    continue;
                // if length was known - just break
                if (hasLength)
                    break;
                // grow buffer and set new count
                buf.Grow();
                count = buf.BufferLength - offset;
            }
            // set actual bytes read
            buf.SetView(0, offset);
            return pooled;
        }

        public static byte[] Serialize<T>(T item) where T : IMessage
        {
            var calcSize = new ProtoSizeCalc(item);
            var result = new byte[calcSize.Size];
            var stream = new ProtoOutputStream(calcSize, result);
            item.WriteTo(stream);
            return result;
        }

        public static void Serialize<T>(Stream stream, T item) where T : IMessage
        {
            using (var pooled = SerializePooled(item))
            {
                var buffer = pooled.Instance;
                stream.Write(buffer.Buffer, buffer.Offset, buffer.Count);
            }
        }

        public static Pooled<ByteBuffer> SerializePooled<T>(T item) where T : IMessage
        {
            var calcSize = new ProtoSizeCalc(item);
            var result = SerializerBuffers.Get(calcSize.Size);
            var buf = result.Instance;
            var stream = new ProtoOutputStream(calcSize, buf.Buffer);
            item.WriteTo(stream);
            buf.SetView(0, calcSize.Size);
            return result;
        }

        public static T Deserialize<T>(byte[] buffer, int index, int count)
            where T : IMessage, new()
        {
            var stream = new ProtoInputStream(buffer, index, count);
            var result = new T();
            result.ReadFrom(stream);
            return result;
        }

        public static T Deserialize<T>(byte[] buffer)
            where T : IMessage, new()
        {
            return Deserialize<T>(buffer, 0, buffer.Length);
        }

        public static T Deserialize<T>(Stream stream)
            where T : IMessage, new()
        {
            using (var pooled = stream.ToBuffer())
            {
                var buf = pooled.Instance;
                return Deserialize<T>(buf.Buffer, buf.Offset, buf.Count);
            }
        }

        public static object Deserialize(Type messageType, Stream stream)
        {
            if (!typeof(IMessage).IsAssignableFrom(messageType))
                throw new ArgumentException("Type " + messageType + " is not IMessage");
            var instance = (IMessage)messageType.CreateInstance();
            using (var pooled = stream.ToBuffer())
            {
                var buf = pooled.Instance;
                var input = new ProtoInputStream(buf.Buffer, buf.Offset, buf.Count);
                instance.ReadFrom(input);
                return instance;
            }
        }
    }
}