using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressionToCodeLib;
using ValueUtils;
using Xunit;

namespace ValueUtilsTest {
    class FieldwiseHasherTest {
        struct SampleStruct {
            public int value;
            public short shortvalue;
            public readonly string hmm;
            byte last;
            public SampleStruct(int a, short b, string c, byte d) {
                value = a;
                shortvalue = b;
                hmm = c;
                last = d;
            }
        }
        static readonly Func<SampleStruct, int> hash = FieldwiseHasher<SampleStruct>.Instance;

        [Fact]
        public void IdenticalStructsWork() {
            PAssert.That(() =>
                hash(new SampleStruct(1, 2, "3", 4))
                == hash(new SampleStruct(1, 2, "3", 4)));
        }

        [Fact]
        public void ChangingMembersHaveDifferentHashCodes() {
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
        public void HashFuncWorksForExternalClasses() {

            PAssert.That(() =>
                hash(new SampleStruct(1, 2, "3", 4))
                !=hash( new SampleStruct(11, 2, "3", 4)));
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

    }

    
}
