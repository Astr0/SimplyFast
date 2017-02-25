﻿using SimplyFast.IoC.Internal.Reflection;

namespace SimplyFast.IoC.Internal.Bindings
{
    internal class ConstructorWithInjectBinding : IBinding
    {
        private readonly FastConstructor _constructor;
        private readonly IInjector _injector;

        public ConstructorWithInjectBinding(FastConstructor constructor, IInjector injector)
        {
            _constructor = constructor;
            _injector = injector;
        }

        public object Get(IGetKernel kernel)
        {
            //using (Log.Measure("Construct&Inject: " + _constructor))
            {
                var instance = _constructor.Invoke(kernel);
                _injector.Inject(kernel, instance);
                return instance;
            }
        }
    }
}