using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SimplyFast.Collections;
using SimplyFast.Reflection.Internal.DelegateBuilders.Maps;

namespace SimplyFast.Reflection.Internal.DelegateBuilders
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
            var pm = map.ParametersMap;
            var parameters = pm.ConvertAll(p => Expression.Parameter(p.Delegate.Type));
            var block = new ExpressionBlockBuilder();
            var invokeParameters = parameters.ConvertAll((p, i) => Prepare(pm[i], block, p));
            var invoke = buildInvoke(invokeParameters);
            var retVal = PrepareInvoke(map.RetValMap, block, invoke);
            for (var i = 0; i < parameters.Length; i++) 
                Finish(pm[i], block, invokeParameters[i], parameters[i]);
            ConvertReturn(map.RetValMap, block, invoke, retVal);

            var body = block.CreateExpression();
            var lambda = Expression.Lambda(map.DelegateType, body, parameters);
            return lambda.Compile();
        }

        private static Expression Prepare(ArgMap map, ExpressionBlockBuilder block, ParameterExpression p)
        {
            if (!map.Method.IsOut && !map.Method.IsByRef)
            {
                // normal parameter
                return map.Method.Type == map.Delegate.Type ? (Expression)p : Expression.Convert(p, map.Method.Type);
            }
            // out or ref
            var mt = map.Method.Type.RemoveByRef();
            var dt = map.Delegate.Type.RemoveByRef();
            if (mt == dt)
                return p;
            
            var localVariable = Expression.Variable(mt);
            block.AddVariable(localVariable);
            if (map.Method.IsOut)
                return localVariable;

            // ref
            var value = map.Delegate.IsOut
                ? (Expression)Expression.Default(mt)
                : Expression.Convert(p, mt);
            var assign = Expression.Assign(localVariable, value);
            block.Add(assign);
            
            return localVariable;
        }

        private static void Finish(ArgMap map, ExpressionBlockBuilder block, Expression prepare, ParameterExpression p)
        {
            // Not out and (Not By Ref or ([byRef] and not (delegate ref or delegate out)
            if (!map.Method.IsOut && (!map.Method.IsByRef || !(map.Delegate.IsByRef || map.Delegate.IsOut)))
                return;
            // out/ref and typeSame - do nothing
            var mt = map.Method.Type.RemoveByRef();
            var dt = map.Delegate.Type.RemoveByRef();
            if (mt == dt)
                return;

            // convert and assign back
            var convertResult = Expression.Convert(prepare, dt);
            var assign = Expression.Assign(p, convertResult);
            block.Add(assign);
        }

        private static ParameterExpression PrepareInvoke(RetValMap retValMap, ExpressionBlockBuilder block, Expression invoke)
        {
            if (retValMap.Method == typeof(void) || retValMap.Delegate == typeof(void))
            {
                // no local variables for void
                block.Add(invoke);
                return null;
            }
            var variable = Expression.Variable(retValMap.Method);
            var assign = Expression.Assign(variable, invoke);
            block.AddVariable(variable);
            block.Add(assign);
            return variable;
        }

        private static readonly Expression _void = Expression.Empty();
        private static void ConvertReturn(RetValMap retValMap, ExpressionBlockBuilder block, Expression invoke, ParameterExpression retVal)
        {
            if (retValMap.Method == typeof(void))
            {
                if (retValMap.Delegate != typeof(void))
                {
                    // add default(_delegateReturn)
                    block.Add(Expression.Default(retValMap.Delegate));
                }
                else if (block.Last != invoke)
                {
                    // should we return void?
                    block.Add(_void);
                }
                return;
            }
            if (retValMap.Delegate == typeof(void))
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
            block.Add(retValMap.Method == retValMap.Delegate ? result : Expression.Convert(result, retValMap.Delegate));
        }

        private class ExpressionBlockBuilder
        {
            private readonly List<Expression> _block = new List<Expression>();
            private List<ParameterExpression> _variables;

            public Expression Last
            {
                get
                {
                    var count = _block.Count;
                    return count != 0 ? _block[count - 1] : null;
                }
            }

            public void Add(Expression expression)
            {
                _block.Add(expression);
            }

            public void AddVariable(ParameterExpression variable)
            {
                if (_variables == null)
                    _variables = new List<ParameterExpression>();
                _variables.Add(variable);
            }

            public void RemoveVariable(ParameterExpression variable)
            {
                _variables?.Remove(variable);
            }

            public void RemoveLast()
            {
                var count = _block.Count;
                if (count != 0)
                    _block.RemoveAt(count - 1);
            }

            public Expression CreateExpression()
            {
                return _block.Count == 1 && (_variables == null || _variables.Count == 0)
                    ? _block[0]
                    : Expression.Block(_variables, _block);
            }
        }
    }
}