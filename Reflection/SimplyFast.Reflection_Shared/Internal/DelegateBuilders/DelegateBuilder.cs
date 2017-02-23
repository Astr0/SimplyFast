using System;
using System.Collections.Generic;
using System.Linq;
using SF.Collections;
using SF.Reflection.Internal.DelegateBuilders.Parameters;
#if EMIT
using System.Reflection.Emit;
#endif

namespace SF.Reflection.Internal.DelegateBuilders
{
    internal abstract class DelegateBuilder
    {
        private readonly SimpleParameterInfo[] _delegateParams;
        private readonly Type _delegateReturn;
        protected readonly Type _delegateType;
        //protected readonly MethodBase _method;
        private bool _delegateExcatlyMatch;
        private List<IDelegateParameterMap> _parametersMap;

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
        }

        public Delegate CreateDelegate()
        {
            MapParameters();
            return _delegateExcatlyMatch ? CreateExactDelegate() : CreateCastDelegate();
        }

        protected abstract Type GetMethodReturnType();
        protected abstract SimpleParameterInfo[] GetMethodParameters();

        private static bool ParametersEquals(ICollection<SimpleParameterInfo> first, IList<SimpleParameterInfo> second)
        {
            return first.Count == second.Count && first.SequenceEqual(second);
        }

        private void MapParameters()
        {
            if (_parametersMap != null)
                return;
            _parametersMap = new List<IDelegateParameterMap>();
            try
            {
                var parameters = GetMethodParameters();
                var methodReturn = GetMethodReturnType();
                // Check return
                IList<SimpleParameterInfo> methodParameters;
                var _this = GetThisParameterForMethod();
                if (_this != null)
                {
                    var list = new List<SimpleParameterInfo>(parameters.Length + 1)
                    {
                        new SimpleParameterInfo(_this)
                    };
                    list.AddRange(parameters);
                    methodParameters = list;
                }
                else
                {
                    methodParameters = parameters;
                }

                // Check param count
                if (_delegateParams.Length != methodParameters.Count)
                    throw new Exception("Invalid parameters count.");

                _delegateExcatlyMatch = _delegateReturn == methodReturn &&
                                        ParametersEquals(_delegateParams, methodParameters);

                // Map params
                for (var i = 0; i < _delegateParams.Length; i++)
                {
                    var delegateParam = _delegateParams[i];
                    var methodParam = methodParameters[i];
                    _parametersMap.Add(ArgParameterMap.CreateParameterMap(delegateParam, i, methodParam));
                }
                _parametersMap.Add(new RetValParameterMap(_delegateReturn, methodReturn));
            }
            catch (Exception ex)
            {
                throw InvalidSignatureException(ex);
            }
        }

        protected abstract Type GetThisParameterForMethod();

#if EMIT
        protected Delegate CreateCastDelegate()
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
            cg.Emit(OpCodes.Ret);
            return m.CreateDelegate(_delegateType);
        }

        protected abstract void EmitInvoke(ILGenerator generator);
#else
        protected Delegate CreateCastDelegate()
        {
            throw new NotSupportedException();
        }

#endif

        protected virtual Delegate CreateExactDelegate()
        {
            return CreateCastDelegate();
        }

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