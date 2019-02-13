namespace Blink.Common.TinyClone
{
    internal class CloneCopy<T>: ICloneType<T>
    {
        public T Clone(CloneContext cloneContext, T obj) => obj;
    }
}