using System.Collections.Generic;
using System.Linq.Expressions;

namespace SimplyFast.Reflection.Internal.DelegateBuilders.Expressions
{
    public class ExpressionBlockBuilder
    {
        private readonly List<Expression> _block = new List<Expression>();
        private List<ParameterExpression> _variables;

        public Expression Last
        {
            get
            {
                var count = _block.Count;
                return count != 0 ? _block[count - 1] : null;
            }
        }

        public void Add(Expression expression)
        {
            _block.Add(expression);
        }

        public void AddVariable(ParameterExpression variable)
        {
            if (_variables == null)
                _variables = new List<ParameterExpression>();
            _variables.Add(variable);
        }

        public void RemoveVariable(ParameterExpression variable)
        {
            _variables?.Remove(variable);
        }

        public void RemoveLast()
        {
            var count = _block.Count;
            if (count != 0)
                _block.RemoveAt(count - 1);
        }

        public Expression CreateExpression()
        {
            return _block.Count == 1 && (_variables == null || _variables.Count == 0)
                ? _block[0]
                : Expression.Block(_variables, _block);
        }
    }
}
