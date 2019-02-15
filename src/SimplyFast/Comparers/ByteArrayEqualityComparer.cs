using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;

namespace SimplyFast.Comparers
{
    internal class ByteArrayEqualityComparer: EqualityComparer<byte[]>
    {
        public static readonly EqualityComparer<byte[]> Instance = new ByteArrayEqualityComparer();

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public override unsafe bool Equals(byte[] strA, byte[] strB)
        {
            if (ReferenceEquals(strA, strB)) return true;
            if (null == strA) return false;
            if (null == strB) return false;

            // TODO: Use ReadonlySpan<T>.SequenceEquals?
            var length = strA.Length;
            if (length != strB.Length)
                return false;

            fixed (byte* fixedA = strA)
            fixed (byte* fixedB = strB)
            {
                var ptrA = fixedA;
                var ptrB = fixedB;
                while (length >= 4)
                {
                    if (*(int*)ptrA != *(int*)ptrB)
                        return false;
                    ptrA += 4;
                    ptrB += 4;
                    length -= 4;
                }
                switch (length)
                {
                    case 0:
                        return true;
                    case 1:
                        return *ptrA == *ptrB;
                    case 2:
                        return *(short*)ptrA == *(short*)ptrB;
                    case 3:
                        return *(short*)ptrA == *(short*)ptrB && *(ptrA + 2) == *(ptrB + 2);
                    default:
                        return true;
                }
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public override unsafe int GetHashCode(byte[] obj)
        {
            if (obj == null)
                return 0;
            unchecked
            {
                var hashCode = 0;
                var length = obj.Length;
                fixed (byte* f = obj)
                {
                    var p = f;
                    while (length >= 4)
                    {
                        hashCode = (hashCode * 397) ^ *(int*) p;
                        p += 4;
                        length -= 4;
                    }
                    switch (length)
                    {
                        case 0:
                            return hashCode;
                        case 1:
                            return (hashCode * 397) ^ *p;
                        case 2:
                            return (hashCode * 397) ^ *(short*)p;
                        case 3:
                            return (hashCode * 397) ^ *(short*)p ^ (*(p + 2) << 2);
                        default:
                            return hashCode;
                    }
                }
            }
        }
    }
}