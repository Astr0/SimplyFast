namespace SimplyFast.IoC.Internal.Bindings.Derived
{
    internal interface IDerivedBinding
    {
        void Add(IBinding binding);
        void RegisterDerivedTypes(IKernel kernel);
    }
}