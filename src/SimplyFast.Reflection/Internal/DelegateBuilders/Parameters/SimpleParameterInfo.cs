using System;
using System.Reflection;

namespace SF.Reflection.DelegateBuilders
{
    internal class SimpleParameterInfo: ParameterInfo
    {
        public SimpleParameterInfo(Type parameterType)
        {
            ClassImpl = parameterType;
        }
    }
}