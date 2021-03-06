﻿using System;
using System.Runtime.CompilerServices;
using SimplyFast.Cache;

namespace SimplyFast.Reflection.Internal
{
    internal static class TypeResolveCache
    {
        private static readonly ICache<string, Type> _resolveCache = CacheEx.ThreadSafe<string, Type>();

        /// <summary>
        ///     Cached Type.GetType or search in all assemblies in current AppDomain
        /// </summary>
        /// <param name="name">Assembly qualified type name or full type name</param>
        /// <returns>Type if found or null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type Resolve(string name)
        {
            return _resolveCache.GetOrAdd(name, ResolveImpl);
        }

        private static Type ResolveImpl(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null)
                return type;
            foreach (var assembly in AssemblyEx.GetAllAssemblies())
            {
                type = assembly.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }
    }
}