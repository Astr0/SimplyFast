using SF.IoC.Reflection;

namespace SF.IoC.Injection
{
    internal class Injector : IInjector
    {
        private readonly FastMethod _method;

        public Injector(FastMethod method)
        {
            _method = method;
        }

        public void Inject(IGetKernel kernel, object instance)
        {
            _method.Invoke(instance, kernel);
        }
    }
}