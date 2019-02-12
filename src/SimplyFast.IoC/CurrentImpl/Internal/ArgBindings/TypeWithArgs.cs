using System;
using System.Linq;
using SimplyFast.Collections;
using SimplyFast.IoC.Internal.Reflection;

namespace SimplyFast.IoC.Internal.ArgBindings
{
    internal struct TypeWithArgs
    {
        private readonly BindArg[] _args;

        public readonly Type Type;

        public TypeWithArgs(Type type, BindArg[] args)
        {
            Type = type;
            _args = args ?? TypeHelper<BindArg>.EmptyArray;
        }

        public bool ConstructorHaveAllArgs(FastConstructor constructor)
        {
            return _args
                .Where(x => x.Name != null)
                .All(arg => constructor.Parameters.Any(p => p.Name == arg.Name));
        }

        public bool HasNamedArg(Type type, string name)
        {
            if (name == null)
                return false;
            for (var i = 0; i < _args.Length; i++)
            {
                if (_args[i].Name != name)
                    continue;
                if (_args[i].Match(type, name))
                    return true;
            }
            return false;
        }

        public bool TryGetArg(Type type, string name, out object value)
        {
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
            for (var i = 0; i < _args.Length; i++)
                if (_args[i].Match(type, name))
                    return true;
            return false;
        }

        public override string ToString()
        {
            return ToString(Type, _args);
        }

        public static string ToString(Type type, BindArg[] args)
        {
            return $"{type.FullName}({string.Join(",", args)})";
        }
    }
}