using System;
using System.Reflection;
using SimplyFast.Collections;

namespace SimplyFast.Reflection.Internal.DelegateBuilders.Parameters
{
    public struct SimpleParameterInfo : IEquatable<SimpleParameterInfo>
    {
        public readonly Type Type;
        public readonly bool IsOut;

        public SimpleParameterInfo(Type type) : this()
        {
            Type = type;
        }

        public SimpleParameterInfo(ParameterInfo parameter) : this()
        {
            Type = parameter.ParameterType;
            IsOut = parameter.IsOut;
        }

        public static SimpleParameterInfo[] FromParameters(ParameterInfo[] parameters)
        {
            return parameters.ConvertAll(x => new SimpleParameterInfo(x));
        }

        public bool Equals(SimpleParameterInfo other)
        {
            return Type == other.Type && IsOut == other.IsOut;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is SimpleParameterInfo && Equals((SimpleParameterInfo) obj);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                return (Type.GetHashCode() * 397) ^ IsOut.GetHashCode();
            }
        }

        public static bool operator ==(SimpleParameterInfo left, SimpleParameterInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SimpleParameterInfo left, SimpleParameterInfo right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return IsOut ? "out " + Type : Type.ToString();
        }
    }
}