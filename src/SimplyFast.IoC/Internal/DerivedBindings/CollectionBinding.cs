using System;
using System.Collections.Generic;
using SimplyFast.IoC.Internal.Bindings;

namespace SimplyFast.IoC.Internal.DerivedBindings
{
    internal static class CollectionBinding
    {
        public static void Register(IDerivedKernel kernel)
        {
            var array = new ArrayBinding();
            kernel.BindDerived(typeof(IEnumerable<>), array);
            kernel.BindDerived(typeof(IReadOnlyList<>), array);
            kernel.BindDerived(typeof(IReadOnlyCollection<>), array);
            kernel.BindDerived(typeof(IEnumerable<>), array);
            kernel.BindDerived(typeof(Array), array);

            var list = new ListBinding();
            kernel.BindDerived(typeof(ICollection<>), list);
            kernel.BindDerived(typeof(IList<>), list);
            kernel.BindDerived(typeof(List<>), list);
        }

        #region Binders

        private class ArrayBinding : IGenericDerivedBinding
        {
            public IBinding TryBind<TInner>()
            {
                return new MethodBinding<TInner[]>(GetArray<TInner>);
            }

            private static T[] GetArray<T>(IGetKernel kernel)
            {
                var bindings = kernel.GetAllBindings(typeof(T));
                var result = new T[bindings.Count];
                for (var i = 0; i < result.Length; i++)
                    result[i] = (T)bindings[i].Get(kernel);
                return result;
            }
        }

        private class ListBinding : IGenericDerivedBinding
        {
            public IBinding TryBind<TInner>()
            {
                return new MethodBinding<List<TInner>>(GetList<TInner>);
            }

            private static List<T> GetList<T>(IGetKernel kernel)
            {
                var bindings = kernel.GetAllBindings(typeof(T));
                var result = new List<T>(bindings.Count);
                // ReSharper disable once LoopCanBeConvertedToQuery
                // ReSharper disable once ForCanBeConvertedToForeach
                foreach (var binding in bindings)
                    result.Add((T)binding.Get(kernel));
                return result;
            }
        }
        
        #endregion
    }
}