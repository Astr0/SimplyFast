namespace SimplyFast.IoC.Internal.Bindings.Derived
{
    internal class FastDerivedBindings<T> : IDerivedBinding
    {
        private readonly IDerivedBinding[] _bindings =
        {
            new CollectionBinding<T>(),
            new FuncFactoryBinding<T>()
        };


        public void Add(IBinding binding)
        {
            foreach (var derivedBinding in _bindings)
                derivedBinding.Add(binding);
        }

        public void RegisterDerivedTypes(IKernel kernel)
        {
            foreach (var derivedBinding in _bindings)
                derivedBinding.RegisterDerivedTypes(kernel);
        }
    }
}