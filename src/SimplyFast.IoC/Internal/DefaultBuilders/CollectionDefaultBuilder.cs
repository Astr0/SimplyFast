using System;
using System.Collections.Generic;

namespace SimplyFast.IoC.Internal.DefaultBuilders
{
    internal static class CollectionDefaultBuilder
    {
        public static void Register(IKernel kernel)
        {
            var array = new ArrayDefaultBuilder();
            kernel.BindDefault(typeof(IEnumerable<>), array);
            kernel.BindDefault(typeof(IReadOnlyList<>), array);
            kernel.BindDefault(typeof(IReadOnlyCollection<>), array);
            kernel.BindDefault(typeof(IEnumerable<>), array);
            kernel.BindDefault(typeof(Array), array);

            var list = new ListDefaultBuilder();
            kernel.BindDefault(typeof(ICollection<>), list);
            kernel.BindDefault(typeof(IList<>), list);
            kernel.BindDefault(typeof(List<>), list);
        }

        #region Binders

        private class ArrayDefaultBuilder : IGenericDefaultBuilder
        {
            public Binding TryBind<TInner>(IGetKernel kernel)
            {
                return c =>
                {
                    var bindings = c.Root.GetUserBindings(typeof(TInner));
                    var result = new TInner[bindings.Count];
                    var i = 0;
                    foreach (var binding in bindings)
                    {
                        result[i++] = (TInner)binding(c);
                    }

                    return result;
                };
            }
        }

        private class ListDefaultBuilder : IGenericDefaultBuilder
        {
            public Binding TryBind<TInner>(IGetKernel kernel)
            {
                return c =>
                {
                    var bindings = c.Root.GetUserBindings(typeof(TInner));
                    var result = new List<TInner>(bindings.Count);
                    // ReSharper disable once LoopCanBeConvertedToQuery
                    // ReSharper disable once ForCanBeConvertedToForeach
                    foreach (var binding in bindings)
                        result.Add((TInner)binding(c));
                    return result;
                };
            }
        }

        #endregion
    }
}