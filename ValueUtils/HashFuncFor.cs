using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ValueUtils {
    public static class HashFuncFor<T> {
        public static readonly Func<T, int> Instance = Create();

        static Func<T, int> Create() {
            //Get all fields including inherited fields
            var fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            var paramExpr = Expression.Parameter(typeof(T), "valueToHash");
            //Strategy: accumulate a scaled hash code for each member

            //Start with some arbitrary constant; pick something type-dependant for the rare mixed-type usecase.
            Expression hashExpr = Expression.Constant((ulong)typeof(T).GetHashCode() * 1234567);
            ulong fieldIndex = 0;
            foreach (var fieldInfo in fields) {
                var fieldExpr = Expression.Field(paramExpr, fieldInfo);

                //use the overridden hash code function directly if available to avoid virtual method lookup
                var rawFieldHashExpr = Expression.Call(fieldExpr, OverriddenHashCodeMethod(fieldInfo.FieldType));

                //we want to scale/bit-rotate the 32bit hashcode, and the fastest way to do that
                //on common x64 systems is via ulong - but we don't want sign-extension, so first we cast to unsigned.
                var uintFieldHashExpr = Expression.Convert(rawFieldHashExpr, typeof(uint))
                var ulongFieldHashExpr = Expression.Convert(uintFieldHashExpr, typeof(ulong));

                // multiply by 1+2n
                // i.e. ensure that at least 2 bits of the lower 32 are set to encourage nice mixing.
                // we want the lowest bit always to be set to ensure that objects without much symmetry are hashed
                // well.  For more complex situations (i.e. pairs of identical values in one object or multiple objects
                // differing only in order etc.) we can never get a perfect situation (after all we need to reduce to 
                // 32 bits), but addition loses less than XOR given repeats, and slightly different scaling values
                // means we aren't entirely insensitive to ordering.
                fieldIndex++;
                ulong scale = 1 + 2 * fieldIndex;
                var scaledFieldHashExpr = Expression.Multiply(ulongFieldHashExpr, Expression.Constant(scale));


                //if this field is null, use some arbitrary hash code.
                var nullSafeFieldHashExpr = fieldInfo.FieldType.IsValueType
                   ? (Expression)scaledFieldHashExpr
                   : Expression.Condition(
                      Expression.Equal(Expression.Default(fieldInfo.FieldType), fieldExpr),
                         Expression.Constant(scale * 3456789ul),
                         scaledFieldHashExpr
                   );

                //add this field to running total.  Note we don't need an explicit accumulation variable!
                hashExpr = Expression.Add(hashExpr, nullSafeFieldHashExpr);
            }

            //we have a 64-bit expression, need to downmix to 32bit.
            var ulongHashVariable = Expression.Variable(typeof(ulong), "hashcode");
            var writeHashStatement = Expression.Assign(ulongHashVariable, hashExpr);

            var intHashExpr =
               Expression.Convert(
                  Expression.ExclusiveOr(
                     ulongHashVariable,
                     Expression.RightShift(ulongHashVariable, Expression.Constant(32))
                  )
               , typeof(int)
               );

            var bodyExpr = Expression.Block(new[] { ulongHashVariable, }, writeHashStatement, intHashExpr);
            var funcExpr = Expression.Lambda<Func<T, int>>(bodyExpr, paramExpr);

            return funcExpr.Compile();
        }

        static MethodInfo OverriddenHashCodeMethod(Type type) {
            var getHashCodeMethod = ((Func<int>)(new object().GetHashCode)).Method;
            var mi = type.GetMethod("GetHashCode", BindingFlags.Public | BindingFlags.Instance);
            return mi != null && mi.GetBaseDefinition() == getHashCodeMethod ? mi : getHashCodeMethod;
        }
    }
}
