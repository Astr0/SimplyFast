using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SimplyFast.Expressions.Dynamic.Internal;

namespace SimplyFast.Expressions.Dynamic
{
    /// <summary>
    ///     Builder for control structures using dynamics
    /// </summary>
    public static class DynamicControlBuilder
    {
        #region Object to Expr stuff

        private static Func<T, Expression> ExprFunc<T>(Func<T, object> func)
        {
            return x => Expr(func(x));
        }

        private static Expression ExprNull(Func<object> value)
        {
            return value == null ? null : Expr(value());
        }

        private static Expression ExprNull(object value)
        {
            return value == null ? null : Expr(value);
        }

        private static Expression Expr(object value)
        {
            return DynamicEBuilder.ToExpression(value);
        }

        private static Expression[] Exprs(IEnumerable<object> values)
        {
            return DynamicEBuilder.ToExpressions(values);
        }

        #endregion

        /// <summary>
        ///     Builds lambda expression
        /// </summary>
        /// <param name="bodyBuilder">parameters => body</param>
        public static LambdaExpression Lambda(Func<dynamic, object> bodyBuilder)
        {
            var builder = new ParametersEBuilder(false);
            var body = Expr(bodyBuilder(builder));
            return Expression.Lambda(body, builder);
        }

        /// <summary>
        ///     Builds block expression
        /// </summary>
        /// <param name="blockBuilder">localVariables => body</param>
        public static BlockExpression Block(Func<dynamic, IEnumerable<object>> blockBuilder)
        {
            var variables = new ParametersEBuilder(true);
            var body = blockBuilder(variables);
            return Expression.Block(variables, Exprs(body));
        }

        /// <summary>
        ///     Builds if expression
        /// </summary>
        /// <param name="test">Test expression</param>
        /// <param name="ifTrue">Then action</param>
        /// <param name="ifFalse">Else action</param>
        public static DynamicEBuilder If(object test, object ifTrue, object ifFalse = null)
        {
            return EBuilder.If(Expr(test), Expr(ifTrue), ExprNull(ifFalse));
        }

        /// <summary>
        ///     Builds cast expression
        /// </summary>
        /// <param name="aexpr">Expression to case</param>
        /// <param name="type">Target type</param>
        public static DynamicEBuilder Cast(object aexpr, Type type)
        {
            return Expr(aexpr).CastAs(type);
        }

        /// <summary>
        ///     Builds convert expression
        /// </summary>
        /// <param name="aexp">Expression to convert</param>
        /// <param name="type">Target type</param>
        public static DynamicEBuilder Convert(object aexp, Type type)
        {
            return Expression.Convert(Expr(aexp), type);
        }

        /// <summary>
        ///     Builds Default Expression for type
        /// </summary>
        public static DynamicEBuilder Default(Type type)
        {
            return Expression.Default(type);
        }

        /// <summary>
        ///     Builds loop
        /// </summary>
        /// <param name="bodyBuilder">loop => loop body</param>
        public static Expression Loop(Func<ILoopControl, object> bodyBuilder)
        {
            return EBuilder.Loop(ExprFunc(bodyBuilder));
        }

        /// <summary>
        ///     Builds while loop
        /// </summary>
        /// <param name="test">() => Test Expression</param>
        /// <param name="bodyBuilder">loop => loop body</param>
        public static Expression While(Func<object> test, Func<ILoopControl, object> bodyBuilder)
        {
            return EBuilder.While(Expr(test()), ExprFunc(bodyBuilder));
        }

        /// <summary>
        ///     Builds for loop
        /// </summary>
        /// <param name="test">() => Test Expression</param>
        /// <param name="iterator">() => Iteration expression</param>
        /// <param name="bodyBuilder">loop => loop body</param>
        public static Expression For(Func<object> test, Func<object> iterator, Func<ILoopControl, object> bodyBuilder)
        {
            return EBuilder.For(ExprNull(test), ExprNull(iterator), ExprFunc(bodyBuilder));
        }

        /// <summary>
        ///     Builds foreach loop
        /// </summary>
        /// <param name="enumerable">() => Enumerable Expression</param>
        /// <param name="bodyBuilder">loop => loop body</param>
        public static Expression ForEach(Func<object> enumerable, Func<IForeachControl, object> bodyBuilder)
        {
            return EBuilder.ForEach(Expr(enumerable()), ExprFunc(bodyBuilder));
        }

        /// <summary>
        ///     Builds typed foreach loop
        /// </summary>
        /// <param name="type">Element type</param>
        /// <param name="enumerable">() => Enumerable Expression</param>
        /// <param name="bodyBuilder">loop => loop body</param>
        public static Expression ForEach(Type type, Func<object> enumerable, Func<IForeachControl, object> bodyBuilder)
        {
            return EBuilder.ForEach(type, Expr(enumerable()), ExprFunc(bodyBuilder));
        }

        /// <summary>
        ///     Builds using(disposable) body;
        /// </summary>
        /// <param name="disposable">IDisposable Expression</param>
        /// <param name="body">Body Expression</param>
        public static Expression Using(object disposable, object body)
        {
            return EBuilder.Using(Expr(disposable), Expr(body));
        }

        /// <summary>
        ///     Builds switch statement
        /// </summary>
        /// <param name="switchValue">switch test expression</param>
        /// <param name="cases">cases</param>
        /// <param name="defaultBody">default expression</param>
        public static SwitchExpression Switch(object switchValue, IEnumerable<SwitchCase> cases, object defaultBody = null)
        {
            return Expression.Switch(Expr(switchValue), ExprNull(defaultBody), null, cases);
        }

        /// <summary>
        ///     Builds switch case
        /// </summary>
        /// <param name="value">Case test value</param>
        /// <param name="body">Case body expression</param>
        public static SwitchCase Case(object value, object body)
        {
            return Expression.SwitchCase(Expr(body), Expr(value));
        }

        /// <summary>
        ///     Builds switch case with multiple values
        /// </summary>
        /// <param name="values">Case test values</param>
        /// <param name="body">Case body</param>
        public static SwitchCase Case(IEnumerable<object> values, object body)
        {
            return Expression.SwitchCase(Expr(body), Exprs(values));
        }
    }
}