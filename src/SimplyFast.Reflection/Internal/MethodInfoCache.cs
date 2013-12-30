using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SF.Reflection
{
    internal class MethodInfoCache
    {
        private static readonly ConcurrentDictionary<Type, MethodInfoCache> _methodCache = new ConcurrentDictionary<Type, MethodInfoCache>();

        // ReSharper disable MemberHidesStaticFromOuterClass
        public readonly MethodInfo[] Methods;
        // ReSharper restore MemberHidesStaticFromOuterClass
        private readonly Dictionary<string, MethodInfo[]> _methods;

        private MethodInfoCache(IReflect type)
        {
            Methods = type.GetMethods(SimpleReflection.BindingFlags);
            _methods = Methods.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToArray(), StringComparer.Ordinal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfoCache ForType(Type type)
        {
            return _methodCache.GetOrAdd(type, t => new MethodInfoCache(t));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MethodInfo[] Get(string name)
        {
            MethodInfo[] methods;
            _methods.TryGetValue(name, out methods);
            return methods;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MethodInfo First(string name)
        {
            MethodInfo[] methods;
            return _methods.TryGetValue(name, out methods) ? methods[0] : null;
        }

        public MethodInfo Get(string name, Type[] arguments)
        {
            MethodInfo[] methods;
            return _methods.TryGetValue(name, out methods) ? FindMatch(methods, arguments) : null;
        }

        private static MethodInfo FindMatch(MethodInfo[] methods, Type[] arguments)
        {
            foreach (var methodInfo in methods)
            {
                var parameters = methodInfo.GetParameters();
                if (parameters.Length != arguments.Length)
                    continue;
                var match = true;
                for (var i = 0; i < arguments.Length; i++)
                {
                    if (arguments[i] == parameters[i].ParameterType)
                        continue;
                    match = false;
                    break;
                }
                if (match)
                    return methodInfo;
            }
            return null;
        }

        public MethodInfo GetSubstituted(string name, int genericArgCount, Type[] arguments)
        {
            if (genericArgCount == 0)
                return Get(name, arguments);

            MethodInfo[] methods;
            if (!_methods.TryGetValue(name, out methods))
                return null;
            foreach (var methodInfo in methods)
            {
                if (!methodInfo.IsGenericMethodDefinition)
                    continue;
                var args = methodInfo.GetGenericArguments();
                if (args.Length != genericArgCount)
                    continue;

                var methodParameters = methodInfo.GetParameters();
                if (methodParameters.Length != arguments.Length)
                    continue;

                var subFunc = Substitute.GetSubstitutionFunction(args);

                var matched = true;
                for (var i = 0; i < methodParameters.Length; i++)
                {
                    if (methodParameters[i].ParameterType == TypeEx.Substitute(arguments[i], subFunc))
                        continue;
                    matched = false;
                    break;
                }
                if (matched)
                    return methodInfo;
            }
            return null;
        }

        public MethodInfo GetGeneric(string name, Type[] genericArguments, Type[] arguments)
        {
            if (genericArguments == null || genericArguments.Length == 0)
                return Get(name, arguments);
            MethodInfo[] methods;
            if (!_methods.TryGetValue(name, out methods))
                return null;
            foreach (var generic in methods)
            {
                if (!generic.IsGenericMethodDefinition)
                    continue;
                if (generic.GetGenericArguments().Length != genericArguments.Length)
                    continue;
                try
                {
                    var method = generic.MakeGeneric(genericArguments);
                    if (method.GetParameters().Select(x => x.ParameterType).SequenceEqual(arguments))
                        return method;
                }
                catch (InvalidOperationException)
                {
                    // just enumerate to next
                }
            }
            // not found (
            return null;
        }

        private static Type[] MatchParameters(IList<ParameterInfo> genericParameters, IList<Type> parameters)
        {
            if (genericParameters.Count != parameters.Count)
                return null;
            var dictionary = new Dictionary<int, Type>(genericParameters.Count);
            for (var i = 0; i < parameters.Count; i++)
            {
                var generic = genericParameters[i];
                var param = parameters[i];
                if (param.IsGenericParameter)
                    return null;
                if (generic.ParameterType.IsByRef != param.IsByRef)
                    return null;
                if (generic.ParameterType.IsGenericParameter)
                {
                    Type genericType;
                    if (dictionary.TryGetValue(generic.ParameterType.GenericParameterPosition, out genericType))
                    {
                        if (genericType != param)
                            return null;
                    }
                    else
                    {
                        dictionary.Add(generic.ParameterType.GenericParameterPosition, param);
                    }
                }
                else
                {
                    if (generic.ParameterType != param)
                        return null;
                }
            }
            var result = new Type[dictionary.Count];
            foreach (var type in dictionary)
            {
                result[type.Key] = type.Value;
            }
            return result;
        }

        private static MethodInfo FindGenericMatch(MethodInfo[] methods, Type[] arguments)
        {
            MethodInfo result = null;
            foreach (var genericMethodInfo in methods)
            {
                if (!genericMethodInfo.IsGenericMethodDefinition)
                    continue;
                var types = MatchParameters(genericMethodInfo.GetParameters(), arguments);
                if (types == null)
                    continue;
                try
                {
                    var method = genericMethodInfo.MakeGeneric(types);
                    if (result != null)
                        return null;
                    result = method;
                }
                catch (InvalidOperationException)
                {
                    // Just enumerate to next error
                }
            }
            return result;
        }

        public MethodInfo FindMethod(string name, Type[] arguments)
        {
            MethodInfo[] methods;
            if (!_methods.TryGetValue(name, out methods))
                return null;

            return FindMatch(methods, arguments) ?? FindGenericMatch(methods, arguments);
        }
    }
}