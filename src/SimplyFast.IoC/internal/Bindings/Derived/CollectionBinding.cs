using System.Collections.Generic;

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

        #region Binders

        private static T[] ArrayBinder(IGetKernel kernel)
        {
            var bindings = kernel.GetAllBindings(typeof(T));
            var result = new T[bindings.Count];
            for (var i = 0; i < result.Length; i++)
                result[i] = (T) bindings[i].Get(kernel);
            return result;
        }

        private static List<T> ListBinder(IGetKernel kernel)
        {
            var bindings = kernel.GetAllBindings(typeof(T));
            var result = new List<T>(bindings.Count);
            // ReSharper disable once LoopCanBeConvertedToQuery
            // ReSharper disable once ForCanBeConvertedToForeach
            foreach (var binding in bindings)
                result.Add((T) binding.Get(kernel));
            return result;
        }

        #endregion
    }
}