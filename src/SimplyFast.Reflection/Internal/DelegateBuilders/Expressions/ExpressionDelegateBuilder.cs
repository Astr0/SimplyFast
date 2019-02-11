using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SimplyFast.Collections;
using SimplyFast.Reflection.Internal.DelegateBuilders.Parameters;

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
            var retVal = PrepareInvoke(map.RetValMap, block, invoke);
            for (var i = 0; i < parameters.Length; i++) map.ParametersMap[i].Finish(block, parameters[i]);
            ConvertReturn(map.RetValMap, block, invoke, retVal);

            var body = block.CreateExpression();
            var lambda = Expression.Lambda(map.DelegateType, body, parameters);
            return lambda.Compile();
        }

        private static ParameterExpression PrepareInvoke(RetValMap retValMap, ExpressionBlockBuilder block, Expression invoke)
        {
            if (retValMap.MethodReturn == typeof(void) || retValMap.DelegateReturn == typeof(void))
            {
                // no local variables for void
                block.Add(invoke);
                return null;
            }
            var variable = Expression.Variable(retValMap.MethodReturn);
            var assign = Expression.Assign(variable, invoke);
            block.AddVariable(variable);
            block.Add(assign);
            return variable;
        }

        private static readonly Expression _void = Expression.Empty();
        private static void ConvertReturn(RetValMap retValMap, ExpressionBlockBuilder block, Expression invoke, ParameterExpression retVal)
        {
            if (retValMap.MethodReturn == typeof(void))
            {
                if (retValMap.DelegateReturn != typeof(void))
                {
                    // add default(_delegateReturn)
                    block.Add(Expression.Default(retValMap.DelegateReturn));
                }
                else if (block.Last != invoke)
                {
                    // should we return void?
                    block.Add(_void);
                }
                return;
            }
            if (retValMap.DelegateReturn == typeof(void))
            {
                block.Add(_void);
                return;
            }
            
            // try optimize local variable
            Expression result;
            if (block.Last is BinaryExpression binary && binary.NodeType == ExpressionType.Assign && binary.Left == retVal)
            {
                result = binary.Right;
                block.RemoveVariable(retVal);
                block.RemoveLast();
            }
            else
            {
                result = retVal;
            }
            block.Add(retValMap.MethodReturn == retValMap.DelegateReturn ? result : Expression.Convert(result, retValMap.DelegateReturn));
        }
    }
}