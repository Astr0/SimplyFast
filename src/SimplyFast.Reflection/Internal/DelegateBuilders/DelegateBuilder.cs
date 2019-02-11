namespace SimplyFast.Reflection.Internal.DelegateBuilders
{
    internal static class DelegateBuilder
    {
        public static readonly IDelegateBuilder Current = new ExpressionDelegateBuilder();
    }
}