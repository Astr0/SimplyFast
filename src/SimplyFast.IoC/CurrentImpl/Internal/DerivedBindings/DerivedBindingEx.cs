using System;
using SimplyFast.IoC.Internal.Bindings;
using SimplyFast.Reflection;

namespace SimplyFast.IoC.Internal.DerivedBindings
{
    internal static class DerivedBindingEx
    {
        /// <summary>
        /// Create derived binding intance by convention = One generic argument and parametless constructor
        /// </summary>
        public static IDerivedBinding Create(Type derivedBindingType, Type bindType)
        {
            var type = derivedBindingType.MakeGeneric(bindType);
            return type.CreateInstance<IDerivedBinding>();
        }

        private static void TryAddDerivedType<TP>(IKernel kernel, IBinding<TP> binder)
        {
            kernel.TryBind(binder);
        }

        public static void TryAddDerivedType<TP>(IKernel kernel, Func<IGetKernel, TP> binder)
        {
            TryAddDerivedType(kernel, new MethodBinding<TP>(binder));
        }
    }
}