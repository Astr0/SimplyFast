using System.Collections.Generic;

namespace SimplyFast.Pool.Internal
{
    internal class StackPool<TGetter> : IPool<TGetter>
    {
        private readonly PooledFactory<TGetter> _factory;
        private readonly Stack<TGetter> _storage;

        public StackPool(PooledFactory<TGetter> factory)
        {
            _factory = factory;
            _storage = new Stack<TGetter>();
        }

        public TGetter Get => _storage.Count != 0 ? _storage.Pop() : _factory(_storage.Push);
    }
}