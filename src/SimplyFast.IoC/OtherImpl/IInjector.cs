namespace SimplyFast.IoC
{
    public interface IInjector
    {
        void Inject(IArgKernel kernel, object instance);
    }
}