//using System;
//using System.Reflection;
//using SF.Common;

//namespace SF.Reflection
//{
//    public static class SimpleConvert
//    {
//        private static readonly MethodInfo _exactCreate = SimpleMethodInfo.Method((int x) => ExactConverter(x)).GetGenericMethodDefinition();
//        private static readonly MethodInfo _castCreate = SimpleMethodInfo.Method((int x) => CastConverter<int, int>(x)).GetGenericMethodDefinition();


//        private static T ExactConverter<T>(T value)
//        {
//            return value;
//        }

//        private static TResult CastConverter<TSource, TResult>(TSource value)
//            where TSource : TResult
//        {
//            return value;
//        }

//        public static Func<TSource, TResult> Converter<TSource, TResult>()
//        {
//            if (typeof (TResult).IsAssignableFrom(typeof (TSource)))
//            {
//                // Cast or Exact
//                return typeof (TResult) == typeof (TSource)
//                           ? _exactCreate.MakeGeneric(typeof (TSource)).InvokerAs<Func<TSource, TResult>>()
//                           : _castCreate.MakeGeneric(typeof(TSource), typeof(TResult)).InvokerAs<Func<TSource, TResult>>();
//            }
//            // Find Convert.To* method
//            var convertTo = FindConvertTo<TSource, TResult>();
//            if (convertTo != null)
//                return convertTo.InvokerAs<Func<TSource, TResult>>();
//            // If source is just object - use Convert.ChangeType
//            if (typeof (TSource) == typeof (object))
//                return x => (TResult) Convert.ChangeType(x, typeof (TResult));

//            // if source is IConvertible, use IConvertible method
//            if (typeof (IConvertible).IsAssignableFrom(typeof (TSource)))
//            {
//                var convertibleTo = FindConvertibleTo<TResult>();
//                if (convertibleTo != null)
//                    return convertibleTo.InvokerAs<Func<TSource, TResult>>();
//                return x => (TResult) ((IConvertible) x).ToType(typeof (TResult), null);
//            }
//            return x => { throw new InvalidCastException(); };
//        }

//        private static string GetConvertToMethod<TResult>()
//        {
//            var type = typeof (TResult);
//            if (type == typeof (Boolean))
//                return "ToBoolean";
//            if (type == typeof (Char))
//                return "ToChar";
//            if (type == typeof (SByte))
//                return "ToSByte";
//            if (type == typeof (Byte))
//                return "ToByte";
//            if (type == typeof (Int16))
//                return "ToInt16";
//            if (type == typeof (UInt16))
//                return "ToUInt16";
//            if (type == typeof (Int32))
//                return "ToInt32";
//            if (type == typeof (UInt32))
//                return "ToUInt32";
//            if (type == typeof (Int64))
//                return "ToInt64";
//            if (type == typeof (UInt64))
//                return "ToUInt64";
//            if (type == typeof (Single))
//                return "ToSingle";
//            if (type == typeof (Double))
//                return "ToDouble";
//            if (type == typeof (Decimal))
//                return "ToDecimal";
//            if (type == typeof (DateTime))
//                return "ToDateTime";
//            if (type == typeof (String))
//                return "ToString";
//            return null;
//        }

//        private static MethodInfo FindConvertTo<TSource, TResult>()
//        {
//            var method = GetConvertToMethod<TResult>();
//            return method == null ? null : typeof(Convert).Method(method, typeof(TSource));
//        }

//        private static MethodInfo FindConvertibleTo<TResult>()
//        {
//            var method = GetConvertToMethod<TResult>();
//            return method == null ? null : typeof(IConvertible).Method(method);
//        }
//    }
//}