using System;
using SimplyFast.Reflection.Internal.DelegateBuilders.Expressions;
#if EMIT
using System.Reflection.Emit;
using SimplyFast.Reflection.Emit;
#else
using System.Linq.Expressions;
#endif

namespace SimplyFast.Reflection.Internal.DelegateBuilders.Parameters
{
    internal class RetValMap
    {
        private readonly Type _delegateReturn;
        private readonly Type _methodReturn;

        public RetValMap(Type delegateReturn, Type methodReturn)
        {
            if (delegateReturn != typeof(void) && !delegateReturn.IsAssignableFrom(methodReturn))
                throw new Exception("Invalid return type.");

            _delegateReturn = delegateReturn;
            _methodReturn = methodReturn;
        }

#if EMIT

        public void EmitConvert(ILGenerator generator)
        {
            if (_methodReturn == _delegateReturn || _delegateReturn == typeof(void))
                return;
            if (_methodReturn == typeof (void))
                generator.Emit(OpCodes.Ldnull);
            else if (_methodReturn.IsValueType && !_delegateReturn.IsValueType)
                generator.EmitBox(_methodReturn);
        }
#else
        private static readonly Expression _void = Expression.Empty();


        private Expression _invoke;
        private ParameterExpression _retVal;
        public void Prepare(ExpressionBlockBuilder block, Expression invoke)
        {
            _invoke = invoke;
            if (_methodReturn == typeof(void) || _delegateReturn == typeof(void))
            {
                // no local variables for void
                block.Add(invoke);
                return;
            }
            var variable = Expression.Variable(_methodReturn);
            var assign = Expression.Assign(variable, invoke);
            block.AddVariable(variable);
            block.Add(assign);
            _retVal = variable;
        }

        public void ConvertReturn(ExpressionBlockBuilder block)
        {
            if (_methodReturn == typeof(void))
            {
                if (_delegateReturn != typeof(void))
                {
                    // add default(_delegateReturn)
                    block.Add(Expression.Default(_delegateReturn));
                }
                else if (block.Last != _invoke)
                {
                    // should we return void?
                    block.Add(_void);
                }
                return;
            }
            if (_delegateReturn == typeof(void))
            {
                block.Add(_void);
                return;
            }
            
            // try optimize local variable
            Expression result;
            var binary = block.Last as BinaryExpression;
            if (binary != null && binary.NodeType == ExpressionType.Assign && binary.Left == _retVal)
            {
                result = binary.Right;
                block.RemoveVariable(_retVal);
                block.RemoveLast();
            }
            else
            {
                result = _retVal;
            }
            block.Add(_methodReturn == _delegateReturn ? result : Expression.Convert(result, _delegateReturn));
        }
#endif
    }
}