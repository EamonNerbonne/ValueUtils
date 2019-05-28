using System;
using System.IO;
using ValueUtils;

namespace ValueUtilsBenchmark
{
    public sealed class ComplicatedValueObject : ValueObject<ComplicatedValueObject>
    {
        public SeekOrigin AnEnum;
        public int A;
        public int? NullableInt;
        public int B;
        public string Label;
        public DateTime Time;
        public int C;
    }

    public struct ComplicatedStruct
    {
        public SeekOrigin AnEnum;
        public int A;
        public int? NullableInt;
        public int B;
        public string Label;
        public DateTime Time;
        public int C;
    }

    public sealed class ComplicatedManual : IEquatable<ComplicatedManual>
    {
        public SeekOrigin AnEnum;
        public int A;
        public int? NullableInt;
        public int B;
        public string Label;
        public DateTime Time;
        public int C;

        public bool Equals(ComplicatedManual obj)
            => obj != null
                && obj.AnEnum == AnEnum
                && obj.A == A
                && obj.NullableInt == NullableInt
                && obj.B == B
                && obj.Label == Label
                && obj.Time == Time
                && obj.C == C;

        public override bool Equals(object obj) => Equals(obj as ComplicatedManual);

        public override int GetHashCode()
        {
            var h = 2166136261;
            h = h * 16777619 ^ (uint)AnEnum.GetHashCode();
            h = h * 16777619 ^ (uint)A.GetHashCode();
            h = h * 16777619 ^ (uint)NullableInt.GetHashCode();
            h = h * 16777619 ^ (uint)B.GetHashCode();
            h = h * 16777619 ^ (uint)(default == Label ? -1 : Label.GetHashCode());
            h = h * 16777619 ^ (uint)Time.GetHashCode();
            h = h * 16777619 ^ (uint)C.GetHashCode();
            return (int)h;
        }

        public ComplicatedStruct ToStruct()
            => new ComplicatedStruct {
                AnEnum = AnEnum,
                A = A,
                NullableInt = NullableInt,
                B = B,
                Label = Label,
                Time = Time,
                C = C,
            };

        public ComplicatedValueObject ToValueObject()
            => new ComplicatedValueObject {
                AnEnum = AnEnum,
                A = A,
                NullableInt = NullableInt,
                B = B,
                Label = Label,
                Time = Time,
                C = C,
            };

        public Tuple<SeekOrigin, int, int?, int, string, DateTime, int> ToTuple()
            => Tuple.Create(
                AnEnum,
                A,
                NullableInt,
                B,
                Label,
                Time,
                C
            );

        public (SeekOrigin AnEnum, int A, int? NullableInt, int B, string Label, DateTime Time, int C) ToCs7Tuple()
            => (AnEnum, A, NullableInt, B, Label, Time, C);
    }
}
