using System.Diagnostics.CodeAnalysis;

namespace SimplyFast.IoC
{
    public abstract class IocModule : IIocModule
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")] 
        protected IKernel Kernel { get; private set; }

        public void Load(IKernel kernel)
        {
            Kernel = kernel;
            Load();
        }

        protected BindingBuilder<T> Bind<T>()
        {
            return Kernel.Bind<T>();
        }

        protected abstract void Load();
    }
}