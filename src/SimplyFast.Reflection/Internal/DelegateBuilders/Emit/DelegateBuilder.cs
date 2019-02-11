//using SimplyFast.Comparers;
//using System.Collections.Generic;
//using System.Reflection.Emit;

//namespace SimplyFast.Reflection.Internal.DelegateBuilders.Emit
//{
//    internal abstract class DelegateBuilder
//    {

//        private static readonly EqualityComparer<SimpleParameterInfo[]> ParametersComparer =
//            EqualityComparerEx.Array<SimpleParameterInfo>();

//        protected bool IsDelegateExcactlyMatchMethod()
//        {
//            return _delegateReturn == _methodReturn &&
//                                        ParametersComparer.Equals(_delegateParams, _methodParameters);
//        }


//        private Delegate CreateCastDelegate()
//        {
//            var paramTypes = _delegateParams.ConvertAll(x => x.Type);
//            var m = new DynamicMethod(string.Empty, _delegateReturn, paramTypes,
//                typeof(DelegateBuilder), MemberInfoEx.PrivateAccess);
//            var cg = m.GetILGenerator();
//            // Prepare parameters...
//            foreach (var parameterMap in _parametersMap)
//            {
//                parameterMap.EmitPrepare(cg);
//            }
//            // Load parameters, stack should be empty here
//            foreach (var parameterMap in _parametersMap)
//            {
//                parameterMap.EmitLoad(cg);
//            }
//            // Emit invoke
//            EmitInvoke(cg);
//            // Emit finish, stack should contain return value here (if not void)
//            foreach (var parameterMap in _parametersMap)
//            {
//                parameterMap.EmitFinish(cg);
//            }
//            // Emit return
//            _retValMap.EmitConvert(cg);
//            cg.Emit(OpCodes.Ret);
//            return m.CreateDelegate(_delegateType);
//        }

//        protected abstract void EmitInvoke(ILGenerator generator);


//    }
//}