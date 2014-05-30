using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueUtils;

namespace ValueUtilsBenchmark {

    public sealed class ComplicatedValueObject : ValueObject<ComplicatedValueObject> {
        public SeekOrigin AnEnum;
        public int A;
        public int? NullableInt;
        public int B;
        public string Label;
        public DateTime Time;
        public int C;
    }
    public struct ComplicatedStruct {
        public SeekOrigin AnEnum; 
        public int A;
        public int? NullableInt;
        public int B;
        public string Label;
        public DateTime Time;
        public int C;
    }
    public sealed class ComplicatedManual : IEquatable<ComplicatedManual> {
        public SeekOrigin AnEnum;
        public int A;
        public int? NullableInt;
        public int B;
        public string Label;
        public DateTime Time;
        public int C;


        public bool Equals(ComplicatedManual obj) {
            return obj != null
                && obj.AnEnum == AnEnum
                && obj.A == A
                && obj.NullableInt == NullableInt
                && obj.B == B
                && obj.Label == Label
                && obj.Time == Time
                && obj.C == C;
        }
        public override bool Equals(object obj) {
            return Equals(obj as ComplicatedManual);
        }
        public override int GetHashCode() {
            uint h = 2166136261;
            h = (h * 16777619) ^ (uint)AnEnum.GetHashCode();
            h = (h * 16777619) ^ (uint)A.GetHashCode();
            h = (h * 16777619) ^ (uint)NullableInt.GetHashCode();
            h = (h * 16777619) ^ (uint)B.GetHashCode();
            h = (h * 16777619) ^ (uint)(default(string) == Label ? -1 : Label.GetHashCode());
            h = (h * 16777619) ^ (uint)Time.GetHashCode();
            h = (h * 16777619) ^ (uint)C.GetHashCode();
            return (int)h;
        }

        public ComplicatedStruct ToStruct() {
            return new ComplicatedStruct {
                AnEnum = AnEnum,
                A = A,
                NullableInt = NullableInt,
                B = B,
                Label = Label,
                Time = Time,
                C = C,
            };
        }
        public ComplicatedValueObject ToValueObject() {
            return new ComplicatedValueObject {
                AnEnum = AnEnum,
                A = A,
                NullableInt = NullableInt,
                B = B,
                Label = Label,
                Time = Time,
                C = C,
            };
        }
        public Tuple<SeekOrigin, int, int?, int, string, DateTime, int> ToTuple() {
            return Tuple.Create(
                    AnEnum,
                    A,
                    NullableInt,
                    B,
                    Label,
                    Time,
                    C
                );
        }
    }
}
