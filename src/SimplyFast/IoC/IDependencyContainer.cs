using System;
using System.Collections.Generic;

namespace SF.IoC
{
    /// <summary>
    /// Interface for configuring dependency container
    /// </summary>
    public interface IDependencyContainer
    {
        void Register<TService, TImplementation>(IReadOnlyDictionary<string, object> constructorParameters = null) where TImplementation : TService;
        void RegisterPerRequest<TService, TImplementation>(IReadOnlyDictionary<string, object> constructorParameters = null) where TImplementation : TService;
        void RegisterGeneric(Type service, Type implementation);
        void RegisterGenericPerRequest(Type service, Type implementation);
        void RegisterSingleton<TService>(TService instance);
        void RegisterSingleton<TService, TImplementation>() where TImplementation : TService;
        void UnRegister<TService>();
        void UnRegister(Type service);

        /// <summary>
        /// Method for configuring the container.
        /// </summary>
        void Configure();
    }
}