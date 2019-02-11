using System;
using System.Reflection;
using System.Reflection.Emit;
using SimplyFast.Collections;
using SimplyFast.Reflection.Emit;
using SimplyFast.Reflection.Internal.DelegateBuilders.Maps;

namespace SimplyFast.Reflection.Internal.DelegateBuilders
{
    internal class EmitDelegateBuilder: IDelegateBuilder
    {
        public Delegate Constructor(ConstructorInfo constructor, Type delegateType)
        {
            return BuildDelegate(DelegateMap.Constructor(delegateType, constructor),
                il => il.Emit(OpCodes.Newobj, constructor));
        }

        public Delegate Method(MethodInfo method, Type delegateType)
        {
            var map = DelegateMap.Method(delegateType, method);
            var declaring = method.DeclaringType;
            // avoid stupid issue in .Net framework... with no CreateDelegate for Generic Interface methods
            if (declaring == null || !(declaring.IsInterface && method.IsGenericMethod))
            {
                // if exact match
                if (map.RetValMap.Matches && Array.TrueForAll(map.ParametersMap, x => x.Matches))
                    return Delegate.CreateDelegate(delegateType, method);
            }
            // TODO: Simple Delegate.CreateDelegate
            return BuildDelegate(map, 
                il => il.EmitMethodCall(method));
        }

        public Delegate FieldGet(FieldInfo field, Type delegateType)
        {
            return BuildDelegate(DelegateMap.FieldGet(delegateType, field),
                il =>
                {
                    if (field.IsLiteral)
                    {
                        var value = field.GetValue(null);
                        il.EmitLdConst(value);
                    }
                    else
                    {
                        il.EmitFieldGet(field);
                    }
                });
        }

        public Delegate FieldSet(FieldInfo field, Type delegateType)
        {
            return BuildDelegate(DelegateMap.FieldSet(delegateType, field),
                il => il.EmitFieldSet(field));
        }

        private static Delegate BuildDelegate(DelegateMap map, Action<ILGenerator> emitInvoke)
        {
            var pm = map.ParametersMap;
            var paramTypes = pm.ConvertAll(x => x.Delegate.Type);
            var m = new DynamicMethod(string.Empty, map.RetValMap.Delegate, paramTypes,
                typeof(EmitDelegateBuilder), MemberInfoEx.PrivateAccess);
            var il = m.GetILGenerator();
            // Prepare parameters...
            var locals = pm.ConvertAll((p, i) => EmitPrepare(il, p, i));
            // Load parameters, stack should be empty here
            for (var i = 0; i < pm.Length; i++)
            {
                EmitLoad(il, pm[i], i, locals[i]);
            }
            // Emit invoke
            emitInvoke(il);
            // Emit finish, stack should contain return value here (if not void)
            for (var i = 0; i < pm.Length; i++)
            {
                EmitFinish(il, pm[i], i, locals[i]);
            }
            // Emit return
            EmitConvertReturn(il, map.RetValMap);
            il.Emit(OpCodes.Ret);
            return m.CreateDelegate(map.DelegateType);
        }

        private static LocalBuilder EmitPrepare(ILGenerator il, ArgMap map, int index)
        {
            // nothing for normal
            if (!map.Method.IsOut && !map.Method.IsByRef)
                return null;
            // out or ref
            var mt = map.Method.Type.RemoveByRef();
            var dt = map.Delegate.Type.RemoveByRef();
            // same types - no local
            if (mt == dt)
                return null;
            var localVariable = il.DeclareLocal(mt);

            // out - just local
            if (map.Method.IsOut)
                return localVariable;
            // ref - convert as store local
            il.EmitLdarg(index);

            if (map.Delegate.IsByRef)
                il.EmitLdind(dt);
            if (dt.IsValueType && !mt.IsValueType)
                il.EmitBox(dt);
            else if (!mt.IsAssignableFrom(dt))
                il.EmitUnBoxAnyOrCastClass(mt);
            il.EmitStloc(localVariable.LocalIndex);

            return localVariable;
        }

        private static void EmitLoad(ILGenerator il, ArgMap map, int index, LocalBuilder local)
        {
            // we have local (out, ref)
            if (local != null)
            {
                il.EmitLdloca(local.LocalIndex);
                return;
            }

            
            // ref or out
            if (map.Method.IsByRef || map.Method.IsOut)
            {
                // ref, but delegate is not ref
                if (map.Method.IsByRef && !map.Delegate.IsByRef)
                    il.EmitLdarga(index);
                 else 
                    il.EmitLdarg(index);
                return;
            }

            // normal
            var dt = map.Delegate.Type.RemoveByRef();
            il.EmitLdarg(index);
            if (map.Delegate.IsByRef)
            {
                il.EmitLdind(dt);
            }

            var mt = map.Method.Type;
            if (dt.IsValueType && !mt.IsValueType)
                il.EmitBox(dt);
            else if (!mt.IsAssignableFrom(dt))
                il.EmitUnBoxAnyOrCastClass(mt);
        }

        private static void EmitFinish(ILGenerator il, ArgMap map, int index, LocalBuilder local)
        {
            // Not out and (Not By Ref or ([byRef] and not (delegate ref or delegate out)
            if (!map.Method.IsOut && (!map.Method.IsByRef || !(map.Delegate.IsByRef || map.Delegate.IsOut)))
                return;

            // out/ref and typeSame - do nothing
            var mt = map.Method.Type.RemoveByRef();
            var dt = map.Delegate.Type.RemoveByRef();
            if (mt == dt)
                return;

            il.EmitLdarg(index);
            il.EmitLdloc(local.LocalIndex);
            if (mt.IsValueType && !dt.IsValueType)
                il.EmitBox(mt);
            il.EmitStind(dt);
        }


        private static void EmitConvertReturn(ILGenerator generator, RetValMap retValMap)
        {
            if (retValMap.Matches || retValMap.Delegate == typeof(void))
                return;
            if (retValMap.Method == typeof(void))
                generator.Emit(OpCodes.Ldnull);
            else if (retValMap.Method.IsValueType && !retValMap.Delegate.IsValueType)
                generator.EmitBox(retValMap.Method);
        }
    }
}