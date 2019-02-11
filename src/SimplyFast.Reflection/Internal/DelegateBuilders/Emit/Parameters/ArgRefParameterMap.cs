//using System;
//using SimplyFast.Reflection.Internal.DelegateBuilders.Expressions;
//using SimplyFast.Reflection.Internal.DelegateBuilders.Maps;
//using System.Reflection.Emit;
//using SimplyFast.Reflection.Emit;


//namespace SimplyFast.Reflection.Internal.DelegateBuilders.Parameters
//{
//    internal class ArgRefParameterMap : ArgOutParameterMap
//    {
//        // Method ref, delegate any
//        public ArgRefParameterMap(SimpleParameterInfo delegateParameter, int delegateParameterIndex,
//            SimpleParameterInfo methodParameter)
//            : base(delegateParameter, delegateParameterIndex, methodParameter)
//        {
//        }

//        protected override void CheckParameters()
//        {
//            if (!MethodParameter.Type.IsByRef)
//                throw new ArgumentException("Invalid methodParameter modifier. Should be Ref.");
//            //if (!(_delegateParameter.ParameterType.IsByRef || _del))
//            //    throw new ArgumentException(string.Format("Invalid modifier for parameter {0}. Should be Ref or None.",
//            //                                              _delegateParameterIndex));
//        }


//        public override void EmitFinish(ILGenerator generator)
//        {
//            if (_delegateParameter.Type.IsByRef)
//                base.EmitFinish(generator);
//        }

//        public override void EmitLoad(ILGenerator generator)
//        {
//            if (!_delegateParameter.Type.IsByRef && !_needLocalVariable)
//                generator.EmitLdarga(_delegateParameterIndex);
//            else
//                base.EmitLoad(generator);
//        }

//        public override void EmitPrepare(ILGenerator generator)
//        {
//            base.EmitPrepare(generator);
//            if (!_needLocalVariable)
//                return;
//            generator.EmitLdarg(_delegateParameterIndex);
//            var dt = _delegateParameter.Type.RemoveByRef();
//            var mt = _methodParameter.Type.RemoveByRef();

//            if (_delegateParameter.Type.IsByRef)
//                generator.EmitLdind(dt);
//            if (dt.IsValueType && !mt.IsValueType)
//                generator.EmitBox(dt);
//            else if (!mt.IsAssignableFrom(dt))
//                generator.EmitUnBoxAnyOrCastClass(mt);
//            generator.EmitStloc(_localVariable.LocalIndex);
//        }
//    }
//}