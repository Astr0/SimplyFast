using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SF.Reflection;

namespace SF.Expressions
{
    public static partial class ExpressionEx
    {
        #region Lambda shortcuts
        public static LambdaExpression Lambda(Func<ParameterExpression[], Expression> builder, params ParameterExpression[] parameters)
        {
            return Expression.Lambda(builder(parameters), parameters);
        }

        public static LambdaExpression Lambda(Func<ParameterExpression[], Expression> builder, params Type[] parameterTypes)
        {
            var parameters = System.Array.ConvertAll(parameterTypes, Expression.Parameter);
            return Expression.Lambda(builder(parameters), parameters);
        }

        public static LambdaExpression Lambda(Func<Expression> builder)
        {
            return Expression.Lambda(builder());
        }

        public static LambdaExpression Lambda(Type parameterType1, Func<ParameterExpression, Expression> builder)
        {
            var p1 = Expression.Parameter(parameterType1);
            return Expression.Lambda(builder(p1), p1);
        }

        public static LambdaExpression Lambda(Type parameterType1, Type parameterType2, Func<ParameterExpression, ParameterExpression, Expression> builder)
        {
            var p1 = Expression.Parameter(parameterType1);
            var p2 = Expression.Parameter(parameterType2);
            return Expression.Lambda(builder(p1, p2), p1, p2);
        }

        public static LambdaExpression Lambda(Type parameterType1, Type parameterType2, Type parameterType3, Func<ParameterExpression, ParameterExpression, ParameterExpression, Expression> builder)
        {
            var p1 = Expression.Parameter(parameterType1);
            var p2 = Expression.Parameter(parameterType2);
            var p3 = Expression.Parameter(parameterType3);
            return Expression.Lambda(builder(p1, p2, p3), p1, p2, p3);
        }

        public static LambdaExpression Lambda(Type parameterType1, Type parameterType2, Type parameterType3, Type parameterType4, Func<ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, Expression> builder)
        {
            var p1 = Expression.Parameter(parameterType1);
            var p2 = Expression.Parameter(parameterType2);
            var p3 = Expression.Parameter(parameterType3);
            var p4 = Expression.Parameter(parameterType4);
            return Expression.Lambda(builder(p1, p2, p3, p4), p1, p2, p3, p4);
        }

        #endregion

        #region Block shortcuts
        public static BlockExpression Block(Func<ParameterExpression[], Expression[]> builder, params ParameterExpression[] variable)
        {
            return Expression.Block(variable, builder(variable));
        }

        public static BlockExpression Block(Func<ParameterExpression[], Expression[]> builder, params Type[] variableTypes)
        {
            var variables = System.Array.ConvertAll(variableTypes, Expression.Variable);
            return Expression.Block(variables, builder(variables));
        }

        public static BlockExpression Block(Func<Expression[]> builder)
        {
            return Expression.Block(builder());
        }

        public static BlockExpression Block(Type variableType1, Func<ParameterExpression, Expression[]> builder)
        {
            var p1 = Expression.Variable(variableType1);
            return Expression.Block(new[] { p1 }, builder(p1));
        }

        public static BlockExpression Block(Type variableType1, Type variableType2, Func<ParameterExpression, ParameterExpression, Expression[]> builder)
        {
            var p1 = Expression.Variable(variableType1);
            var p2 = Expression.Variable(variableType2);
            return Expression.Block(new[] { p1, p2 }, builder(p1, p2));
        }

        public static BlockExpression Block(Type variableType1, Type variableType2, Type variableType3, Func<ParameterExpression, ParameterExpression, ParameterExpression, Expression[]> builder)
        {
            var p1 = Expression.Variable(variableType1);
            var p2 = Expression.Variable(variableType2);
            var p3 = Expression.Variable(variableType3);
            return Expression.Block(new[] { p1, p2, p3 }, builder(p1, p2, p3));
        }

        public static BlockExpression Block(Type variableType1, Type variableType2, Type variableType3, Type variableType4, Func<ParameterExpression, ParameterExpression, ParameterExpression, ParameterExpression, Expression[]> builder)
        {
            var p1 = Expression.Variable(variableType1);
            var p2 = Expression.Variable(variableType2);
            var p3 = Expression.Variable(variableType3);
            var p4 = Expression.Variable(variableType4);
            return Expression.Block(new[] { p1, p2, p3, p4 }, builder(p1, p2, p3, p4));
        }

        #endregion

        public static Expression Const<T>(T value)
        {
            return Expression.Constant(value, typeof (T));
        }

        public static Expression Convert(this Expression expression, Type type)
        {
            return expression.Type == type ? expression : Expression.Convert(expression, type);
        }

        public static Expression TypeAs(this Expression expression, Type type)
        {
            return expression.Type == type ? expression : Expression.TypeAs(expression, type);
        }

