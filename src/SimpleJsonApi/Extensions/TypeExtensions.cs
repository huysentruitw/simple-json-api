using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SimpleJsonApi.Extensions
{
    internal static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, Type> IEnumerableVariantCache = new ConcurrentDictionary<Type, Type>();
        private static readonly ConcurrentDictionary<Tuple<Type, Type>, bool> ImplementsCache = new ConcurrentDictionary<Tuple<Type, Type>, bool>(); 

        public static Type ToGenericIEnumerableVariant(this Type type)
            => IEnumerableVariantCache.GetOrAdd(type, x => typeof(IEnumerable<>).MakeGenericType(x));

        public static bool Implements(this Type type, Type otherType)
            => ImplementsCache.GetOrAdd(new Tuple<Type, Type>(type, otherType), x =>
                x.Item2.IsAssignableFrom(x.Item1) ||
                x.Item1.ImplementsInterface(x.Item2) ||
                x.Item1.ImplementsGeneric(x.Item2));

        public static Type GetFirstGenericArgument(this Type genericType)
            => genericType.IsGenericType ? genericType.GetGenericArguments().FirstOrDefault() : null;

        private static bool ImplementsInterface(this Type type, Type interfaceType)
            => interfaceType.IsInterface && type.GetInterface(interfaceType.FullName) != null;

        private static bool ImplementsGeneric(this Type type, Type genericType)
            => type.IsGenericType && genericType.IsGenericType && genericType.IsAssignableFrom(type.GetGenericTypeDefinition());
    }
}
