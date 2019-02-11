using System;
using System.Diagnostics.CodeAnalysis;

namespace SimplyFast.IoC
{
    public struct BindArg : IEquatable<BindArg>
    {
        public bool Equals(BindArg other)
        {
            return Type == other.Type && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is BindArg && Equals((BindArg) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Type?.GetHashCode() ?? 0) * 397) ^ (Name?.GetHashCode() ?? 0);
            }
        }

        public static bool operator ==(BindArg left, BindArg right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BindArg left, BindArg right)
        {
            return !left.Equals(right);
        }


        public BindArg(Type type, string name, object value)
        {
            Type = type ?? value?.GetType();
            Name = name;
            Value = value;
        }


        public readonly Type Type;
        public readonly string Name;
        public readonly object Value;
        
        public override string ToString()
        {
            return $"{Name ?? "?"}:{(Type != null ? Type.FullName : "<any>")}={Value ?? "<null>"}";
        }

        internal bool Match(Type type, string name)
        {
            return 
                (type == null || type.IsAssignableFrom(Type)) && 
                (Name == null || string.Equals(Name, name));
        }

        public static BindArg Typed<T>(T value)
        {
            return new BindArg(typeof(T), null, value);
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public static BindArg Named(string name, object value)
        {
            return new BindArg(null, name, value);
        }
    }
}