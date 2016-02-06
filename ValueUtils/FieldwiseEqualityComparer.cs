using System;
using System.Collections.Generic;

namespace ValueUtils
{
    /// <summary>
    /// Implements an fieldwise IEqualityComparer singleton.
    /// This comparer simply wraps FieldwiseHasher and FieldwiseEquality.
    /// </summary>
    public sealed class FieldwiseEqualityComparer<T> : IEqualityComparer<T>
    {
        public static readonly FieldwiseEqualityComparer<T> Instance = new FieldwiseEqualityComparer<T>();
        public bool Equals(T x, T y) => equalsDelegate(x, y);
        public int GetHashCode(T obj) => getHashCodeDelegate(obj);

        private FieldwiseEqualityComparer() { }
        readonly Func<T, T, bool> equalsDelegate = FieldwiseEquality<T>.Instance;
        readonly Func<T, int> getHashCodeDelegate = FieldwiseHasher<T>.Instance;
    }
}
