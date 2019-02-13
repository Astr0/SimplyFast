namespace Blink.Common.TinyClone
{
    internal interface ICloneType<T>
    {
        T Clone(CloneContext cloneContext, T obj);
    }
}