using SimplyFast.Cloning.Internal;

namespace SimplyFast.Cloning
{
    public static class Clone
    {
        private static readonly CloneObject _defaultClone = new DefaultCloneFactory().Clone;

        public static T Custom<T>(T obj, CloneObject cloneObject)
        {
            return (T) new CloneContext(cloneObject).Clone(obj);
        }

        public static T Deep<T>(T obj)
        {
            return Custom(obj, _defaultClone);
        }
    }
}
