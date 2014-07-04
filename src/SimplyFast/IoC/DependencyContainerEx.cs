using SF.Logging;

namespace SF.IoC
{
    public  static class DependencyContainerEx
    {
        public static void Load(this IDependencyContainer container, IModule[] modules)
        {
            foreach (var module in modules)
            {
                module.Load(container);
            }

            var loadLogger = new ModuleLoadLogger();
            foreach (var module in modules)
            {
                module.AfterLoad(loadLogger);
            }

            var realLogger = ServiceLocator.Get<ILogger>();
            if (realLogger != null)
                loadLogger.ReplayTo(realLogger);
            loadLogger.ThrowIfFatal();
        }
    }
}