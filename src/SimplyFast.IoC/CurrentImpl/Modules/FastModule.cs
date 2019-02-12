using System.Diagnostics.CodeAnalysis;

namespace SimplyFast.IoC.Modules
{
    public abstract class FastModule : IFastModule
    {
        private IKernel _kernel;

        protected IKernel Kernel => _kernel;

        public void Load(IKernel kernel)
        {
            _kernel = kernel;
            Load();
        }

        protected BindingBuilder<T> Bind<T>()
        {
            return _kernel.Bind<T>();
        }

        // This should be protected, but for ninject sake..
        [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
        public abstract void Load();
    }
}