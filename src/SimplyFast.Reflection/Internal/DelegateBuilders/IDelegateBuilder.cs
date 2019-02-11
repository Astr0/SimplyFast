using System;
using System.Reflection;

namespace SimplyFast.Reflection.Internal.DelegateBuilders
{
    internal interface IDelegateBuilder
    {
        Delegate Constructor(ConstructorInfo constructor, Type delegateType);
        Delegate Method(MethodInfo method, Type delegateType);
        Delegate FieldGet(FieldInfo field, Type delegateType);
        Delegate FieldSet(FieldInfo field, Type delegateType);
    }
}