using System;

namespace SimplyFast.Reflection.Internal.DelegateBuilders.Maps
{
    public struct ArgMap
    {
        public readonly SimpleParameterInfo Delegate;
        public readonly SimpleParameterInfo Method;

        // Method | Delegate
        // =======================
        // Normal | Normal,Ref
        // Ref    | Normal,Ref,Out
        // Out    | Ref, Out

        public ArgMap(int index, SimpleParameterInfo @delegate, SimpleParameterInfo method) 
        {
            Delegate = @delegate;
            Method = method;
            var methodType = method.Type.RemoveByRef();
            var delegateType = @delegate.Type.RemoveByRef();


            if (method.IsOut || method.IsByRef)
            {
                // check underlying types are assignable
                if (!delegateType.IsAssignableFrom(methodType))
                    throw new ArgumentException("Invalid type for parameter " + index);
                
                if (method.IsOut)
                {
                    // check that delegate can accept method's out
                    if (!(@delegate.IsOut || @delegate.IsByRef))
                        throw new ArgumentException(
                            $"Invalid modifier for parameter {index}. Should be Out or Ref.");
                } 
            }
            else 
            {
                if (@delegate.IsOut)
                    throw new ArgumentException($"Invalid modifier for parameter {index}. Should be None or Ref.");

                // cast up or down should work
                if (!delegateType.IsAssignableFrom(methodType) && !methodType.IsAssignableFrom(delegateType))
                    throw new ArgumentException("Invalid type for parameter " + index);
            }

            
        }
    }
}