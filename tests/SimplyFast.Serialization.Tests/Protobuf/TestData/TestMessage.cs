using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SimplyFast.Comparers;

namespace SimplyFast.Serialization.Tests.Protobuf.TestData
{
    public partial class FTestMessage : IEquatable<FTestMessage>
    {
        private static readonly IEqualityComparer<List<InnerMessage>> FrepComparer
            = EqualityComparerEx.Collection<List<InnerMessage>, InnerMessage>();

        private static readonly IEqualityComparer<List<TestEnum>> FrepEnumComparer
            = EqualityComparerEx.Collection<List<TestEnum>, TestEnum>();

        private static readonly IEqualityComparer<List<string>> FrepStringComparer
            = EqualityComparerEx.Collection<List<string>, string>();

        private static readonly IEqualityComparer<List<uint>> FrepFixed32Comparer
            = EqualityComparerEx.Collection<List<uint>, uint>();

        private static readonly IEqualityComparer<List<uint>> FrepUint32Comparer
            = EqualityComparerEx.Collection<List<uint>, uint>();

        private static readonly IEqualityComparer<byte[]> BytesComparer
            = EqualityComparerEx.Array<byte>();

        public bool Equals(FTestMessage other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _fdouble.Equals(other._fdouble) && _ffloat.Equals(other._ffloat) && _fint32 == other._fint32 &&
                   _fint64 == other._fint64 && _fuint32 == other._fuint32 && _fuint64 == other._fuint64 &&
                   _fsint32 == other._fsint32 && _fsint64 == other._fsint64 && _ffixed32 == other._ffixed32 &&
                   _ffixed64 == other._ffixed64 && _fsfixed32 == other._fsfixed32 && _fsfixed64 == other._fsfixed64 &&
                   _fbool == other._fbool && string.Equals(_fstring, other._fstring) &&
                   BytesComparer.Equals(_fbytes, other._fbytes) &&
                   _fenum == other._fenum && Equals(_finner, other._finner) &&
                   FrepComparer.Equals(_frep, other._frep) &&
                   FrepEnumComparer.Equals(_frepEnum, other._frepEnum) &&
                   FrepStringComparer.Equals(_frepString, other._frepString) &&
                   FrepFixed32Comparer.Equals(_frepFixed32, other._frepFixed32) &&
                   FrepUint32Comparer.Equals(_frepUint32, other._frepUint32);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((FTestMessage) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _fdouble.GetHashCode();
                hashCode = (hashCode * 397) ^ _ffloat.GetHashCode();
                hashCode = (hashCode * 397) ^ _fint32;
                hashCode = (hashCode * 397) ^ _fint64.GetHashCode();
                hashCode = (hashCode * 397) ^ _fuint32.GetHashCode();
                hashCode = (hashCode * 397) ^ _fuint64.GetHashCode();
                hashCode = (hashCode * 397) ^ _fsint32.GetHashCode();
                hashCode = (hashCode * 397) ^ _fsint64.GetHashCode();
                hashCode = (hashCode * 397) ^ _ffixed32.GetHashCode();
                hashCode = (hashCode * 397) ^ _ffixed64.GetHashCode();
                hashCode = (hashCode * 397) ^ _fsfixed32.GetHashCode();
                hashCode = (hashCode * 397) ^ _fsfixed64.GetHashCode();
                hashCode = (hashCode * 397) ^ _fbool.GetHashCode();
                hashCode = (hashCode * 397) ^ (_fstring?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ BytesComparer.GetHashCode(_fbytes);
                hashCode = (hashCode * 397) ^ _fenum.GetHashCode();
                hashCode = (hashCode * 397) ^ (_finner?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ FrepComparer.GetHashCode(_frep);
                hashCode = (hashCode * 397) ^ FrepEnumComparer.GetHashCode(_frepEnum);
                hashCode = (hashCode * 397) ^ FrepStringComparer.GetHashCode(_frepString);
                hashCode = (hashCode * 397) ^ FrepFixed32Comparer.GetHashCode(_frepFixed32);
                hashCode = (hashCode * 397) ^ FrepUint32Comparer.GetHashCode(_frepUint32);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{nameof(_fdouble)}: {_fdouble}, {nameof(_ffloat)}: {_ffloat}, {nameof(_fint32)}: {_fint32}, {nameof(_fint64)}: {_fint64}, {nameof(_fuint32)}: {_fuint32}, {nameof(_fuint64)}: {_fuint64}, {nameof(_fsint32)}: {_fsint32}, {nameof(_fsint64)}: {_fsint64}, {nameof(_ffixed32)}: {_ffixed32}, {nameof(_ffixed64)}: {_ffixed64}, {nameof(_fsfixed32)}: {_fsfixed32}, {nameof(_fsfixed64)}: {_fsfixed64}, {nameof(_fbool)}: {_fbool}, {nameof(_fstring)}: {_fstring}, {nameof(_fbytes)}: {_fbytes}, {nameof(_fenum)}: {_fenum}, {nameof(_finner)}: {_finner}, {nameof(_frep)}: {_frep}, {nameof(_frepEnum)}: {_frepEnum}, {nameof(_frepString)}: {_frepString}, {nameof(_frepFixed32)}: {_frepFixed32}, {nameof(_frepUint32)}: {_frepUint32}, {nameof(Fdouble)}: {Fdouble}, {nameof(Ffloat)}: {Ffloat}, {nameof(Fint32)}: {Fint32}, {nameof(Fint64)}: {Fint64}, {nameof(Fuint32)}: {Fuint32}, {nameof(Fuint64)}: {Fuint64}, {nameof(Fsint32)}: {Fsint32}, {nameof(Fsint64)}: {Fsint64}, {nameof(Ffixed32)}: {Ffixed32}, {nameof(Ffixed64)}: {Ffixed64}, {nameof(Fsfixed32)}: {Fsfixed32}, {nameof(Fsfixed64)}: {Fsfixed64}, {nameof(Fbool)}: {Fbool}, {nameof(Fstring)}: {Fstring}, {nameof(Fbytes)}: {Fbytes}, {nameof(Fenum)}: {Fenum}, {nameof(Finner)}: {Finner}, {nameof(Frep)}: {Frep}, {nameof(FrepEnum)}: {FrepEnum}, {nameof(FrepString)}: {FrepString}, {nameof(FrepFixed32)}: {FrepFixed32}, {nameof(FrepUint32)}: {FrepUint32}";
        }
    }

    public partial class InnerMessage : IEquatable<InnerMessage>
    {
        public bool Equals(InnerMessage other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _test == other._test;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((InnerMessage) obj);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return _test;
        }
    }
}