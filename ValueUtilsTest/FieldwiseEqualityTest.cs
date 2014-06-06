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
        public void IdenticalValuesAreEqual() {
            PAssert.That(() =>
                eq(new SampleStruct(1, 2, "3", 4), new SampleStruct(1, 2, "3", 4)));
        }

        [Fact(Skip = "Don't support nulls")]
        public void CanCheckEqualityWithNull() {
            PAssert.That(() =>
                !FieldwiseEquality.AreEqual(Tuple.Create(1), null));
        }


        [Fact]
        public void OneChangedMemberCausesInequality() {
            PAssert.That(() =>
                !eq(new SampleStruct(1, 2, "3", 4), new SampleStruct(11, 2, "3", 4)));
            PAssert.That(() =>
                !eq(new SampleStruct(1, 2, "3", 4), new SampleStruct(1, 12, "3", 4)));
            PAssert.That(() =>
                !eq(new SampleStruct(1, 2, "3", 4), new SampleStruct(1, 2, "13", 4)));
            PAssert.That(() =>
                !eq(new SampleStruct(1, 2, "3", 4), new SampleStruct(1, 2, "3", 14)));
        }

        [Fact]
        public void TuplesWithTheSameFieldValuesAreEqual() {
            //it's important that this is a class not struct instance so we've checked that
            //also, that means we're accessing another assemblies private fields
            PAssert.That(() =>
                FieldwiseEquality.AreEqual(Tuple.Create(1, 2, "3", 4), Tuple.Create(1, 2, "3", 4)));
        }

        [Fact]
        public void OneDifferentObjectMemberCausesInequality() {
            PAssert.That(() =>
                !FieldwiseEquality.AreEqual(Tuple.Create(1, 2, "3", 4), Tuple.Create(11, 2, "3", 4)));
            PAssert.That(() =>
                !FieldwiseEquality.AreEqual(Tuple.Create(1, 2, "3", 4), Tuple.Create(1, 12, "3", 4)));
            PAssert.That(() =>
                !FieldwiseEquality.AreEqual(Tuple.Create(1, 2, "3", 4), Tuple.Create(1, 2, "13", 4)));
            PAssert.That(() =>
                !FieldwiseEquality.AreEqual(Tuple.Create(1, 2, "3", 4), Tuple.Create(1, 2, "3", 14)));
        }

        [Fact]
        public void AutoPropsAffectEquality() {
            PAssert.That(() =>
                FieldwiseEquality.AreEqual(new SampleClass { AutoPropWithPrivateBackingField = "x" }, new SampleClass { AutoPropWithPrivateBackingField = "x" }));
            PAssert.That(() =>
                !FieldwiseEquality.AreEqual(new SampleClass { AutoPropWithPrivateBackingField = "x" }, new SampleClass { AutoPropWithPrivateBackingField = "y" }));
        }

        [Fact]
        public void StructFieldsAffectEquality() {
            PAssert.That(() =>
                FieldwiseEquality.AreEqual(new SampleClass { PlainStruct = { Bla = 1 } }, new SampleClass { PlainStruct = { Bla = 1 } }));
            PAssert.That(() =>
                !FieldwiseEquality.AreEqual(new SampleClass { PlainStruct = { Bla = 1 } }, new SampleClass { PlainStruct = { Bla = 2 } }));
        }

        [Fact]
        public void TypeDoesNotAffectRuntimeEquality() {
            var sampleClass = new SampleClass { AnEnum = SampleEnum.Q };
            var sampleSubClass = new SampleSubClass { AnEnum = SampleEnum.Q };

            //This is really pretty unwanted behavior
            PAssert.That(() => FieldwiseEquality.AreEqual(sampleClass, sampleSubClass));
        }

        [Fact]
        public void SubClassesVerifyEqualityOfBaseClassFields() {
            PAssert.That(() =>
                !FieldwiseEquality.AreEqual(new SampleSubClassWithFields { AnEnum = SampleEnum.Q }, new SampleSubClassWithFields { AnEnum = SampleEnum.P }));
            PAssert.That(() =>
                FieldwiseEquality.AreEqual(new SampleSubClassWithFields { AnEnum = SampleEnum.Q }, new SampleSubClassWithFields { AnEnum = SampleEnum.Q }));
        }

        [Fact]
        public void StructIntFieldsAffectEquality() {
            var customStruct1 = new CustomStruct { Bla = 1 };
            var customStruct2 = new CustomStruct { Bla = 2 };
            PAssert.That(() => !FieldwiseEquality.AreEqual(customStruct1, customStruct2));
            PAssert.That(() => FieldwiseEquality.AreEqual(new CustomStruct { Bla = 3 }, new CustomStruct { Bla = 3 }));
        }

    }
}
