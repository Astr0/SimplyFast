using System.Reflection;
using SimplyFast.Reflection;

namespace SimplyFast.IoC.Internal.Reflection
{
    internal class FastMethod
    {
        public readonly MethodInfo MethodInfo;
        public readonly ParameterInfo[] Parameters;
        private volatile MethodInvoker _invoker;

        public FastMethod(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            Parameters = methodInfo.GetParameters();
        }

        public void Invoke(object instance, object[] args)
        {
            if (_invoker == null)
                _invoker = MethodInfo.Invoker();
            _invoker(instance, args);
        }
    }
}