using SimplyFast.Reflection.Emit;

namespace SimplyFast.Reflection.Internal.DelegateBuilders
{
    internal static class DelegateBuilder
    {
        public static readonly IDelegateBuilder Current = EmitEx.Supported
            ? (IDelegateBuilder) new EmitDelegateBuilder()
            : new ExpressionDelegateBuilder();

        //public static readonly IDelegateBuilder Current = new ExpressionDelegateBuilder();
    }
}