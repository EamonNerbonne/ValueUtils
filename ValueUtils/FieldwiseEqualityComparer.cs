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
        //The downside of wrapping is that a consumer effective will pay the price of two 
        //interface calls: once to e.g. IEqualityComparer.Equals and once to Func<...>.Invoke
        //It's not a huge issue, and a smart JIT might inline those, but I'm not holding my breath.

        public static readonly FieldwiseEqualityComparer<T> Instance = new FieldwiseEqualityComparer<T>();
        public bool Equals(T x, T y) => equalsDelegate(x, y);
        public int GetHashCode(T obj) => getHashCodeDelegate(obj);

        private FieldwiseEqualityComparer() { }
        readonly Func<T, T, bool> equalsDelegate = FieldwiseEquality<T>.Instance;
        readonly Func<T, int> getHashCodeDelegate = FieldwiseHasher<T>.Instance;
    }
}
