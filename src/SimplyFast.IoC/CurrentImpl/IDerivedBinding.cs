using System;
using System.Diagnostics.CodeAnalysis;

namespace SimplyFast.IoC
{
    public interface IDerivedBinding
    {
        IBinding TryBind(Type type);
    }

    public interface IGenericDerivedBinding
    {
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        IBinding TryBind<TInner>();
    }
}