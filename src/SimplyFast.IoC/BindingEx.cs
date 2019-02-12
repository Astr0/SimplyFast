using System;
using System.Diagnostics.CodeAnalysis;

namespace SimplyFast.IoC
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class BindingEx
    {
        public static Binding Default(Type type)
        {
            return c => c.GetDefault(type);
        }
    }
}