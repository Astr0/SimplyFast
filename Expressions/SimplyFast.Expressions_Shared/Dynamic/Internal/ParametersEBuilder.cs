using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SF.Expressions.Dynamic
{
    internal class ParametersEBuilder : DynamicEBuilder, IEnumerable<ParameterExpression>
    {
        private readonly List<ParameterExpression> _parameters = new List<ParameterExpression>();
        private readonly Dictionary<string, ParameterExpression> _parametersDictionary = new Dictionary<string, ParameterExpression>();
        private readonly bool _variables;

        public ParametersEBuilder(bool variables)
        {
            _variables = variables;
        }

        protected override Expression Operation
        {
            get { return null; }
        }

        #region IEnumerable<ParameterExpression> Members

        public IEnumerator<ParameterExpression> GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private ParameterExpression GetExpr(string name)
        {
            return _parametersDictionary[name];
        }

        private ParameterExpression DoVar(Type type, string name)
        {
            var variable = _variables ? Expression.Variable(type, name) : Expression.Parameter(type, name);
            _parametersDictionary.Add(name, variable);
            _parameters.Add(variable);
            return variable;
        }

        public ParameterExpression Var(Type type, string name)
        {
            ParameterExpression old;
            if (_parametersDictionary.TryGetValue(name, out old))
            {
                if (old.Type != type)
                    throw new InvalidOperationException(string.Format("Parameter already declared with type {0}. Can't re-declare it with type {1}", old.Type,
                        type));
                return old;
            }
            return DoVar(type, name);
        }

        protected override DynamicEBuilder Set(string name, Expression value)
        {
            ParameterExpression variable;
            if (!_parametersDictionary.TryGetValue(name, out variable))
            {
                variable = DoVar(value.Type, name);
            }
            return variable.Assign(value);
        }

        protected override DynamicEBuilder Binary(ExpressionType operation, Expression value)
        {
            throw new NotSupportedException();
        }

        protected override DynamicEBuilder Method(string name, params Expression[] args)
        {
            return GetExpr(name).InvokeDelegate(args);
        }

        protected override DynamicEBuilder GetIndex(params Expression[] index)
        {
            throw new NotSupportedException();
        }

        protected override DynamicEBuilder SetIndex(Expression[] index, Expression value)
        {
            throw new NotSupportedException();
        }

        protected override DynamicEBuilder Invoke(Expression[] args)
        {
            throw new NotSupportedException();
        }

        public override DynamicEBuilder Invoke(params object[] args)
        {
            if (_variables)
                throw new NotSupportedException("Need to inialize variables using x.VarName = something");
            if (args.Length == 0 || args.Length%2 != 0)
                throw new ArgumentException("Invalid arguments count, should be pairs of (Type, String)");
            Expression last = null;
            for (var i = 0; i < args.Length; i += 2)
            {
                var type = args[i] as Type;
                var name = args[i + 1] as string;
                if (type == null)
                    throw new ArgumentException("First argument should be Type");
                if (name == null)
                    throw new ArgumentException("Second argument should be String");
                last = Var(type, name);
            }
            return _variables ? null : last;
        }

        public override DynamicEBuilder Get(string name)
        {
            return GetExpr(name);
        }

        public override DynamicEBuilder Unary(ExpressionType operation)
        {
            throw new NotSupportedException();
        }

        public override DynamicEBuilder Convert(Type type)
        {
            throw new NotSupportedException();
        }
    }
}