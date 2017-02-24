using System;
using System.Diagnostics.CodeAnalysis;
using SimplyFast.Collections;
using SimplyFast.Reflection.Internal.DelegateBuilders.Parameters;

#if EMIT
using SimplyFast.Comparers;
using System.Collections.Generic;
using System.Reflection.Emit;
#else
using System.Linq.Expressions;
#endif

namespace SimplyFast.Reflection.Internal.DelegateBuilders
{
    internal abstract class DelegateBuilder
    {
        private readonly SimpleParameterInfo[] _delegateParams;
        private readonly Type _delegateReturn;
        protected readonly Type _delegateType;
        private Type _methodReturn;
        private ArgParameterMap[] _parametersMap;
        private RetValMap _retValMap;
        private SimpleParameterInfo[] _methodParameters;

        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        protected DelegateBuilder(Type delegateType)
        {
            if (delegateType == null)
                throw new ArgumentNullException(nameof(delegateType));
            _delegateType = delegateType;
            if (delegateType.TypeInfo().BaseType != typeof (MulticastDelegate))
                throw NotDelegateException();
            var invokeMethod = delegateType.Method("Invoke");
            if (invokeMethod == null)
                throw NotDelegateException();
            _delegateParams = SimpleParameterInfo.FromParameters(invokeMethod.GetParameters());
            _delegateReturn = invokeMethod.ReturnType;
        }

        protected void Init(Type thisParameter, SimpleParameterInfo[] methodParameters, Type methodReturn)
        {
            _methodReturn = methodReturn;
            // Check return
            if (thisParameter == null)
            {
                _methodParameters = methodParameters;
                return;
            }
            var fullParameters = new SimpleParameterInfo[methodParameters.Length + 1];
            fullParameters[0] = new SimpleParameterInfo(thisParameter);
            Array.Copy(methodParameters, 0, fullParameters, 1, methodParameters.Length);
            _methodParameters = fullParameters;
        }

        [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
        public virtual Delegate CreateDelegate()
        {
            MapParameters();
            return CreateCastDelegate();
        }

        private void MapParameters()
        {
            if (_parametersMap != null)
                return;
            try
            {

                // Check param count
                if (_delegateParams.Length != _methodParameters.Length)
                    throw new Exception("Invalid parameters count.");

                _parametersMap = _delegateParams
                    .ConvertAll((delegateParam, i) => 
                    ArgParameterMap.CreateParameterMap(delegateParam, i, _methodParameters[i]));

                _retValMap = new RetValMap(_delegateReturn, _methodReturn);
            }
            catch (Exception ex)
            {
                throw InvalidSignatureException(ex);
            }
        }


#if EMIT
        private static readonly EqualityComparer<SimpleParameterInfo[]> ParametersComparer =
            EqualityComparerEx.Array<SimpleParameterInfo>();

        protected bool IsDelegateExcactlyMatchMethod()
        {
            return _delegateReturn == _methodReturn &&
                                        ParametersComparer.Equals(_delegateParams, _methodParameters);
        }


        private Delegate CreateCastDelegate()
        {
            var paramTypes = _delegateParams.ConvertAll(x => x.Type);
            var m = new DynamicMethod(string.Empty, _delegateReturn, paramTypes,
                typeof(DelegateBuilder), MemberInfoEx.PrivateAccess);
            var cg = m.GetILGenerator();
            // Prepare parameters...
            foreach (var parameterMap in _parametersMap)
            {
                parameterMap.EmitPrepare(cg);
            }
            // Load parameters, stack should be empty here
            foreach (var parameterMap in _parametersMap)
            {
                parameterMap.EmitLoad(cg);
            }
            // Emit invoke
            EmitInvoke(cg);
            // Emit finish, stack should contain return value here (if not void)
            foreach (var parameterMap in _parametersMap)
            {
                parameterMap.EmitFinish(cg);
            }
            // Emit return
            _retValMap.EmitConvert(cg);
            cg.Emit(OpCodes.Ret);
            return m.CreateDelegate(_delegateType);
        }

        protected abstract void EmitInvoke(ILGenerator generator);
#else
        private Delegate CreateCastDelegate()
        {
            var parameters = _delegateParams.ConvertAll(p => Expression.Parameter(p.Type));
            var block = new ExpressionBlockBuilder();
            var invokeParameters = parameters
                .ConvertAll((p, i) => _parametersMap[i].Prepare(block, p));
            var invoke = Invoke(invokeParameters);
            _retValMap.Prepare(block, invoke);
            for (var i = 0; i < parameters.Length; i++)
            {
                _parametersMap[i].Finish(block, parameters[i]);
            }
            _retValMap.ConvertReturn(block);

            var body = block.CreateExpression();
            var lambda = Expression.Lambda(_delegateType, body, parameters);
            return lambda.Compile();
        }

        protected abstract Expression Invoke(Expression[] parameters);
#endif

        private Exception NotDelegateException()
        {
            return new InvalidOperationException(_delegateType + " is not delegate type.");
        }

        private Exception InvalidSignatureException(Exception innerException)
        {
            return new InvalidOperationException(_delegateType + " has invalid signature.", innerException);
        }
    }
}