using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ValueUtils {
    public static class FieldwiseHasher {
        /// <summary>
        /// Computes a hashcode for an object based on its fields.  Type resolution is done 
        /// statically, which allows fast code (similar to hand-rolled performance).
        /// However, warning: if the instance is of compile-time type BaseType, but at runtime turns out
        /// to be SubClass, then only the fields of BaseType will be checked.
        /// 
        /// This is simply a type-inference friendly wrapper around FieldwiseHasher&lt;&gt;.Instance
        /// </summary>
        /// <typeparam name="T">The type of the object to hash.</typeparam>
        public static int Hash<T>(T val) { return FieldwiseHasher<T>.Instance(val); }
    }

    public static class FieldwiseHasher<T> {
        /// <summary>
        /// Computes a hashcode for an object based on its fields.  Type resolution is specified 
        /// statically, which allows fast code (similar to hand-rolled performance).
        /// However, warning: if the instance is of compile-time type BaseType, but at runtime turns out
        /// to be SubClass, then only the fields of BaseType will be checked.
        /// </summary>
        public static readonly Func<T, int> Instance = Create();

        static Func<T, int> Create() {
            //Get all fields including inherited fields
            var fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            var paramExpr = Expression.Parameter(typeof(T), "valueToHash");
            //Strategy: accumulate a scaled hash code for each member

            //Start with some arbitrary constant; pick something type-dependant for the rare mixed-type usecase.
            Expression hashExpr = Expression.Constant((ulong)typeof(T).GetHashCode() * 1234567);
            ulong fieldIndex = 0;
            var getHashCodeMethod = ((Func<int>)(new object().GetHashCode)).Method;

            foreach (var fieldInfo in fields) {
                var fieldExpr = Expression.Field(paramExpr, fieldInfo);

                //use the field's hash code (pre-resolving overridden GetHashCode does not appear to be faster)
                var rawFieldHashExpr = Expression.Call(fieldExpr, getHashCodeMethod);

                //we want to scale/bit-rotate the 32bit hashcode, and the fastest way to do that on common
                //x64 systems is via ulong - but we don't want sign-extension, so first we cast to unsigned.
                var uintFieldHashExpr = Expression.Convert(rawFieldHashExpr, typeof(uint));
                var ulongFieldHashExpr = Expression.Convert(uintFieldHashExpr, typeof(ulong));

                // multiply by 1+2n
                // i.e. ensure that at least 2 bits of the lower 32 are set to encourage nice mixing.
                // we want the lowest bit always to be set to ensure that objects without much symmetry are
                // hashed well.  For more complex situations (i.e. pairs of identical values in one object or multiple
                // objects differing only in order etc.) we can never get a perfect situation (after all we need to
                // reduce to 32 bits), but addition loses less than XOR given repeats, and slightly different scaling
                // values means we aren't entirely insensitive to ordering.
                fieldIndex++;
                ulong scale = 1023 + 1022 * fieldIndex;
                var scaledFieldHashExpr = Expression.Multiply(ulongFieldHashExpr, Expression.Constant(scale));


                //if this field is null, use some arbitrary hash code.
                var nullSafeFieldHashExpr = fieldInfo.FieldType.IsValueType
                   ? (Expression)scaledFieldHashExpr
                   : Expression.Condition(
                        Expression.Equal(Expression.Default(typeof(object)), 
                            Expression.Convert(fieldExpr,typeof(object))),
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
    }
}
