using System;
using System.Linq.Expressions;

namespace SF.Expressions.Dynamic
{
    internal class OperationEBuilder : DynamicEBuilder
    {
        private readonly Expression _operation;

        public OperationEBuilder(Expression operation)
        {
            _operation = operation;
        }


        protected override Expression Operation
        {
            get { return _operation; }
        }

        public override string ToString()
        {
            return Operation.ToString();
        }

        public override DynamicEBuilder Get(string name)
        {
            return Operation.MemberAccess(name);
        }

        protected override DynamicEBuilder Set(string name, Expression value)
        {
            return Operation.MemberAccess(name).Assign(value);
        }

        public override DynamicEBuilder Unary(ExpressionType operation)
        {
            return Operation.Unary(operation);
        }

        protected override DynamicEBuilder Binary(ExpressionType operation, Expression value)
        {
            return Operation.Binary(operation, value);
        }

        public override DynamicEBuilder Convert(Type type)
        {
            return Operation.Convert(type);
        }

        protected override DynamicEBuilder Method(string name, params Expression[] args)
        {
            return Operation.Method(name, args);
        }

        protected override DynamicEBuilder GetIndex(params Expression[] index)
        {
            return Operation.Index(index);
        }

        protected override DynamicEBuilder SetIndex(Expression[] index, Expression value)
        {
            return Operation.Index(index).Assign(value);
        }

        protected override DynamicEBuilder Invoke(Expression[] args)
        {
            return Operation.InvokeDelegate(args);
        }
    }
}