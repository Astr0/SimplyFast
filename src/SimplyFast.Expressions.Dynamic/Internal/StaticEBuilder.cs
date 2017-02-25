using System;
using System.Linq.Expressions;
using SimplyFast.Reflection;

namespace SimplyFast.Expressions.Dynamic.Internal
{
    internal class StaticEBuilder : DynamicEBuilder
    {
        private readonly Type _type;

        public StaticEBuilder(Type type)
        {
            _type = type;
        }

        protected override Expression Operation
        {
            get { return Expression.Constant(_type, typeof (Type)); }
        }

        protected override DynamicEBuilder Set(string name, Expression value)
        {
            return GetExpr(name).Assign(value);
        }

        protected override DynamicEBuilder Binary(ExpressionType operation, Expression value)
        {
            throw new NotSupportedException();
        }

        protected override DynamicEBuilder Method(string name, params Expression[] args)
        {
            return EBuilder.Method(_type, name, args);
        }

        protected override DynamicEBuilder GetIndex(params Expression[] index)
        {
            return GetIndexExpr(index);
        }

        private IndexExpression GetIndexExpr(Expression[] index)
        {
            return EBuilder.Index(_type, index);
        }

        protected override DynamicEBuilder SetIndex(Expression[] index, Expression value)
        {
            return GetIndexExpr(index).Assign(value);
        }

        protected override DynamicEBuilder Invoke(Expression[] args)
        {
            return EBuilder.New(_type, args);
        }

        public override DynamicEBuilder Get(string name)
        {
            return GetExpr(name);
        }

        private MemberExpression GetExpr(string name)
        {
            return EBuilder.MemberAccess(null, _type.FieldOrProperty(name));
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