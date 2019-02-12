using System;

namespace SimplyFast.Serialization
{
    public sealed class RepeatedType<T>
    {
        public readonly RepeatedBaseType BaseType;
        public readonly Func<IInputStream, T> ReadElement;
        public readonly Action<IOutputStream, T> WriteElement;

        public RepeatedType(RepeatedBaseType baseType,
            Func<IInputStream, T> readElement, Action<IOutputStream, T> writeElement)
        {
            BaseType = baseType;
            ReadElement = readElement;
            WriteElement = writeElement;
        }
    }
}