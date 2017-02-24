using System;
using System.Diagnostics.CodeAnalysis;
using SimplyFast.Collections;
using SimplyFast.Reflection.Internal.DelegateBuilders.Parameters;

#if EMIT
using System.Reflection.Emit;
#else
using System.Collections.Generic;
using System.Linq.Expressions;
#endif

namespace SimplyFast.Reflection.Internal.DelegateBuilders
{
    internal abstract class DelegateBuilder
    {
        protected readonly SimpleParameterInfo[] _delegateParams;
        protected readonly Type _delegateReturn;
        protected readonly Type _delegateType;
        protected readonly Type _methodReturn;
        //protected readonly MethodBase _method;
        private ArgParameterMap[] _parametersMap;
        private RetValMap _retValMap;
        protected readonly SimpleParameterInfo[] _methodParameters;

        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        protected DelegateBuilder(Type delegateType)
        {
            _delegateType = delegateType;
            if (_delegateType == null)
                throw new ArgumentNullException(nameof(delegateType));
            if (_delegateType.TypeInfo().BaseType != typeof (MulticastDelegate))
                throw NotDelegateException();
            var invokeMethod = _delegateType.Method("Invoke");
            if (invokeMethod == null)
                throw NotDelegateException();
            _delegateParams = SimpleParameterInfo.FromParameters(invokeMethod.GetParameters());
            _delegateReturn = invokeMethod.ReturnType;

            _methodReturn = GetMethodReturnType();
            _methodParameters = BuildMethodParameters();
        }

        public virtual Delegate CreateDelegate()
        {
            MapParameters();
            return CreateCastDelegate();
        }

        protected abstract Type GetMethodReturnType();
        protected abstract SimpleParameterInfo[] GetMethodParameters();

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

        private SimpleParameterInfo[] BuildMethodParameters()
        {
            var parameters = GetMethodParameters();
            // Check return
            var _this = GetThisParameterForMethod();
            if (_this == null)
            {
                return parameters;
            }
            var fullParameters = new SimpleParameterInfo[parameters.Length + 1];
            fullParameters[0] = new SimpleParameterInfo(_this);
            Array.Copy(parameters, 0, fullParameters, 1, parameters.Length);
            return fullParameters;
        }

        protected abstract Type GetThisParameterForMethod();

#if EMIT
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
            var block = new List<Expression>();
            var invokeParameters = parameters
                .ConvertAll((p, i) => _parametersMap[i].Prepare(block, p));
            var ret = Invoke(block, invokeParameters);
            for (var i = 0; i < parameters.Length; i++)
            {
                _parametersMap[i].Finish(block, parameters[i]);
            }
            block.Add(_retValMap.ConvertReturn(ret));

            var body = block.Count != 1 ? Expression.Block(block) : block[0];
            var lambda = Expression.Lambda(body, parameters);
            return lambda.Compile();
        }

        protected abstract Expression Invoke(List<Expression> block, Expression[] parameters);
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