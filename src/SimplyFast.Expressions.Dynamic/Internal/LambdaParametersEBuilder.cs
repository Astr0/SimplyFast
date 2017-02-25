using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SimplyFast.Expressions.Dynamic.Internal
{
    internal class LambdaParametersEBuilder : DynamicEBuilder
    {
        private readonly Dictionary<string, ParameterExpression> _parameters;

        public LambdaParametersEBuilder(IEnumerable<ParameterExpression> parameters)
        {
            _parameters = parameters.ToDictionary(x => x.Name);
        }

        protected override Expression Operation
        {
            get { throw new NotSupportedException(); }
        }

        protected override DynamicEBuilder Set(string name, Expression value)
        {
            return _parameters[name].Assign(value);
        }

        protected override DynamicEBuilder Binary(ExpressionType operation, Expression value)
        {
            throw new NotSupportedException();
        }

        protected override DynamicEBuilder Method(string name, params Expression[] args)
        {
            throw new NotSupportedException();
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

        public override DynamicEBuilder Get(string name)
        {
            return _parameters[name];
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