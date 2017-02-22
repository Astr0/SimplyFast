using System;

namespace SF.IoC.Bindings.Args
{
    internal partial class ArgsBinding 
    {
        private partial class Kernel
        {
            private struct ArgKey : IEquatable<ArgKey>
            {
                public bool Equals(ArgKey other)
                {
                    return Type == other.Type && string.Equals(Name, other.Name);
                }

                public override bool Equals(object obj)
                {
                    if (ReferenceEquals(null, obj)) return false;
                    return obj is ArgKey && Equals((ArgKey) obj);
                }

                public override int GetHashCode()
                {
                    unchecked
                    {
                        return (Type.GetHashCode() * 397) ^ (Name?.GetHashCode() ?? 0);
                    }
                }

                public static bool operator ==(ArgKey left, ArgKey right)
                {
                    return left.Equals(right);
                }

                public static bool operator !=(ArgKey left, ArgKey right)
                {
                    return !left.Equals(right);
                }

                public readonly Type Type;
                public readonly string Name;

                public ArgKey(Type type, string name)
                {
                    Type = type;
                    Name = name;
                }
            }
        }
    }
}