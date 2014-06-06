using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
        public static readonly Func<T, int> Instance = CreateLambda().Compile();

        internal static Expression<Func<T, int>> CreateLambda() {
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


                /*
thoughts here are: I'd like common cases (i.e. the first 50 or so fields) not to wrap in 64-bit arithmetic.
This allows me to use plain old normal math to ensure that the coefficients are relatively prime
 - of course I guess I could just use primes themselves...
Furthermore, I want coefficients to be relatively large, so that I'm using as much of my 64-bit entropy
as possible to encourage good mixing.  In practice this means I want my multiplicative factor to be not\
too far from 2^64/2^32/100 say, i.e. not too much smaller than 50 million.
                  
The below constants were chosen so that field coefficients are less than 2^32 but not that far
from it, and (almost entirely) relatively prime for the first 50 coefficients.
                 
                 */
                fieldIndex++;
                ulong scale = 1794967171ul /*prime*/
                + fieldIndex * 19399380ul /*2*2*3*5*7*11*13*17*19 */;
                var scaledFieldHashExpr = Expression.Multiply(ulongFieldHashExpr, Expression.Constant(scale));


                //if this field is null, use some arbitrary hash code.
                var nullSafeFieldHashExpr = fieldInfo.FieldType.IsValueType
                   ? (Expression)scaledFieldHashExpr
                   : Expression.Condition(
                        Expression.Equal(Expression.Default(typeof(object)),
                            Expression.Convert(fieldExpr, typeof(object))),
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
            return Expression.Lambda<Func<T, int>>(bodyExpr, paramExpr);
        }
    }
}
