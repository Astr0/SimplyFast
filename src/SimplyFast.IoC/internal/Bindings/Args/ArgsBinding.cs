namespace SF.IoC.Bindings.Args
{
    internal partial class ArgsBinding : IBinding
    {
        private readonly TypeWithArgs _args;

        private ArgsBinding(TypeWithArgs args)
        {
            _args = args;
        }

        public object Get(IArgKernel kernel)
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