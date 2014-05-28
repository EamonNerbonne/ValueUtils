using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressionToCodeLib;
using ValueUtils;
using Xunit;

namespace ValueUtilsTest {
    public class FieldwiseEqualityTest {

        static readonly Func<SampleStruct, SampleStruct, bool> eq = FieldwiseEquality<SampleStruct>.Instance;

        [Fact]
        public void IdenticalValuesHaveIdenticalHashes() {
            PAssert.That(() =>
                eq(new SampleStruct(1, 2, "3", 4), new SampleStruct(1, 2, "3", 4)));
        }


        [Fact]
        public void OneDifferentValueMemberChangesHash() {
            PAssert.That(() =>
                !eq(new SampleStruct(1, 2, "3", 4) , new SampleStruct(11, 2, "3", 4)));
            PAssert.That(() =>
                !eq(new SampleStruct(1, 2, "3", 4) , new SampleStruct(1, 12, "3", 4)));
            PAssert.That(() =>
                !eq(new SampleStruct(1, 2, "3", 4) , new SampleStruct(1, 2, "13", 4)));
            PAssert.That(() =>
                !eq(new SampleStruct(1, 2, "3", 4) , new SampleStruct(1, 2, "3", 14)));
        }

        [Fact]
        public void IdenticalObjectsHaveIdenticalHashes() {
            //it's important that this is a class not struct instance so we've checked that
            //also, that means we're accessing another assemblies private fields
            PAssert.That(() =>
                FieldwiseEquality.AreEqual(Tuple.Create(1, 2, "3", 4) , Tuple.Create(1, 2, "3", 4)));
        }

        [Fact]
        public void OneDifferentObjectMemberChangesHash() {
            PAssert.That(() =>
                !FieldwiseEquality.AreEqual(Tuple.Create(1, 2, "3", 4) , Tuple.Create(11, 2, "3", 4)));
            PAssert.That(() =>
                !FieldwiseEquality.AreEqual(Tuple.Create(1, 2, "3", 4) , Tuple.Create(1, 12, "3", 4)));
            PAssert.That(() =>
                !FieldwiseEquality.AreEqual(Tuple.Create(1, 2, "3", 4) , Tuple.Create(1, 2, "13", 4)));
            PAssert.That(() =>
                !FieldwiseEquality.AreEqual(Tuple.Create(1, 2, "3", 4) , Tuple.Create(1, 2, "3", 14)));
        }

        [Fact]
        public void AutoPropsAffectHash() {
            PAssert.That(() =>
                FieldwiseEquality.AreEqual(new SampleClass { AutoPropWithPrivateBackingField = "x" } , new SampleClass { AutoPropWithPrivateBackingField = "x" }));
            PAssert.That(() =>
                !FieldwiseEquality.AreEqual(new SampleClass { AutoPropWithPrivateBackingField = "x" } , new SampleClass { AutoPropWithPrivateBackingField = "y" }));
        }

        [Fact]
        public void TypeDoesNotMatterAtRuntime() {
            PAssert.That(() =>
                !FieldwiseEquality.AreEqual(new SampleClass {  AnEnum = SampleEnum.Q } , new SampleSubClass { AnEnum = SampleEnum.Q }));
        }

        [Fact]
        public void SubClassesCheckBaseClassFields() {
            PAssert.That(() =>
                !FieldwiseEquality.AreEqual(new SampleSubClassWithFields { AnEnum = SampleEnum.Q } , new SampleSubClassWithFields { AnEnum = SampleEnum.P }));
            PAssert.That(() =>
                FieldwiseEquality.AreEqual(new SampleSubClassWithFields { AnEnum = SampleEnum.Q } , new SampleSubClassWithFields { AnEnum = SampleEnum.Q }));
        }

    }
}
