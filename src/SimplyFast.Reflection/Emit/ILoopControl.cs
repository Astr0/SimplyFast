namespace SF.Reflection.Emit
{
    public interface ILoopControl
    {
        void EmitBreak();
        void EmitContinue();
    }
}