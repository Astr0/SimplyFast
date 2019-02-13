namespace Blink.Common.TinyClone
{
    internal class CloneIgnore<T>: ICloneType<T>
    {
        public T Clone(CloneContext cloneContext, T obj) => default;
    }
}