namespace Blink.Common.TinyClone
{
    public static class Clone
    {
        private static readonly CloneTypeCache _cloneTypeCache = new CloneTypeCache();

        public static T Deep<T>(T obj)
        {
            return new CloneContext(_cloneTypeCache).Clone(obj);
        }
    }
}
