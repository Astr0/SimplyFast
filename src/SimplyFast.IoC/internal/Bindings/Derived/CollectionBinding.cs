using System.Collections.Generic;
using SF.Collections.Concurrent;

namespace SF.IoC.Bindings.Derived
{
    internal class CollectionBinding<T> : IDerivedBinding
    {
        // Nice behavior
        //private static int _count;
        //public CollectionBinding()
        //{
        //    if (Interlocked.Increment(ref _count) != 1)
        //        throw new InvalidOperationException();
        //}

        private readonly ConcurrentGrowList<IBinding> _itemBindings = new ConcurrentGrowList<IBinding>();

        public void RegisterDerivedTypes(IKernel kernel)
        {
            DerivedBindingEx.TryAddDerivedType<IEnumerable<T>>(kernel, ArrayBinder);
            DerivedBindingEx.TryAddDerivedType<IReadOnlyList<T>>(kernel, ArrayBinder);
            DerivedBindingEx.TryAddDerivedType<IReadOnlyCollection<T>>(kernel, ArrayBinder);
            DerivedBindingEx.TryAddDerivedType(kernel, ArrayBinder);

            DerivedBindingEx.TryAddDerivedType<ICollection<T>>(kernel, ListBinder);
            DerivedBindingEx.TryAddDerivedType<IList<T>>(kernel, ListBinder);
            DerivedBindingEx.TryAddDerivedType(kernel, ListBinder);
        }

        public void Add(IBinding binding)
        {
            _itemBindings.Add(binding);
        }

        #region Binders

        private T[] ArrayBinder(IGetKernel kernel)
        {
            var snapshot = _itemBindings.GetSnapshot();
            var result = new T[snapshot.Count];
            for (var i = 0; i < result.Length; i++)
                result[i] = (T) snapshot[i].Get(kernel);
            return result;
        }

        private List<T> ListBinder(IGetKernel kernel)
        {
            var snapshot = _itemBindings.GetSnapshot();
            var result = new List<T>(snapshot.Count);
            // ReSharper disable once LoopCanBeConvertedToQuery
            // ReSharper disable once ForCanBeConvertedToForeach
            foreach (var binding in snapshot)
                result.Add((T) binding.Get(kernel));
            return result;
        }

        #endregion
    }
}