using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueUtils {
    public abstract class ValueObject<T> : IEquatable<T> where T : ValueObject<T> {

        //CRTP to encourage you to use the value type itself as the type parameter.
        //However, we cannot prevent the following: ClassA :  ValueObject<ClassA>
        //and ClassB : ValueObject<ClassA>
        //If you do that, then GetHashCode and Equals will crash with an invalid cast exception.

        static readonly Func<T, T, bool> equalsFunc = FieldwiseEquality<T>.Instance;
        static readonly Func<T, int> hashFunc = FieldwiseHasher<T>.Instance;

        static ValueObject() {
            if (!typeof(T).IsSealed)
                throw new NotSupportedException("Value objects must be sealed.");
        }
        public override bool Equals(object obj) {
            return obj is T && equalsFunc((T)this, (T)obj);
        }
        public bool Equals(T obj) {
            return obj != null && equalsFunc((T)this, obj);
        }
        public override int GetHashCode() {
            return hashFunc((T)this);
        }
    }
}
