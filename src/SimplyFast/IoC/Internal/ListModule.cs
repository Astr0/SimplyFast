using System;
using SF.Logging;

namespace SF.IoC
{
    internal class ListModule: IModule
    {
        private readonly IModule[] _modules;

        internal ListModule(IModule[] modules)
        {
            _modules = modules;
        }

        public void Load(IDependencyContainer container)
        {
            Array.ForEach(_modules, x => x.Load(container));
        }

        public void Unload(IDependencyContainer container)
        {
            Array.ForEach(_modules, x => x.Unload(container));
        }

        public void AfterLoad(ILogger logger)
        {
            Array.ForEach(_modules, x => x.AfterLoad(logger));
        }
    }
}