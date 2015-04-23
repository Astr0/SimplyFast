﻿using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ProtoBuf;
using SF.Pipes;

namespace SF.Protobuf
{
    internal class ProtobufConsumer<T>: IConsumer<T>
    {
        private readonly IConsumer<byte[]> _consumer;

        public ProtobufConsumer(IConsumer<byte[]> consumer)
        {
            _consumer = consumer;
        }

        public async Task<T> Take(CancellationToken cancellation = new CancellationToken())
        {
            var buf = await _consumer.Take(cancellation);
            return ProtobufEx.Deserialize<T>(buf);
        }

        public void Dispose()
        {
            _consumer.Dispose();
        }
    }
}