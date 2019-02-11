//using System;
//using System.Linq.Expressions;
//using System.Reflection;

//namespace SimplyFast.Reflection.Internal.DelegateBuilders.Expressions
//{
//    internal class ExpressionFieldGetDelegateBuilder : ExpressionFieldAccessorDelegateBuilder
//    {
//        protected override void EmitInvoke(ILGenerator generator)
//        {
//            if (_fieldInfo.IsLiteral)
//            {
//                var value = _fieldInfo.GetValue(null);
//                generator.EmitLdConst(value);
//            }
//            else
//            {
//                generator.EmitFieldGet(_fieldInfo);
//            }
//        }
//    }
//}