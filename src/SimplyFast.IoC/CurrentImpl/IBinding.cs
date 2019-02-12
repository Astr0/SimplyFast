using System.Diagnostics.CodeAnalysis;

namespace SimplyFast.IoC
{
    public interface IBinding
    {
        object Get(IGetKernel kernel);
    }

    [SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
    [SuppressMessage("ReSharper", "UnusedTypeParameter")]
    public interface IBinding<T> : IBinding
    {
    }
}