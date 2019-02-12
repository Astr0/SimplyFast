using System;
using System.Diagnostics.CodeAnalysis;
using SimplyFast.IoC.Internal.Bindings;

namespace SimplyFast.IoC
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class BindingEx
    {
        public static IBinding Default(Type type)
        {
            return new DefaultBinding(type);
        }
    }
}