using System.Diagnostics.CodeAnalysis;

namespace SimplyFast.IoC
{
    public abstract class FastModule : IFastModule
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

        // This should be protected, but for ninject sake..
        [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
        public abstract void Load();
    }
}