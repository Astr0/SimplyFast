//using System;
//using System.Reflection.Emit;
//using SimplyFast.Reflection.Internal.DelegateBuilders.Expressions;
//using SimplyFast.Reflection.Internal.DelegateBuilders.Maps;
//using System.Reflection.Emit;

//namespace SimplyFast.Reflection.Internal.DelegateBuilders.Parameters
//{
//    internal abstract class ArgLocalVariableParameterMap : ArgParameterMap
//    {
//        protected readonly bool NeedLocalVariable;
//        private readonly Type _methodType;

//        protected ArgLocalVariableParameterMap(SimpleParameterInfo delegateParameter, int delegateParameterIndex,
//            SimpleParameterInfo methodParameter)
//            : base(delegateParameter, delegateParameterIndex, methodParameter)
//        {
//            _methodType = methodParameter.Type.RemoveByRef();
//            var delegateType = delegateParameter.Type.RemoveByRef();
//            if (!delegateType.IsAssignableFrom(_methodType))
//                throw new ArgumentException("Invalid type for parameter " + delegateParameterIndex);
//            NeedLocalVariable = delegateType != _methodType;
//        }

        
//        protected LocalBuilder _localVariable;
//        public override void EmitPrepare(ILGenerator generator)
//        {
//            if (_needLocalVariable)
//                _localVariable = generator.DeclareLocal(_methodType);
//        }
//    }
//}