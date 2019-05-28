using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ValueUtils
{
    /// <summary>
    /// Implements field-wise equality via reflection-based code generation.
    /// </summary>
    public static class FieldwiseEquality
    {
        /// <summary>
        /// Checks whether two objects of the same type are field-wise equal.  Type resolution is done
        /// statically, which allows fast code (similar to hand-rolled performance).
        /// However, warning: if the objects are of compile-time type BaseType, but at runtime turn out
        /// to be SubClass, then only the fields of BaseType will be checked.
        /// This is simply a type-inference friendly wrapper around FieldwiseEquality&lt;&gt;.Instance
        /// </summary>
        /// <typeparam name="T">The type of the objects to compare.</typeparam>
        public static bool AreEqual<T>(T a, T b) => FieldwiseEquality<T>.Instance(a, b);
    }

    /// <summary>
    /// Implements field-wise equality via reflection-based code generation.
    /// </summary>
    public static class FieldwiseEquality<T>
    {
        /// <summary>
        /// Checks whether two objects of the same type are field-wise equal.  Type resolution is specified
        /// statically, which allows fast code (similar to hand-rolled performance).
        /// However, warning: if the objects are of compile-time type BaseType, but at runtime turn out
        /// to be SubClass, then only the fields of BaseType will be checked.
        /// </summary>
        public static readonly Func<T, T, bool> Instance = CreateLambda().Compile();

        static Expression<Func<T, T, bool>> CreateLambda()
        {
            //Get all fields including inherited fields
            var type = typeof(T);
            var fields = ReflectionHelper.GetAllFields(type);

            var aExpr = Expression.Parameter(type, "a");
            var bExpr = Expression.Parameter(type, "b");

            Expression eqExpr = Expression.Constant(true);

            foreach (var fieldInfo in fields) {
                var aFieldExpr = Expression.Field(aExpr, fieldInfo);
                var bFieldExpr = Expression.Field(bExpr, fieldInfo);
                var bestEqualityApproach =
                        EqualityByOperatorOrNull(aFieldExpr, bFieldExpr, fieldInfo)
                        ?? InstanceEqualsOrNull(aFieldExpr, bFieldExpr, fieldInfo)
                    ;

                eqExpr = Expression.AndAlso(eqExpr, bestEqualityApproach);
            }

            return Expression.Lambda<Func<T, T, bool>>(eqExpr, aExpr, bExpr);
        }

        static bool HasEqualityOperator(TypeInfo type) =>
            type.IsPrimitive
            || type.IsEnum
            || type.GetMethod("op_Equality", BindingFlags.Public | BindingFlags.Static) != null
            || WhenNullableValueTypeGetNonNullableType(type) is Type nonNullableType
            && HasEqualityOperator(nonNullableType.GetTypeInfo());

        static Type WhenNullableValueTypeGetNonNullableType(TypeInfo type)
            => type.IsValueType && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? type.GetGenericArguments()[0]
                : null;

        static Expression EqualityByOperatorOrNull(Expression aFieldExpr, Expression bFieldExpr, FieldInfo fieldInfo) =>
            HasEqualityOperator(fieldInfo.FieldType.GetTypeInfo())
                ? Expression.Equal(aFieldExpr, bFieldExpr)
                : null;

        static Expression InstanceEqualsOrNull(Expression aFieldExpr, Expression bFieldExpr, FieldInfo fieldInfo)
        {
            var fieldType = fieldInfo.FieldType;
            var equalsMethod = fieldType.GetTypeInfo().GetMethod(
                "Equals", new[] { fieldType }
            );

            var fieldsEqualExpr = equalsMethod == null || equalsMethod.GetParameters()[0].ParameterType != fieldType
                    ? Expression.Call(((Func<object, object, bool>)Equals).GetMethodInfo(),
                        Expression.Convert(aFieldExpr, typeof(object)),
                        Expression.Convert(bFieldExpr, typeof(object))
                    )
                    : Expression.Call(aFieldExpr, equalsMethod, bFieldExpr)
                ;
            // TODO: optimization possibility: if an object needs object.Equals, and it has *not* overridden
            // which is quite likely if you need it in the first place, then I just just use a reference equality for
            // reference types and a by-field equality for structs.

            var nullSafeFieldsEqualExpr = fieldInfo.FieldType.GetTypeInfo().IsValueType || equalsMethod == null
                ? (Expression)fieldsEqualExpr
                : Expression.Condition(
                    Expression.Equal(Expression.Default(fieldInfo.FieldType), aFieldExpr),
                    Expression.Equal(Expression.Default(fieldInfo.FieldType), aFieldExpr),
                    fieldsEqualExpr
                );
            return nullSafeFieldsEqualExpr;
        }
    }
}
