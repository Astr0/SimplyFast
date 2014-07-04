using SF.Logging;

namespace SF.IoC
{
    /// <summary>
    /// Interface to register module provided services
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Called when module is loaded
        /// </summary>
        void Load(IDependencyContainer container);
        /// <summary>
        /// Called when module is unloaded
        /// </summary>
        void Unload(IDependencyContainer container);
        /// <summary>
        /// Called after all modules are loaded. Fatal should cause an exception
        /// </summary>
        void AfterLoad(ILogger logger);
    }
}