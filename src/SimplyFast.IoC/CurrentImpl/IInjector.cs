namespace SimplyFast.IoC
{
    public interface IInjector
    {
        void Inject(IGetKernel kernel, object instance);
    }
}