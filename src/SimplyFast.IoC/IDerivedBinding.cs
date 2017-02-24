using System;

namespace SimplyFast.IoC
{
    public interface IDerivedBinding
    {
        IBinding TryBind(Type type);
    }

    public interface IGenericDerivedBinding
    {
        IBinding TryBind<TInner>();
    }
}