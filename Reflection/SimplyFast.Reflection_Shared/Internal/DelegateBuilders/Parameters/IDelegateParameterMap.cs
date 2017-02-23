#if EMIT
using System.Reflection.Emit;
#endif

namespace SF.Reflection.Internal.DelegateBuilders.Parameters
{
    internal interface IDelegateParameterMap
    {
#if EMIT
        void EmitPrepare(ILGenerator generator);
        void EmitLoad(ILGenerator generator);
        void EmitFinish(ILGenerator generator);
#endif
    }
}