namespace SF.IoC.Bindings.Derived
{
    internal class DerivedBindings<T> : IDerivedBinding
    {
        private readonly IDerivedBinding[] _bindings =
        {
            new CollectionBinding<T>(),
            new FuncFactoryBinding<T>()
        };


        public void RegisterDerivedTypes(IKernel kernel)
        {
            foreach (var derivedBinding in _bindings)
                derivedBinding.RegisterDerivedTypes(kernel);
        }
    }
}