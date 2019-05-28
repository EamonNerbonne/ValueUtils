using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ValueUtils
{
    static class ReflectionHelper
    {
        public static IEnumerable<Type> WalkMeaningfulInheritanceChain(Type type)
        {
            var info = type.GetTypeInfo();
            if (info.IsClass) {
                while (type != typeof(object)) {
                    yield return type;
                    type = type.GetTypeInfo().BaseType;
                }
            } else if (info.IsValueType) {
                yield return type;
            }
        }

        const BindingFlags OnlyDeclaredInstanceMembers =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        public static IEnumerable<FieldInfo> GetAllFields(Type type)
            => WalkMeaningfulInheritanceChain(type).Reverse()
                .SelectMany(t => t.GetTypeInfo().GetFields(OnlyDeclaredInstanceMembers));
    }
}