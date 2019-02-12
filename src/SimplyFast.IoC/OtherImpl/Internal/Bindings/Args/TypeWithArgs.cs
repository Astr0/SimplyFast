using System;
using System.Diagnostics.Contracts;
using System.Linq;
using SimplyFast.Collections;
using SimplyFast.IoC.Internal.Reflection;

namespace SimplyFast.IoC.Internal.Bindings.Args
{
    internal struct TypeWithArgs : IEquatable<TypeWithArgs>
    {
        public bool Equals(TypeWithArgs other)
        {
            return Type == other.Type && _args.Length == other._args.Length && _args.SequenceEqual(other._args);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TypeWithArgs && Equals((TypeWithArgs) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Type.GetHashCode() * 397) ^
                       _args.Aggregate(0, (current, arg) => (current * 397) ^ arg.GetHashCode());
            }
        }

        public readonly Type Type;
        private readonly BindArg[] _args;

        public TypeWithArgs(Type type, BindArg[] args)
        {
            Type = type;
            if (args == null)
                args = new BindArg[0];
            if (args.Length > 0)
            {
                _args = new BindArg[args.Length];
                Array.Copy(args, _args, args.Length);
                Array.Sort(_args, (a, b) =>
                {
                    var byName = string.Compare(a.Name, b.Name, StringComparison.Ordinal);
                    if (byName == 0)
                        return string.Compare(a.Type != null ? a.Type.AssemblyQualifiedName : null,
                            b.Type != null ? b.Type.AssemblyQualifiedName : null,
                            StringComparison.Ordinal);
                    return byName;
                });
            }
            else
            {
                _args = args;
            }
        }

        internal bool ConstructorHaveAllArgs(FastConstructor constructor)
        {
            return _args
                .Where(x => x.Name != null)
                .All(arg => constructor.Parameters.Any(x => x.Name == arg.Name));
        }

        public override string ToString()
        {
            return $"{Type.FullName}({string.Join(",", _args)})";
        }

        public bool TryGetArg(Type type, string name, out object value)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < _args.Length; i++)
            {
                if (!_args[i].Match(type, name))
                    continue;
                value = _args[i].Value;
                return true;
            }
            value = null;
            return false;
        }

        public bool Match(Type type, string name)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < _args.Length; i++)
                if (_args[i].Match(type, name))
                    return true;
            return false;
        }

        [Pure]
        public BindArg[] Combine(BindArg[] args)
        {
            return ArrayEx.Concat(args, _args);
        }
    }
}