using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace SimplyFast.Disposables
{
    public static partial class DisposableEx
    {
        public static IDisposable KeepAlive(this IDisposable disposable, object obj)
        {
            return new KeepAliveDisposable(disposable, obj);
        }

        public static IDisposable Null()
        {
            return NullDisposable.Instance;
        }

        public static IDisposable Concat(params IDisposable[] disposables)
        {
            return new Disposables(disposables);
        }

        public static IDisposable Action(Action disposeAction)
        {
            return new DisposableAction(disposeAction);
        }

        public static IDisposable AlwaysAction(Action action)
        {
            return new AlwaysDisposableAction(action);
        }

        public static IDisposable Remove<T>(ICollection<T> collection, T item)
        {
            return new CollectionRemove<T>(collection, item);
        }

        public static IDisposable UseContext(this IDisposable disposable, SynchronizationContext context)
        {
            return new ContextDisposable(disposable, context);
        }
        
        [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
        public static IDisposable DisposeOnFinalize(this IDisposable disposable)
        {
            return new FinalizeDisposable(disposable);
        }

        public static IAssignDisposable<T> Assign<T>(T item) where T : class, IDisposable
        {
            return new AssignDisposable<T>(item);
        }

        public static IAssignDisposable<T> Assign<T>() where T : class, IDisposable
        {
            return new AssignDisposable<T>();
        }

        public static void Dispose<T>(IEnumerable<T> collection)
        {
            foreach (var disposable in collection.OfType<IDisposable>())
                disposable.Dispose();
        }

        public static void Dispose<T>(ICollection<T> collection)
        {
            Dispose((IEnumerable<T>) collection);
            if (!(collection is T[]) && !collection.IsReadOnly)
                collection.Clear();
        }

        public static void AddAction(this ICollection<IDisposable> disposables, Action action)
        {
            disposables.Add(Action(action));
        }
    }
}