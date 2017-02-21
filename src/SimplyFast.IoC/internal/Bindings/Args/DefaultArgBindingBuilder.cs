using System;

namespace SF.IoC.Bindings.Args
{
    internal static class DefaultArgBindingBuilder
    {
        public static IBinding GetBinding(IGetKernel kernel, Type type, params BindArg[] args)
        {
            if (args == null || args.Length == 0)
                return kernel.GetBinding(type);
            var typeWithArgs = new TypeWithArgs(type, args);
            return ArgsBinding.Create(typeWithArgs);
            //return _argsBindings.GetOrAdd(typeWithArgs, CreateArgsBinding);
        }
    }
}