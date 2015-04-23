using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SF.Collections
{
    public class WeakCollection<T> : ICollection<T>, IReadOnlyCollection<T>
        where T: class
    {
        private readonly List<WeakReference> _list;

        public WeakCollection(int capacity)
        {
            _list = new List<WeakReference>(capacity);
        }

        public WeakCollection()
        {
            _list = new List<WeakReference>();
        }

        public WeakCollection(IEnumerable<T> items)
        {
            _list = new List<WeakReference>(items.Select(x => new WeakReference(x)));
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        private IEnumerable<T> Items
        {
            get
            {
                var cleanup = false;
                foreach (var weakReference in _list)
                {
                    var t = weakReference.Target;
                    if (t != null)
                        yield return (T) t;
                    else
                        cleanup = true;
                }
                if (cleanup)
                    Cleanup();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            Cleanup();
            _list.Add(new WeakReference(item));
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(T item)
        {
            var @default = EqualityComparer<T>.Default;
            return _list.Any(x => @default.Equals((T)x.Target, item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var found = false;
            var @default = EqualityComparer<T>.Default;
            _list.RemoveAll(x =>
            {
                var t = x.Target;
                if (t == null)
                    return true;
                if (found || !@default.Equals((T)t, item))
                    return false;
                 return found = true;
            });
            return found;
        }

        public int CapCount { get { return _list.Count; } }

        public int Count
        {
            get
            {
                Cleanup();
                return _list.Count;
            }
        }

        private void Cleanup()
        {
            _list.RemoveAll(x => !x.IsAlive);
        }

        public bool IsReadOnly { get { return false; } }
    }
}