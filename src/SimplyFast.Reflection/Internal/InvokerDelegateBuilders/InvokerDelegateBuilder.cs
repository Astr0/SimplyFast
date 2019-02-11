using SimplyFast.Reflection.Emit;

namespace SimplyFast.Reflection.Internal
{
    internal static class InvokerDelegateBuilder
    {
        // TODO: Test both somehow
        public static readonly IInvokerDelegateBuilder Current = EmitEx.Supported
            ? (IInvokerDelegateBuilder) new EmitInvokerDelegateBuilder()
            : new ExpressionsInvokerDelegateBuilder();
    }
}