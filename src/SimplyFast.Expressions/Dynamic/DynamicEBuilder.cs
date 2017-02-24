using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SimplyFast.Expressions.Dynamic.Internal;

namespace SimplyFast.Expressions.Dynamic
{
    public abstract class DynamicEBuilder : IDynamicMetaObjectProvider // DynamicObject
    {
        #region IDynamicMetaObjectProvider Members

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
        {
            return GetMetaObject(parameter);
        }

        #endregion

        #region Conversions

        protected abstract Expression Operation { get; }

        public static implicit operator Expression(DynamicEBuilder v)
        {
            return v.Operation;
        }

        public static implicit operator DynamicEBuilder(Expression x)
        {
            return new OperationEBuilder(x);
        }

        #endregion

        #region Boilerplate

        public static Expression ToExpression(object value)
        {
            var expression = value as Expression;
            if (expression != null)
                return expression;
            var builder = value as DynamicEBuilder;
            return builder ?? Expression.Constant(value);
        }

        public static Expression[] ToExpressions(params object[] args)
        {
            return Array.ConvertAll(args, ToExpression);
        }

        public static Expression[] ToExpressions(IEnumerable<object> args)
        {
            return args.Select(ToExpression).ToArray();
        }

        #endregion

        #region Protected

        protected abstract DynamicEBuilder Set(string name, Expression value);
        protected abstract DynamicEBuilder Binary(ExpressionType operation, Expression value);
        protected abstract DynamicEBuilder Method(string name, params Expression[] args);
        protected abstract DynamicEBuilder GetIndex(params Expression[] index);
        protected abstract DynamicEBuilder SetIndex(Expression[] index, Expression value);
        protected abstract DynamicEBuilder Invoke(Expression[] args);

        protected virtual DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new MetaBuilder(parameter, this);
        }

        #endregion

        #region Public

        public abstract DynamicEBuilder Get(string name);

        public DynamicEBuilder Set(string name, object value)
        {
            return Set(name, ToExpression(value));
        }

        public abstract DynamicEBuilder Unary(ExpressionType operation);

        public DynamicEBuilder Binary(ExpressionType operation, object value)
        {
            return Binary(operation, ToExpression(value));
        }

        public abstract DynamicEBuilder Convert(Type type);

        public virtual DynamicEBuilder Method(string name, params object[] args)
        {
            return Method(name, ToExpressions(args));
        }

        public DynamicEBuilder GetIndex(params object[] args)
        {
            return GetIndex(ToExpressions(args));
        }

        public DynamicEBuilder SetIndex(object[] index, object value)
        {
            return SetIndex(ToExpressions(index), ToExpression(value));
        }

        public virtual DynamicEBuilder Invoke(params object[] args)
        {
            return Invoke(ToExpressions(args));
        }

        #endregion

        #region Factory

        /// <summary>
        ///     Creates DynamicEBuilder from expression
        /// </summary>
        public static DynamicEBuilder Create(Expression expression)
        {
            return new OperationEBuilder(expression);
        }

        /// <summary>
        ///     Creates DynamicEBuilder from type
        /// </summary>
        public static DynamicEBuilder Create(Type type)
        {
            return new StaticEBuilder(type);
        }

        #endregion

        #region Nested type: MetaBuilder

        private class MetaBuilder : DynamicMetaObject
        {
            #region MethodInfos

            private static readonly MethodInfo _getMethod = Method(x => x.Get(""));
            private static readonly MethodInfo _setMethod = Method(x => x.Set("", new object()));
            private static readonly MethodInfo _unaryMethod = Method(x => x.Unary(ExpressionType.Call));
            private static readonly MethodInfo _binaryMethod = Method(x => x.Binary(ExpressionType.Call, new object()));
            private static readonly MethodInfo _methodMethod = Method(x => x.Method("", new object()));
            private static readonly MethodInfo _getIndexMethod = Method(x => x.GetIndex(new object()));
            private static readonly MethodInfo _setIndexMethod = Method(x => x.SetIndex(new object[0], new object()));
            private static readonly MethodInfo _invokeMethod = Method(x => x.Invoke());

            private static MethodInfo Method(Expression<Func<DynamicEBuilder, DynamicEBuilder>> method)
            {
                return LambdaExtract.Method(method);
            }

            #endregion

            public MetaBuilder(Expression expression, DynamicEBuilder value)
                : base(expression, BindingRestrictions.Empty, value)
            {
            }

            #region System stuff

            private Expression LimitSelf
            {
                get { return Expression.Convert(LimitType); }
            }

            private DynamicMetaObject Build(Expression expression)
            {
                // The binding restriction parameter pertains to the dynamic object whose
                // member is being accessed, not the dynamic object resulting from the member
                // access.  So as a general rule, just pass through the binding restrictions
                // for this DynamicMetaObject instance, which matches the dynamic object whose
                // member is being accessed.
                return new DynamicMetaObject(expression, BindingRestrictions.GetTypeRestriction(Expression, LimitType));
            }

            private Expression CallSelf(MethodInfo method, params Expression[] parameters)
            {
                return LimitSelf.Method(method, parameters);
            }

            private DynamicMetaObject BuildCallSelf(MethodInfo method, params Expression[] parameters)
            {
                return Build(CallSelf(method, parameters));
            }

            #endregion

            public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
            {
                return BuildCallSelf(_getMethod, Expression.Constant(binder.Name));
            }

            public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
            {
                return BuildCallSelf(_setMethod, Expression.Constant(binder.Name), value.Expression.Convert(typeof(object)));
            }

            private static ExpressionType ConvertUnaryOperation(ExpressionType original)
            {
                switch (original)
                {
                    case ExpressionType.Decrement:
                        return ExpressionType.PreDecrementAssign;
                    case ExpressionType.Increment:
                        return ExpressionType.PreIncrementAssign;
                    default:
                        return original;
                }
            }

            public override DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder)
            {
                return BuildCallSelf(_unaryMethod, Expression.Constant(ConvertUnaryOperation(binder.Operation)));
            }

            public override DynamicMetaObject BindBinaryOperation(BinaryOperationBinder binder, DynamicMetaObject arg)
            {
                return BuildCallSelf(_binaryMethod, Expression.Constant(binder.Operation), arg.Expression.Convert(typeof(object)));
            }

            private static NewArrayExpression ToObjArray(DynamicMetaObject[] args)
            {
                return Expression.NewArrayInit(typeof(object), Array.ConvertAll(args, a => a.Expression.Convert(typeof(object))));
            }

            public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
            {
                return BuildCallSelf(_methodMethod, Expression.Constant(binder.Name), ToObjArray(args));
            }

            public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
            {
                return BuildCallSelf(_getIndexMethod, ToObjArray(indexes));
            }

            public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
            {
                return BuildCallSelf(_setIndexMethod, ToObjArray(indexes), value.Expression.Convert(typeof(object)));
            }

            public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
            {
                return BuildCallSelf(_invokeMethod, ToObjArray(args));
            }
        }

        #endregion
    }
}