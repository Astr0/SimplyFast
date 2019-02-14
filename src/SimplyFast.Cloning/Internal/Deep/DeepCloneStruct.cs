using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using SimplyFast.Reflection;

namespace SimplyFast.Cloning.Internal.Deep
{
    internal class DeepCloneStruct
    {
        private readonly Type _type;
        private readonly FieldClone[] _cloneFields;

        public DeepCloneStruct(Type type)
        {
            Debug.Assert(type.IsValueType);
            _type = type;
            _cloneFields = DeepCloneHelper
                .GetDeepCloneFields(type)
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
                cloneField.Set(ref target, value);
            }

            return target;
        }

        private delegate void SetStructHelper(ref object o, object v);

        private struct FieldClone
        {
            public readonly bool Deep;
            public readonly Func<object, object> Get;
            public readonly SetStructHelper Set;

            public FieldClone(CloneFieldInfo field)
            {
                Debug.Assert(field.CloneType != CloneType.Ignore);
                Deep = field.CloneType != CloneType.Copy;
                Set = field.Field.SetterAs<SetStructHelper>();
                Get = field.Field.GetterAs<Func<object, object>>();
                Debug.Assert(Set != null && Get != null);
            }
        }

    }
}