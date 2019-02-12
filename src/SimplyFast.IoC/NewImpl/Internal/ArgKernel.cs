using System;

namespace SimplyFast.IoC.Internal
{
    internal class ArgKernel: IGetKernel
    {
        private readonly IGetKernel _kernel;
        private readonly BindArg[] _args;

        public ArgKernel(IGetKernel kernel, BindArg[] args)
        {
            _kernel = kernel;
            _args = args;
            Root = _kernel.Root;
        }

        public IRootKernel Root { get; }

        public Binding GetDefaultBinding(Type type)
        {
            // TODO: Root create
            return GetOverrideBinding(type, null) ?? Root.CreateDefaultBinding(type, this);
        }

        public Binding GetOverrideBinding(Type type, string name)
        {
            // TODO: Root create
            foreach (var arg in _args)
            {
                if (arg.Match(type, name))
                    return c => arg.Value;
            }

            return _kernel.GetOverrideBinding(type, name);
        }

        public Injector GetInjector(Type type)
        {
            return Root.CreateDefaultInjector(type, this);
        }
    }
}