using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SF.Expressions
{
    /// <summary>
    ///     Evaluates & replaces sub-trees when first candidate is reached (top-down)
    /// </summary>
    internal class PartialEvaluator : ExpressionVisitor
    {
        private readonly HashSet<Expression> _canBeEvaluated;
        private bool _allowEval = true;

        internal PartialEvaluator(HashSet<Expression> canBeEvaluated)
        {
            _canBeEvaluated = canBeEvaluated;
        }

        internal Expression Eval(Expression exp)
        {
            return Visit(exp);
        }

        public override Expression Visit(Expression exp)
        {
            if (!_allowEval)
                return base.Visit(exp);
            return TryEval(exp);
        }

        private Expression TryEval(Expression exp)
        {
            if (exp == null)
            {
                return null;
            }
            return _canBeEvaluated.Contains(exp) ? Evaluate(exp) : base.Visit(exp);
        }

        protected override Expression VisitListInit(ListInitExpression node)
        {
            // If we here, than eval the whole block failed, don't touch constructor!
            _allowEval = false;
            var newExpr = VisitAndConvert(node.NewExpression, "VisitListInit");
            _allowEval = true;
            // ReSharper disable once AssignNullToNotNullAttribute
            return node.Update(newExpr, Visit(node.Initializers, VisitElementInit));
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            // If we here, than eval the whole block failed, don't touch constructor!
            _allowEval = false;
            var newExpr = VisitAndConvert(node.NewExpression, "VisitMemberInit");
            _allowEval = true;
            // ReSharper disable once AssignNullToNotNullAttribute
            return node.Update(newExpr, Visit(node.Bindings, VisitMemberBinding));
        }

        private Expression Evaluate(Expression e)
        {
            if (e.NodeType == ExpressionType.Constant || e.NodeType == ExpressionType.Default)
            {
                return e;
            }
            try
            {
                if (e.Type != typeof (void))
                {
                    var eval = e.TypeAs(typeof (object));
                    var lambda = Expression.Lambda<Func<object>>(eval);
                    var func = lambda.Compile();
                    return Expression.Constant(func(), e.Type);
                }
                else
                {
                    var lambda = Expression.Lambda<Action>(e);
                    var action = lambda.Compile();
                    action();
                    return Expression.Default(typeof (void));
                }
            }
            catch
            {
                // Don't eval if catched something =\
                return base.Visit(e);
            }
        }
    }
}