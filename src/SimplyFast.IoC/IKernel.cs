using System;

namespace SF.IoC
{
    public interface IGetKernel
    {
        int Version { get; }
        IBinding GetBinding(Type type, params BindArg[] args);
        IBinding GetBinding(Type type);
        IBinding GetArgBinding(Type type, string name);

        IInjector GetInjector(Type type);
    }
    
    public interface IKernel : IGetKernel
    {
        void Bind(Type type, IBinding binding);
        bool TryBind(Type type, IBinding binding);
    }
}