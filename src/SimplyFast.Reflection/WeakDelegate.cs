#if EMIT
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using SimplyFast.Reflection.Emit;
using SimplyFast.Collections;

namespace SimplyFast.Reflection
{
    internal static class WeakDelegate
    {
        private static readonly object _lock = new object();
        private static volatile ModuleBuilder _module;

        public static ModuleBuilder Module
        {
            get
            {
                if (_module != null)
                    return _module;
                lock (_lock)
                {
                    if (_module != null)
                        return _module;
                    var module = AssemblyEx.DynamicAssembly.DefineDynamicModule("WeakDelegate");
                    _module = module;
                    return module;
                }
            }
        }
    }

    public abstract class WeakDelegate<T>: WeakCollection<T> 
        where T : class
    {
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        protected WeakDelegate()
        {
            Invoker = BuildInvoker();
        }

        protected abstract T BuildInvoker();

        public readonly T Invoker;

        public static WeakDelegate<T> Create()
        {
            return _constructor();
        }

#region Magic

        private static readonly Func<WeakDelegate<T>> _constructor = CompileImplType().Constructor().InvokerAs<Func<WeakDelegate<T>>>();

        private static string GetTypeName()
        {
            return "WeakDelegate" + typeof(T).IdentifierFriendlyName();
        }
        private static Type CompileImplType()
        {
            var invokeMethod = MethodInfoEx.GetInvokeMethod(typeof (T));
            var tb = WeakDelegate.Module.DefineType(GetTypeName()
                                , TypeAttributes.Public |
                                TypeAttributes.Class |
                                TypeAttributes.AutoClass |
                                TypeAttributes.AnsiClass |
                                TypeAttributes.BeforeFieldInit |
                                TypeAttributes.AutoLayout
                                , typeof(WeakDelegate<T>));

            tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            var invoke = BuildInvoke(tb, invokeMethod);

            var buildInvoker = tb.DefineMethod("BuildInvoker",
                MethodAttributes.Family | MethodAttributes.ReuseSlot | MethodAttributes.Virtual | 
                MethodAttributes.HideBySig,
                typeof (T), Type.EmptyTypes);

            var il = buildInvoker.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldftn, invoke);
            il.Emit(OpCodes.Newobj, typeof(T).Constructor(typeof(object), typeof(IntPtr)));
            il.Emit(OpCodes.Ret);
            return tb.CreateType();
        }

        private static MethodBuilder BuildInvoke(TypeBuilder tb, MethodInfo invokeMethod)
        {
            var args = invokeMethod.GetParameters().Select(x => x.ParameterType).ToArray();
            var returnType = invokeMethod.ReturnType;
            var invoke = tb.DefineMethod("Invoke", MethodAttributes.Private | MethodAttributes.HideBySig,
                returnType, args);

            var il = invoke.GetILGenerator();
            //var del = il.DeclareLocal(typeof(T));
            var hasReturn = returnType != typeof(void);
            var returnValue = hasReturn ? il.DeclareLocal(returnType) : null;
            
            // get enumerator
            il.Emit(OpCodes.Ldarg_0);
            il.EmitForEach(typeof(WeakCollection<T>), l =>
            {
                for (var i = 1; i <= args.Length; i++)
                {
                    il.EmitLdarg(i);
                }
                il.EmitCall(OpCodes.Callvirt, invokeMethod, null);
                if (hasReturn)
                    il.EmitStloc(returnValue);
            });

            if (hasReturn)
                il.EmitLdloc(returnValue);
            il.Emit(OpCodes.Ret);

            return invoke;
        }

#endregion
    }
}
#endif