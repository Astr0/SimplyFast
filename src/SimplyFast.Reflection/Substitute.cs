using System;
using System.Collections.Generic;
using System.Linq;

namespace SF.Reflection
{
    public static class Substitute
    {
        public static readonly IList<Type> T = Array.AsReadOnly(new[]
        {
            typeof (T0),
            typeof (T1),
            typeof (T2),
            typeof (T3),
            typeof (T4),
            typeof (T5),
            typeof (T6),
            typeof (T7),
            typeof (T8),
            typeof (T9),
            typeof (T10),
            typeof (T11),
            typeof (T12),
            typeof (T13),
            typeof (T14),
            typeof (T15)
        });

        private static readonly Dictionary<Type, int> _genericToIndex = T
            .Select((x, i) => new KeyValuePair<Type, int>(x, i))
            .ToDictionary(x => x.Key, x => x.Value);

        public static Func<Type, Type> GetSubstitutionFunction(Type[] genericArgs)
        {
            if (genericArgs.Length > 15)
                throw new NotSupportedException("More than 15 arguments not supported.");
            return t =>
            {
                int index;
                if (_genericToIndex.TryGetValue(t, out index))
                {
                    if (index < genericArgs.Length)
                        return genericArgs[index];
                    throw new ArgumentException("Type out of range");
                }
                return null;
            };
        }
    }

    public sealed class T0
    {
    }

    public sealed class T1
    {
    }

    public sealed class T10
    {
    }

    public sealed class T11
    {
    }

    public sealed class T12
    {
    }

    public sealed class T13
    {
    }

    public sealed class T14
    {
    }

    public sealed class T15
    {
    }

    public sealed class T2
    {
    }

    public sealed class T3
    {
    }

    public sealed class T4
    {
    }

    public sealed class T5
    {
    }

    public sealed class T6
    {
    }

    public sealed class T7
    {
    }

    public sealed class T8
    {
    }

    public sealed class T9
    {
    }
}