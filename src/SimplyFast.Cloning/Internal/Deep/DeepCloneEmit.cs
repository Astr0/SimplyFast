using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using SimplyFast.Reflection;
using SimplyFast.Reflection.Emit;

namespace SimplyFast.Cloning.Internal.Deep
{
    internal static class DeepCloneEmit
    {
        public static CloneObject Build(Type type)
        {
            

            var m = new DynamicMethod(string.Empty, 
                typeof(object), new[]
            {
                typeof(ICloneContext),
                typeof(object)
            },typeof(DeepCloneEmit), MemberInfoEx.PrivateAccess);

            var il = m.GetILGenerator();

            il.EmitClone(type);

            il.Emit(OpCodes.Ret);
            return (CloneObject)m.CreateDelegate(typeof(CloneObject));
        }

        private static readonly MethodInfo _createObj =
            typeof(FormatterServices).Method("GetUninitializedObject", typeof(Type));

        private static readonly MethodInfo _ctxClone =
            typeof(ICloneContext).Methods()[0];

        private static void EmitCreateObject(this ILGenerator il, Type type)
        {
            il.EmitLoadType(type);
            il.EmitMethodCall(_createObj);
        }

        private static void EmitClone(this ILGenerator il, Type type)
        {
            var fields = DeepCloneHelper
                .GetDeepCloneFields(type)
                .ToArray();

            if (fields.Length == 0)
            {
                // nothing to clone
                il.EmitCreateObject(type);
                return;
            }

            var src = il.DeclareLocal(type).LocalIndex;
            var dest = il.DeclareLocal(type).LocalIndex;

            // src = (type)p_1;
            il.EmitLdarg(1);
            il.EmitUnBoxAnyOrCastClass(type);
            il.EmitStloc(src);
            
            // dest = (type)CreateObject(type);
            il.EmitCreateObject(type);
            il.EmitUnBoxAnyOrCastClass(type);
            il.EmitStloc(dest);

            foreach (var field in fields)
            {
                Debug.Assert(field.CloneType != CloneType.Ignore);

                var fieldInfo = field.Field;

                if (type.IsValueType)
                    il.EmitLdloca(dest);
                else
                    il.EmitLdloc(dest);

                if (field.CloneType == CloneType.Deep)
                {
                    // ctx
                    il.EmitLdarg(0);

                    // (object)src.Field
                    il.EmitLdloc(src);
                    il.EmitFieldGet(fieldInfo);
                    il.EmitBox(fieldInfo.FieldType);

                    // ctx.Clone()
                    il.EmitMethodCall(_ctxClone);

                    // (object)
                    il.EmitUnBoxAnyOrCastClass(fieldInfo.FieldType);
                }
                else
                {
                    il.EmitLdloc(src);
                    il.EmitFieldGet(fieldInfo);
                }

                // [dest].Field=
                il.EmitFieldSet(fieldInfo);
            }

            il.EmitLdloc(dest);
            il.EmitBox(type);
        }
    }
}