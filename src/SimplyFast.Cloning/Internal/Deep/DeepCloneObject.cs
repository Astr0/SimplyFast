using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using SimplyFast.Reflection;

namespace SimplyFast.Cloning.Internal.Deep
{
    internal class DeepCloneObject: ICloneObject
    {
        private readonly Type _type;
        private readonly FieldClone[] _cloneFields;

        public DeepCloneObject(Type type)
        {
            _type = type;
            _cloneFields = BuildCloneFields(type);
        }

        private static FieldClone[] BuildCloneFields(Type type)
        {
            return DeepCloneHelper
                    .GetDeepCloneFields(type)
                    .Where(x => x.Field.CanWrite())
                    .Select(x => new FieldClone(x))
                    .ToArray();
        }

        public object Clone(ICloneContext context, object src)
        {
            var target = FormatterServices.GetUninitializedObject(_type);
            foreach (var cloneField in _cloneFields)
            {
                var value = cloneField.Get(src);
                if (cloneField.Deep)
                    value = context.Clone(value);
                cloneField.Set(target, value);
            }

            return target;
        }

        private struct FieldClone
        {
            public readonly bool Deep;
            public readonly Func<object, object> Get;
            public readonly Action<object, object> Set;

            public FieldClone(CloneFieldInfo field)
            {
                Debug.Assert(field.CloneType != CloneType.Ignore);
                Deep = field.CloneType != CloneType.Copy;
                Get = field.Field.GetterAs<Func<object, object>>();
                Set = field.Field.SetterAs<Action<object, object>>();
            }
        }

    }
}