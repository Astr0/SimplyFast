using System;
using SimplyFast.Reflection;

namespace SimplyFast.IoC.Internal.Bindings.Derived
{
    internal static class DerivedBindingEx
    {
        /// <summary>
        ///     Create derived binding instance by convention = One generic argument and default constructor
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

        public static void TryAddDerivedType<TP>(IKernel kernel, Func<IArgKernel, TP> binder)
        {
            TryAddDerivedType(kernel, new MethodBinding<TP>(binder));
        }
    }
}