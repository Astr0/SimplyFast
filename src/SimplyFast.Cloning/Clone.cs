using SimplyFast.Cloning.Internal;

namespace SimplyFast.Cloning
{
    public static class Clone
    {
        private static readonly DefaultCloneFactory _defaultCloneFactory = new DefaultCloneFactory();

        public static T Custom<T>(T obj, ICloneObject cloneObject)
        {
            return (T) new CloneContext(cloneObject).Clone(obj);
        }

        public static T Deep<T>(T obj)
        {
            return Custom(obj, _defaultCloneFactory);
        }
    }
}
