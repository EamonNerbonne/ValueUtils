using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ValueUtils {

    /// <summary>
    /// Represents a C# object with value semantics.
    /// ValueObjects implements GetHashCode, Equals, IEquatable&lt;&gt; and operators == and != for you.
    /// 
    /// A class deriving from ValueObject should pass itself as
    /// the generic type parameter to ValueObject.  ValueObjects must be sealed.
    /// </summary>
    /// <typeparam name="T">The sealed subclass of ValueObject.</typeparam>
    public abstract class ValueObject<T> : IEquatable<T> where T : ValueObject<T> {

        //CRTP to encourage you to use the value type itself as the type parameter.
        //However, we cannot prevent the following: ClassA :  ValueObject<ClassA>
        //and ClassB : ValueObject<ClassA>
        //If you do that, then GetHashCode and Equals will crash with an invalid cast exception.

        static readonly Func<T, T, bool> equalsFunc = FieldwiseEquality<T>.Instance;
        static readonly Func<T, int> hashFunc = FieldwiseHasher<T>.Instance;

        static ValueObject() {
            if (!typeof(T).GetTypeInfo().IsSealed)
                throw new NotSupportedException("Value objects must be sealed.");
        }

        public sealed override bool Equals(object obj) { return obj is T && equalsFunc((T)this, (T)obj); }

        public bool Equals(T obj) { return (object)obj != null && equalsFunc((T)this, obj); }
        //optimization note: (object)obj == null is equivalent to ReferenceEquals(obj, null) but faster here.

        public sealed override int GetHashCode() { return hashFunc((T)this); }

        public static bool operator !=(ValueObject<T> a, T b) {
            // ReSharper disable once CSharpWarnings::CS0252
            return (object)a != b
                && ((object)a == null || (object)b == null || !equalsFunc((T)a, b)); 
        }

        public static bool operator ==(ValueObject<T> a, T b) {
            // ReSharper disable once CSharpWarnings::CS0252
            return (object)a == b
                || (object)a != null && (object)b != null && equalsFunc((T)a, b);
        }
    }
}
