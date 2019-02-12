using System;

namespace SimplyFast.IoC.Internal.Bindings.Args
{
    internal class ArgBindingCollection
    {
        private readonly IKernel _kernel;
        //private readonly ConcurrentDictionary<TypeWithArgs, IBinding> _argsBindings = new ConcurrentDictionary<TypeWithArgs, IBinding>();

        public ArgBindingCollection(IKernel kernel)
        {
            _kernel = kernel;
        }

        private IBinding CreateArgsBinding(TypeWithArgs typeWithArgs)
        {
            return ArgsBinding.Create(typeWithArgs);
        }

        public IBinding GetBinding(Type type, params BindArg[] args)
        {
            if (args == null || args.Length == 0)
                return _kernel.GetBinding(type);
            var typeWithArgs = new TypeWithArgs(type, args);
            return CreateArgsBinding(typeWithArgs);
            //return _argsBindings.GetOrAdd(typeWithArgs, CreateArgsBinding);
        }
    }
}