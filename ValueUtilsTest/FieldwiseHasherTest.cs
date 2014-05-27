using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressionToCodeLib;
using ValueUtils;
using Xunit;

namespace ValueUtilsTest {
    public class FieldwiseHasherTest {
        struct SampleStruct {
            int value;
            public short shortvalue; //we want to check various underlying value types
            public readonly string hmm; //and at least one reference type member
            byte last; //also different combos of readonly/private/protected
            public SampleStruct(int a, short b, string c, byte d) {
                value = a;
                shortvalue = b;
                hmm = c;
                last = d;
            }
        }
        static readonly Func<SampleStruct, int> hash = FieldwiseHasher<SampleStruct>.Instance;

        [Fact]
        public void IdenticalValuesHaveIdenticalHashes() {
            PAssert.That(() =>
                hash(new SampleStruct(1, 2, "3", 4))
                == hash(new SampleStruct(1, 2, "3", 4)));
        }


        [Fact]
        public void IsNotJustAWrapperAroundGetHashCode() {
            PAssert.That(() =>
                hash(new SampleStruct(1, 2, "3", 4))
                != new SampleStruct(1, 2, "3", 4).GetHashCode());
        }

        [Fact]
        public void OneDifferentValueMemberChangesHash() {
            PAssert.That(() =>
                hash(new SampleStruct(1, 2, "3", 4))
                != hash(new SampleStruct(11, 2, "3", 4)));
            PAssert.That(() =>
                hash(new SampleStruct(1, 2, "3", 4))
                != hash(new SampleStruct(1, 12, "3", 4)));
            PAssert.That(() =>
                hash(new SampleStruct(1, 2, "3", 4))
                != hash(new SampleStruct(1, 2, "13", 4)));
            PAssert.That(() =>
                hash(new SampleStruct(1, 2, "3", 4))
                != hash(new SampleStruct(1, 2, "3", 14)));
        }

        [Fact]
        public void IdenticalObjectsHaveIdenticalHashes() {
            //it's important that this is a class not struct instance so we've checked that
            //also, that means we're accessing another assemblies private fields
            PAssert.That(() =>
                FieldwiseHasher.Get(Tuple.Create(1, 2, "3", 4))
                == FieldwiseHasher.Get(Tuple.Create(1, 2, "3", 4)));
        }

        [Fact]
        public void OneDifferentObjectMemberChangesHash() {
            PAssert.That(() =>
                FieldwiseHasher.Get(Tuple.Create(1, 2, "3", 4))
                != FieldwiseHasher.Get(Tuple.Create(11, 2, "3", 4)));
            PAssert.That(() =>
                FieldwiseHasher.Get(Tuple.Create(1, 2, "3", 4))
                != FieldwiseHasher.Get(Tuple.Create(1, 12, "3", 4)));
            PAssert.That(() =>
                FieldwiseHasher.Get(Tuple.Create(1, 2, "3", 4))
                != FieldwiseHasher.Get(Tuple.Create(1, 2, "13", 4)));
            PAssert.That(() =>
                FieldwiseHasher.Get(Tuple.Create(1, 2, "3", 4))
                != FieldwiseHasher.Get(Tuple.Create(1, 2, "3", 14)));
        }
    }
}
