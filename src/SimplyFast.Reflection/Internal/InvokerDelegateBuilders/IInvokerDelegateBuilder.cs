using System.Reflection;

namespace SimplyFast.Reflection.Internal
{
    internal interface IInvokerDelegateBuilder
    {
        MethodInvoker BuildMethodInvoker(MethodInfo methodInfo);
        ConstructorInvoker BuildConstructorInvoker(ConstructorInfo constructorInfo);
    }
}