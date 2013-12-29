using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SF.Expressions
{
    /// <summary>
    ///     Performs bottom-up analysis to determine which nodes can possibly
    ///     be part of an evaluated sub-tree.
    /// </summary>
    internal class PartialEvaluationSearcher : ExpressionVisitor
    {
        private readonly Func<Expression, bool> _canBeEvaluated;
        private bool _cannotBeEvaluated;
        private HashSet<Expression> _evaluatable;

        public PartialEvaluationSearcher(Func<Expression, bool> canBeEvaluated)
        {
            _canBeEvaluated = canBeEvaluated;
        }

        internal HashSet<Expression> Process(Expression expression)
        {
            _evaluatable = new HashSet<Expression>();
            Visit(expression);
            return _evaluatable;
        }

        public override Expression Visit(Expression expression)
        {
            if (expression == null) 
                return null;
            var saveCannotBeEvaluated = _cannotBeEvaluated;
            _cannotBeEvaluated = false;
            base.Visit(expression);
            if (!_cannotBeEvaluated)
            {
                if (_canBeEvaluated(expression))
                {
                    _evaluatable.Add(expression);
                }
                else
                {
                    _cannotBeEvaluated = true;
                }
            }
            _cannotBeEvaluated |= saveCannotBeEvaluated;
            return expression;
        }
    }
}