using System.Collections.Generic;

namespace SimplyFast.Pool.Internal
{
    internal class StackLockPool<TGetter> : IPool<TGetter>
    {
        private readonly PooledFactory<TGetter> _factory;
        private readonly Stack<TGetter> _storage;

        public StackLockPool(PooledFactory<TGetter> factory)
        {
            _factory = factory;
            _storage = new Stack<TGetter>();
        }


        public TGetter Get
        {
            get
            {
                lock (_storage)
                {
                    return _storage.Count != 0 ? _storage.Pop() : _factory(Return);
                }
            }
        }

        private void Return(TGetter getter)
        {
            lock (_storage)
                _storage.Push(getter);
        }
    }
}