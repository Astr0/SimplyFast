using System;
using System.Linq.Expressions;

namespace SimplyFast.Expressions.Internal
{
    internal class ReplaceParametersVisitor : ExpressionVisitor
    {
        private readonly Func<ParameterExpression, Expression> _replace;

        public ReplaceParametersVisitor(Func<ParameterExpression, Expression> replace)
        {
            _replace = replace;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return _replace(node) ?? base.VisitParameter(node);
        }
    }
}