        public static Expression CastAs(this Expression expression, Type type)
        {
            if (expression.Type == type)
                return expression;
            if (type.IsValueType)
            {
                return expression.Type.IsValueType ? Expression.Convert(expression, type) : Expression.Unbox(expression, type);
            }
            return Expression.TypeAs(expression, type);
        }

        public static Expression Unary(this Expression expression, ExpressionType operation, Type type = null)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            return Expression.MakeUnary(operation, expression, type);
            // ReSharper restore AssignNullToNotNullAttribute
        }

        public static Expression Binary(this Expression expression, ExpressionType operation, Expression right)
        {
            return Expression.MakeBinary(operation, expression, right);
        }

        public static IndexExpression Array(this Expression expression, IEnumerable<Expression> indexes)
        {
            return Expression.ArrayAccess(expression, indexes);
        }

        public static IndexExpression Array(this Expression expression, params Expression[] indexes)
        {
            return Expression.ArrayAccess(expression, indexes);
        }

        public static IndexExpression Array(this Expression expression, Expression index)
        {
            return Expression.ArrayAccess(expression, index);
        }

        public static MemberExpression Field(this Expression expression, string name)
        {
            return Expression.Field(expression, name);
        }

        public static MemberExpression Field(this Expression expression, FieldInfo field)
        {
            return Expression.Field(expression, field);
        }

        public static MemberExpression Property(this Expression expression, string name)
        {
            return Expression.Property(expression, name);
        }

        public static MemberExpression Property(this Expression expression, PropertyInfo property)
        {
            return Expression.Property(expression, property);
        }

        public static IndexExpression Property(this Expression expression, PropertyInfo property, IEnumerable<Expression> arguments)
        {
            return Expression.Property(expression, property, arguments);
        }

        // ReSharper disable MethodOverloadWithOptionalParameter
        public static IndexExpression Property(this Expression expression, PropertyInfo property, params Expression[] arguments)
        // ReSharper restore MethodOverloadWithOptionalParameter
        {
            return Expression.Property(expression, property, arguments);
        }

        public static MemberExpression MemberAccess(this Expression expression, MemberInfo member)
        {
            return Expression.MakeMemberAccess(expression, member);
        }

        public static MemberExpression MemberAccess(this Expression expression, string member)
        {
            return Expression.PropertyOrField(expression, member);
        }

        public static BinaryExpression Assign(this Expression lvalue, Expression value)
        {
            return Expression.Assign(lvalue, value);
        }

        public static BinaryExpression AssignWithConvert(this Expression lvalue, Expression value)
        {
            return lvalue.Assign(value.Convert(lvalue.Type));
        }

        public static IndexExpression Index(Type type, params Expression[] indexers)
        {
            var property = type.Indexer(indexers.Select(x => x.Type).ToArray());
            if (property == null)
                throw new ArgumentException("Index Property not found.");
            return Expression.Property(null, property, indexers);
        }

        public static IndexExpression Index(this Expression expression, params Expression[] indexers)
        {
            if (expression.Type.IsArray)
                return Expression.ArrayAccess(expression, indexers);
            var property = expression.Type.Indexer(indexers.Select(x => x.Type).ToArray());
            if (property == null)
                throw new ArgumentException("Index Property not found.");
            return Expression.Property(expression, property, indexers);
        }

        public static Expression InvokeDelegate(this Expression expression, params Expression[] args)
        {
            return Expression.Invoke(expression, args);
        }

        public static Expression Method(this Expression expression, MethodInfo method, params Expression[] args)
        {
            return Expression.Call(expression, method, args);
        }

        public static Expression Method(Type type, string methodOrMemberName, params Expression[] arguments)
        {
            var method = type.BestMemberOverload(methodOrMemberName, arguments.Select(x => x.Type).ToArray());
            if (method == null)
                throw new ArgumentException("Method not found.", "methodOrMemberName");
            if (method.MemberType == MemberTypes.Method)
                return Expression.Call(null, (MethodInfo)method, arguments);
            return MemberAccess(null, method).InvokeDelegate(arguments);
        }

        public static Expression Method(this Expression expression, string methodOrMemberName, params Expression[] arguments)
        {
            var method = expression.Type.BestMemberOverload(methodOrMemberName, arguments.Select(x => x.Type).ToArray());
            if (method == null)
                throw new ArgumentException("Method not found.", "methodOrMemberName");
            if (method.MemberType == MemberTypes.Method)
                return Expression.Call(expression, (MethodInfo)method, arguments);
            return expression.MemberAccess(method).InvokeDelegate(arguments);
        }

        public static Expression New(Type type, params Expression[] arguments)
        {
            if (type.IsArray)
            {
                return Expression.NewArrayBounds(type.GetElementType(), arguments);
            }
            var constructor = type.Constructor(System.Array.ConvertAll(arguments, x => x.Type));
            if (constructor == null)
                throw new ArgumentException("Constructor not found.", "arguments");
            return Expression.New(constructor, arguments);
        }
    }
}