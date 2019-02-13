namespace SimplyFast.Cloning
{
    public interface ICloneObject
    {
        object Clone(ICloneContext context, object src);
    }
}