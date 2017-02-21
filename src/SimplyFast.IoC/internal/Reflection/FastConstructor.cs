using System.Reflection;
using SF.Reflection;

namespace SF.IoC.Reflection
{
    internal class FastConstructor
    {
        public readonly ConstructorInfo ConstructorInfo;
        public readonly ParameterInfo[] Parameters;
        private volatile ConstructorInvoker _invoker;

        public FastConstructor(ConstructorInfo constructorInfo)
        {
            ConstructorInfo = constructorInfo;
            Parameters = constructorInfo.GetParameters();
        }

        public object Invoke(object[] args)
        {
            if (_invoker == null)
                _invoker = ConstructorInfo.Invoker();
            return _invoker(args);
        }
    }
}