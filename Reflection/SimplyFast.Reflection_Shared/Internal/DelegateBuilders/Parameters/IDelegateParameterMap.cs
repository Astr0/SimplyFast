using System.Reflection.Emit;

namespace SF.Reflection.DelegateBuilders
{
    internal interface IDelegateParameterMap
    {
        void EmitPrepare(ILGenerator generator);
        void EmitLoad(ILGenerator generator);
        void EmitFinish(ILGenerator generator);
    }
}