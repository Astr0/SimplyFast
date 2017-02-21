namespace SF.IoC.Injection
{
    internal class NullInjector : IInjector
    {
        public static readonly NullInjector Instance = new NullInjector();

        private NullInjector()
        {
        }

        public void Inject(IArgKernel kernel, object instance)
        {
        }
    }
}