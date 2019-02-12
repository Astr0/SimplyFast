using System;
using SimplyFast.IoC.Internal.Bindings.Args;

namespace SimplyFast.IoC.Internal
{
    internal abstract class KernelBase : IArgKernel
    {
        public abstract IBinding GetBinding(Type type, params BindArg[] args);

        public abstract IBinding GetBinding(Type type);

        public abstract IBinding GetArgBinding(Type type, string name);

        public abstract IInjector GetInjector(Type type);

        public virtual void Inject(object instance)
        {
            if (instance == null)
                return;
            var injector = GetInjector(instance.GetType());
            if (injector != null)
                injector.Inject(this, instance);
        }

        public virtual object Get(Type type)
        {
            var binding = GetBinding(type);
            if (binding == null)
                throw new InvalidOperationException("Can't create: " + type + " no binding found");
            return binding.Get(this);
        }

        public virtual object Get(Type type, params BindArg[] args)
        {
            var binding = GetBinding(type, args);
            if (binding == null)
                throw new InvalidOperationException("Can't create: " + new TypeWithArgs(type, args) +
                                                    " no binding found");
            return binding.Get(this);
        }

        public virtual object Arg(Type type, string name)
        {
            var binding = GetArgBinding(type, name);
            if (binding == null)
                throw new InvalidOperationException("Can't create: " + type + " for arg " + name + " no binding found");
            return binding.Get(this);
        }

        public virtual bool CanBind(Type type, string argName)
        {
            var binding = GetArgBinding(type, argName);
            return binding != null;
        }
    }
}