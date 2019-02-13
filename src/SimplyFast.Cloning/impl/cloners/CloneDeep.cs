using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Blink.Common.Fast.Reflection;

namespace Blink.Common.TinyClone
{
    internal struct CloneFieldInfo
    {
        public readonly FastField Field;
        public readonly CloneType CloneType;

        public CloneFieldInfo(FastField field, CloneType cloneType)
        {
            Field = field;
            CloneType = cloneType;
        }
    }

    internal static class CloneDeepHelper
    {
        public static IEnumerable<CloneFieldInfo> GetTypeFields(FastType type)
        {
            return type
                .Fields
                .Where(x => !x.FieldInfo.IsStatic && x.FieldInfo.DeclaringType == type.Type)
                .Select(x =>
                {
                    var attrMember = x.GetDeclaringProperty()?.MemberInfo ?? x.MemberInfo;

                    var clone = CloneTypeCache.GetCloneTypeFromAttribute(attrMember) ??
                                CloneTypeCache.GetCloneType(x.FieldInfo.FieldType);

                    return new CloneFieldInfo(x, clone);
                });
        }
    }

    internal class CloneDeep<T>: ICloneType<T>
    {
        private readonly IFieldClone[] _cloneFields;

        public CloneDeep()
        {
            _cloneFields = BuildCloneFields();
        }

        private static IFieldClone[] BuildCloneFields()
        {
            return typeof(T)
                .Fast()
                .Hierarchy
                .SelectMany(CloneDeepHelper.GetTypeFields)
                .Where(x => x.CloneType != CloneType.Ignore)
                .Select(BuildCloneField)
                .ToArray();
        }

        private static IFieldClone BuildCloneField(CloneFieldInfo cloneField)
        {
            Type generic;
            switch (cloneField.CloneType)
            {
                case CloneType.Copy:
                    generic = typeof(FieldCopy<>);
                    break;
                case CloneType.Deep:
                    generic = typeof(FieldClone<>);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var field = cloneField.Field;
            return (IFieldClone)generic
                .Fast()
                .MakeGeneric(field.FieldInfo.FieldType)
                .Constructors
                .Single()
                .Invoke(field);
        }

        public T Clone(CloneContext cloneContext, T obj)
        {
            var result = (T) FormatterServices.GetUninitializedObject(typeof(T));
            foreach (var cloneField in _cloneFields)
            {
                cloneField.Clone(obj, result, cloneContext);
            }

            return result;
        }

        private interface IFieldClone
        {
            void Clone(T src, T dest, CloneContext context);
        }

        private class FieldHandler<TF>
        {
            protected readonly Func<T, TF> _getter;
            protected readonly Action<T, TF> _setter;

            protected FieldHandler(FastField field)
            {
                _getter = x => default;
                _setter = (x, v) => { Console.WriteLine(field.ToString()); };
                // TODO =\ This requires nice reflection, like SF...
            }
        }

        private class FieldCopy<TF> : FieldHandler<TF>, IFieldClone
        {
            public void Clone(T src, T dest, CloneContext context)
            {
                _setter(dest, _getter(src));
            }

            public FieldCopy(FastField field) : base(field)
            {
            }
        }

        private class FieldClone<TF> : FieldHandler<TF>,IFieldClone
        {
            public void Clone(T src, T dest, CloneContext context)
            {
                var clone = context.Clone(_getter(src));
                _setter(dest, clone);
            }

            public FieldClone(FastField field) : base(field)
            {
            }
        }
    }
}