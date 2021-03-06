﻿using System;
using ValueUtils;

namespace ValueUtilsBenchmark
{
    public sealed class NastyNestedValueObject : ValueObject<NastyNestedValueObject>
    {
        public NastyNestedValueObject Nested;
        public int A;
        public int B;
    }

    public class NastyNestedManual : IEquatable<NastyNestedManual>
    {
        public NastyNestedManual Nested;
        public int A;
        public int B;

        public bool Equals(NastyNestedManual obj)
            => obj != null
                && (obj.Nested == Nested || Nested != null && obj.Nested != null && Nested.Equals(obj.Nested))
                && obj.A == A
                && obj.B == B;

        public override bool Equals(object obj) => Equals(obj as NastyNestedManual);

        public override int GetHashCode()
        {
            var h = 2166136261;
            h = h * 16777619 ^ (uint)(default(NastyNestedManual) == Nested ? -1 : Nested.GetHashCode());
            h = h * 16777619 ^ (uint)A.GetHashCode();
            h = h * 16777619 ^ (uint)B.GetHashCode();
            return (int)h;
        }

        public NastyNestedValueObject ToValueObject()
            => new NastyNestedValueObject {
                Nested = Nested?.ToValueObject(),
                A = A,
                B = B,
            };

        public Tuple<object, int, int> ToTuple()
            => Tuple.Create(
                (object)Nested?.ToTuple(),
                A,
                B
            );

        public (object Nested, int A, int B) ToCs7Tuple()
            => (
                Nested: Nested?.ToCs7Tuple(),
                A,
                B
            );
    }
}
