using System;
using ValueUtils;

namespace ValueUtilsBenchmark
{
    public sealed class IntPairValueObject : ValueObject<IntPairValueObject>
    {
        public int A;
        public int B;
    }

    public struct IntPairStruct
    {
        public int A;
        public int B;
    }

    public class IntPairManual : IEquatable<IntPairManual>
    {
        public int A;
        public int B;

        public bool Equals(IntPairManual obj)
            => obj != null
                && obj.A == A
                && obj.B == B;

        public override bool Equals(object obj) => Equals(obj as IntPairManual);

        public override int GetHashCode()
        {
            var h = 2166136261;
            h = h * 16777619 ^ (uint)A.GetHashCode();
            h = h * 16777619 ^ (uint)B.GetHashCode();
            return (int)h;
        }

        public IntPairStruct ToStruct()
            => new IntPairStruct {
                A = A,
                B = B,
            };

        public IntPairValueObject ToValueObject()
            => new IntPairValueObject {
                A = A,
                B = B,
            };

        public Tuple<int, int> ToTuple()
            => Tuple.Create(
                A,
                B
            );

        public (int A, int B) ToCs7Tuple()
            => (
                A,
                B
            );
    }
}
