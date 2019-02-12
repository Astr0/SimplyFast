using System;
using System.Collections.Generic;

namespace SimplyFast.IoC
{
    public delegate object Binding(IGetKernel kernel);
    public delegate void Injector(IGetKernel kernel, object instance);

    public delegate Binding DefaultBuilder(Type type, IGetKernel kernel);

    public interface IGenericDefaultBuilder
    {
        Binding TryBind<TInner>(IGetKernel kernel);
    }

    public interface IGetKernel
    {
        IRootKernel Root { get; }
        /// <summary>
        /// Returns default bindings for type. Should return also arg and anything kernel can make up
        /// </summary>
        Binding GetDefaultBinding(Type type);

        /// <summary>
        /// Returns binding override for argument
        /// </summary>
        Binding GetOverrideBinding(Type type, string name);

        /// <summary>
        /// Returns injector for type
        /// </summary>
        Injector GetInjector(Type type);
    }

    public interface IRootKernel : IGetKernel
    {
        /// <summary>
        /// Returns relevant user binding for type
        /// </summary>
        Binding GetUserBinding(Type type);

        /// <summary>
        /// Returns all user binding for type
        /// </summary>
        IReadOnlyCollection<Binding> GetUserBindings(Type type);

        Binding CreateDefaultBinding(Type type, IGetKernel kernel);
        Injector CreateDefaultInjector(Type type, IGetKernel kernel);
    }

    public interface IKernel: IRootKernel
    {
        /// <summary>
        /// Adds new binding for type
        /// </summary>
        void Bind(Type type, Binding binding);

        void BindDefault(DefaultBuilder builder);
        void BindDefault(Type genericTypeDefinition, IGenericDefaultBuilder builder);
    }
}