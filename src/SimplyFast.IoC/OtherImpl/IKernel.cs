using System;
using System.Diagnostics.CodeAnalysis;

namespace SimplyFast.IoC
{
    public interface IGetKernel
    {
        IBinding GetBinding(Type type, params BindArg[] args);
        IBinding GetBinding(Type type);
        IInjector GetInjector(Type type);
        void Inject(object instance);
        object Get(Type type);
        object Get(Type type, params BindArg[] args);
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Global")]
    public interface IArgKernel : IGetKernel
    {
        IBinding GetArgBinding(Type type, string name);
        object Arg(Type type, string name);
        bool CanBind(Type type, string argName);
    }

    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
    public interface IKernel : IArgKernel
    {
        void Bind(Type type, IBinding binding);
        bool TryBind(Type type, IBinding binding);
    }
}