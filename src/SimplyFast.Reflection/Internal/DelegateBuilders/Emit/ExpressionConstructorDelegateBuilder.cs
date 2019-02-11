//using System;
//using System.Linq.Expressions;
//using System.Reflection;

//namespace SimplyFast.Reflection.Internal.DelegateBuilders.Expressions
//{
//    internal class ExpressionConstructorDelegateBuilder : ExpressionDelegateBuilder
//    {
//        protected override void EmitInvoke(ILGenerator generator)
//        {
//            generator.Emit(OpCodes.Newobj, _constructorInfo);
//        }
//    }
//}