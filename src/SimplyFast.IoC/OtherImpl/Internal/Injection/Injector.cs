using SimplyFast.IoC.Internal.Reflection;

namespace SimplyFast.IoC.Internal.Injection
{
    internal class Injector : IInjector
    {
        private readonly FastMethod _method;

        public Injector(FastMethod method)
        {
            _method = method;
        }

        public void Inject(IArgKernel kernel, object instance)
        {
            _method.Invoke(instance, kernel);
        }
    }
}