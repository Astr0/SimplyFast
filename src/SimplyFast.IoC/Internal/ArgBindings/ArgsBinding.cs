namespace SimplyFast.IoC.Internal.ArgBindings
{
    internal partial class ArgsBinding : IBinding
    {
        private readonly TypeWithArgs _args;

        private ArgsBinding(TypeWithArgs args)
        {
            _args = args;
        }

        public object Get(IGetKernel kernel)
        {
            var argKernel = new Kernel(kernel, _args);
            return argKernel.Get(_args.Type);
        }

        public static IBinding Create(TypeWithArgs typeWithArgs)
        {
            return new ArgsBinding(typeWithArgs);
        }
    }
}