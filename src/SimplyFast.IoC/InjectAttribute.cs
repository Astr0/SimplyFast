using System;
using System.Diagnostics.CodeAnalysis;

namespace SimplyFast.IoC
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [AttributeUsage(AttributeTargets.Method)]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class InjectAttribute : Attribute
    {
    }
}