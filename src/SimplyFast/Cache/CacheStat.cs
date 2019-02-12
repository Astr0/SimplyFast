namespace SimplyFast.Cache
{
    public struct CacheStat
    {
        public readonly int Count;
        public readonly int Capacity;

        public CacheStat(int count, int capacity)
        {
            Count = count;
            Capacity = capacity;
        }

        public CacheStat(int count) 
        {
            Count = count;
            Capacity = count;
        }
    }

}