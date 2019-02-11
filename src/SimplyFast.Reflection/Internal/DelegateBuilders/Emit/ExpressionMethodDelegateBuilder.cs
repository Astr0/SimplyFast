//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;

//namespace SimplyFast.Reflection.Internal.DelegateBuilders.Expressions
//{
//    internal class ExpressionMethodDelegateBuilder : ExpressionDelegateBuilder
//    {
//        public override Delegate CreateDelegate()
//        {
//            var declaring = _methodInfo.DeclaringType;
//            // avoid stupid issue in .Net framework...
//            if (declaring != null && declaring.IsInterface && _methodInfo.IsGenericMethod)
//                return base.CreateDelegate();

//            var delegateExcatlyMatch = IsDelegateExcactlyMatchMethod();
//            return delegateExcatlyMatch ? Delegate.CreateDelegate(_delegateType, _methodInfo) : base.CreateDelegate();
//        }

//        protected override void EmitInvoke(ILGenerator generator)
//        {
//            generator.EmitMethodCall(_methodInfo);
//        }

//    }
//}