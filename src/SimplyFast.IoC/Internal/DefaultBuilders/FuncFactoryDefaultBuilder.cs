using System;

namespace SimplyFast.IoC.Internal.DefaultBuilders
{
    internal class FuncFactoryDefaultBuilder : IGenericDefaultBuilder
    {
        private static readonly FuncFactoryDefaultBuilder _instance = new FuncFactoryDefaultBuilder();
        private FuncFactoryDefaultBuilder()
        {
        }

        public static void Register(IKernel kernel)
        {
            kernel.BindDefault(typeof(Func<>), _instance);
        }

        public Binding TryBind<TInner>(IGetKernel kernel)
        {
            return c => (Func<TInner>)c.Get<TInner>;
        }
    }
}