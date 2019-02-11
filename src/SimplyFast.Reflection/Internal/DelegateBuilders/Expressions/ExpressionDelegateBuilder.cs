using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SimplyFast.Collections;

namespace SimplyFast.Reflection.Internal.DelegateBuilders.Expressions
{
    internal class ExpressionDelegateBuilder : IDelegateBuilder
    {
        public Delegate Constructor(ConstructorInfo constructor, Type delegateType)
        {
            return BuildDelegate(DelegateMap.Constructor(delegateType, constructor),
                parameters => Expression.New(constructor, parameters));
        }

        public Delegate Method(MethodInfo method, Type delegateType)
        {
            return BuildDelegate(DelegateMap.Method(delegateType, method), parameters =>
            {
                Expression instance;
                IEnumerable<Expression> invokeParams;
                if (method.IsStatic)
                {
                    instance = null;
                    invokeParams = parameters;
                }
                else
                {
                    instance = parameters[0];
                    invokeParams = parameters.Skip(1);
                }

                return Expression.Call(instance, method, invokeParams);
            });
        }

        public Delegate FieldGet(FieldInfo field, Type delegateType)
        {
            return BuildDelegate(DelegateMap.FieldGet(delegateType, field),
                parameters => Expression.Field(parameters.Length != 0 ? parameters[0] : null, field));
        }

        public Delegate FieldSet(FieldInfo field, Type delegateType)
        {
            return BuildDelegate(DelegateMap.FieldSet(delegateType, field),
                parameters =>
                {
                    var instance = parameters.Length > 1 ? parameters[0] : null;
                    var value = parameters.Length > 1 ? parameters[1] : parameters[0];
                    var fieldExpr = Expression.Field(instance, field);
                    return Expression.Assign(fieldExpr, value);
                });
        }

        private static Delegate BuildDelegate(DelegateMap map, Func<Expression[], Expression> buildInvoke)
        {
            var parameters = map.ParametersMap.ConvertAll(p => Expression.Parameter(p.DelegateParameter.Type));
            var block = new ExpressionBlockBuilder();
            var invokeParameters = parameters
                .ConvertAll((p, i) => map.ParametersMap[i].Prepare(block, p));
            var invoke = buildInvoke(invokeParameters);
            map.RetValMap.Prepare(block, invoke);
            for (var i = 0; i < parameters.Length; i++) map.ParametersMap[i].Finish(block, parameters[i]);
            map.RetValMap.ConvertReturn(block);

            var body = block.CreateExpression();
            var lambda = Expression.Lambda(map.DelegateType, body, parameters);
            return lambda.Compile();
        }
    }
}