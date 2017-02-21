using System.Diagnostics.CodeAnalysis;

namespace SF.IoC
{
    public interface IBinding
    {
        object Get(IArgKernel kernel);
    }

    [SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
    [SuppressMessage("ReSharper", "UnusedTypeParameter")]
    public interface IBinding<T> : IBinding
    {
    }
}