//using System.Diagnostics.CodeAnalysis;
//using SimplyFast.Reflection.Internal.DelegateBuilders.Expressions;
//using SimplyFast.Reflection.Internal.DelegateBuilders.Maps;
//using System.Reflection.Emit;

//namespace SimplyFast.Reflection.Internal.DelegateBuilders.Parameters
//{
//    internal abstract class ArgParameterMap
//    {
//        public readonly SimpleParameterInfo DelegateParameter;
//        protected readonly int DelegateParameterIndex;
//        protected readonly SimpleParameterInfo MethodParameter;

//        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
//        protected ArgParameterMap(SimpleParameterInfo delegateParameter, int delegateParameterIndex,
//            SimpleParameterInfo methodParameter)
//        {
//            DelegateParameter = delegateParameter;
//            DelegateParameterIndex = delegateParameterIndex;
//            MethodParameter = methodParameter;
//            CheckParameters();
//        }

//        protected abstract void CheckParameters();

//        public static ArgParameterMap CreateParameterMap(SimpleParameterInfo delegateParameter, int delegateParameterIndex,
//            SimpleParameterInfo methodParameter)
//        {
//            if (methodParameter.IsOut)
//                return new ArgOutParameterMap(delegateParameter, delegateParameterIndex, methodParameter);
//            if (methodParameter.IsByRef)
//                return new ArgRefParameterMap(delegateParameter, delegateParameterIndex, methodParameter);
//            return new ArgNoneParameterMap(delegateParameter, delegateParameterIndex, methodParameter);
//        }

//        public abstract void EmitPrepare(ILGenerator generator);
//        public abstract void EmitLoad(ILGenerator generator);
//        public abstract void EmitFinish(ILGenerator generator);

//    }
//}