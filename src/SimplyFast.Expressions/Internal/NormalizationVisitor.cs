using System.Linq;
using System.Linq.Expressions;
using SimplyFast.Reflection;

namespace SimplyFast.Expressions.Internal
{
    internal class NormalizationVisitor : ExpressionVisitor
    {
        public static readonly NormalizationVisitor Instance = new NormalizationVisitor();

        private NormalizationVisitor()
        {
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType != ExpressionType.ArrayIndex)
                return base.VisitBinary(node);
            // rewrite...
            return Visit(node.Left).Array(Visit(node.Right));
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var method = node.Method;
            var property = PropertyInfoEx.Property(method);
            if (property == null)
                return base.VisitMethodCall(node);
            // rewrite...
            var instance = Visit(node.Object);
            var isSet = ReferenceEquals(property.SetMethod, method);
            if (!isSet)
            {
                if (node.Arguments.Count == 0)
                    return instance.Property(property);
                return instance.Property(property, node.Arguments.Select(Visit));
            }
            // set method =\
            // last argument is value, other - index
            if (node.Arguments.Count == 1)
                return instance.Property(property).Assign(Visit(node.Arguments[0]));
            return instance.Property(property, node.Arguments.Take(node.Arguments.Count - 1).Select(Visit))
                .Assign(node.Arguments[node.Arguments.Count - 1]);
        }
    }
}