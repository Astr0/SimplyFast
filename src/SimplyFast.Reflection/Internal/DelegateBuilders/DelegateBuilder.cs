using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SF.Reflection.DelegateBuilders
{
    internal abstract class DelegateBuilder
    {
        private readonly ParameterInfo[] _delegateParams;
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
            if (_delegateType.BaseType != typeof (MulticastDelegate))
                throw NotDelegateException();
            var invokeMethod = _delegateType.GetMethod("Invoke");
            if (invokeMethod == null)
                throw NotDelegateException();
            _delegateParams = invokeMethod.GetParameters();
            _delegateReturn = invokeMethod.ReturnType;
        }

        public Delegate CreateDelegate()
        {
            MapParameters();
            return _delegateExcatlyMatch ? CreateExactDelegate() : CreateCastDelegate();
        }

        protected abstract Type GetMethodReturnType();
        protected abstract ParameterInfo[] GetMethodParameters();

        private static bool ParametersEquals(IList<ParameterInfo> first, IList<ParameterInfo> second)
        {
            if (first.Count != second.Count)
                return false;
            return !first.Where((t, i) => t.IsOut != second[i].IsOut || t.ParameterType != second[i].ParameterType).Any();
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
                IList<ParameterInfo> methodParameters;
                var _this = GetThisParameterForMethod();
                if (_this != null)
                {
                    var list = new List<ParameterInfo>(parameters.Length + 1)
                    {
                        _this
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

        protected abstract ParameterInfo GetThisParameterForMethod();

        private Delegate CreateCastDelegate()
        {
            var paramTypes = _delegateParams.Select(x => x.ParameterType).ToArray();
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

        protected virtual Delegate CreateExactDelegate()
        {
            return CreateCastDelegate();
        }

        protected abstract void EmitInvoke(ILGenerator generator);

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