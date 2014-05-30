using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueUtils;

namespace ValueUtilsBenchmark {

    public sealed class IntPairValueObject : ValueObject<IntPairValueObject> {
        public int A;
        public int B;
    }
    public struct IntPairStruct {
        public int A;
        public int B;
    }
    public class IntPairManual : IEquatable<IntPairManual> {
        public int A;
        public int B;


        public bool Equals(IntPairManual obj) {
            return obj != null
                && obj.A == A
                && obj.B == B;
        }
        public override bool Equals(object obj) {
            return Equals(obj as IntPairManual);
        }
        public override int GetHashCode() {
            uint h = 2166136261;
            h = (h * 16777619) ^ (uint)A.GetHashCode();
            h = (h * 16777619) ^ (uint)B.GetHashCode();
            return (int)h;
        }

        public IntPairStruct ToStruct() {
            return new IntPairStruct {
                A = A,
                B = B,
            };
        }
        public IntPairValueObject ToValueObject() {
            return new IntPairValueObject {
                A = A,
                B = B,
            };
        }
        public Tuple<int, int> ToTuple() {
            return Tuple.Create(
                    A,
                    B
                );
        }
    }
}
