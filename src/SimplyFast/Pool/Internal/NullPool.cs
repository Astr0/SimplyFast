using SimplyFast.Cache;

namespace SimplyFast.Pool.Internal
{
    public class NullPool<TGetter>: IPool<TGetter>
    {
        private readonly PooledFactory<TGetter> _factory;

        public NullPool(PooledFactory<TGetter> factory)
        {
            _factory = factory;
        }

        public TGetter Get => _factory(NoReturn);

        private static void NoReturn(TGetter getter)
        {
        }

        public CacheStat CacheStat => new CacheStat(0);
    }
}