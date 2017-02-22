using System;

namespace SF.IoC
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