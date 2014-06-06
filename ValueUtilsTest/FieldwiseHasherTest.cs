// ReSharper disable UnusedMember.Global
// ReSharper disable EqualExpressionComparison
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressionToCodeLib;
using ValueUtils;
using ValueUtilsTest.Annotations;
using Xunit;

namespace ValueUtilsTest {
    public class FieldwiseHasherTest {

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
                FieldwiseHasher.Hash(Tuple.Create(1, 2, "3", 4))
                == FieldwiseHasher.Hash(Tuple.Create(1, 2, "3", 4)));
        }

        [Fact]
        public void OneDifferentObjectMemberChangesHash() {
            PAssert.That(() =>
                FieldwiseHasher.Hash(Tuple.Create(1, 2, "3", 4))
                != FieldwiseHasher.Hash(Tuple.Create(11, 2, "3", 4)));
            PAssert.That(() =>
                FieldwiseHasher.Hash(Tuple.Create(1, 2, "3", 4))
                != FieldwiseHasher.Hash(Tuple.Create(1, 12, "3", 4)));
            PAssert.That(() =>
                FieldwiseHasher.Hash(Tuple.Create(1, 2, "3", 4))
                != FieldwiseHasher.Hash(Tuple.Create(1, 2, "13", 4)));
            PAssert.That(() =>
                FieldwiseHasher.Hash(Tuple.Create(1, 2, "3", 4))
                != FieldwiseHasher.Hash(Tuple.Create(1, 2, "3", 14)));
        }

        [Fact]
        public void AutoPropsAffectHash() {
            PAssert.That(() =>
                FieldwiseHasher.Hash(new SampleClass { AutoPropWithPrivateBackingField = "x" })
                == FieldwiseHasher.Hash(new SampleClass { AutoPropWithPrivateBackingField = "x" }));
            PAssert.That(() =>
                FieldwiseHasher.Hash(new SampleClass { AutoPropWithPrivateBackingField = "x" })
                != FieldwiseHasher.Hash(new SampleClass { AutoPropWithPrivateBackingField = "y" }));
        }

        [Fact]
        public void TypeMattersAtCompileTime() {
            PAssert.That(() =>
                FieldwiseHasher.Hash(new SampleClass {  AnEnum = SampleEnum.Q })
                != FieldwiseHasher.Hash(new SampleSubClass { AnEnum = SampleEnum.Q }));
        }

        [Fact]
        public void TypeDoesNotMatterAtRunTime() {
            var hasher = FieldwiseHasher<SampleClass>.Instance;
            PAssert.That(() =>
                hasher(new SampleClass { AnEnum = SampleEnum.Q })
                == hasher(new SampleSubClass { AnEnum = SampleEnum.Q }));
        }
        
        [Fact]
        public void SubClassesCheckBaseClassFields() {
            PAssert.That(() =>
                FieldwiseHasher.Hash(new SampleSubClassWithFields { AnEnum = SampleEnum.Q })
                != FieldwiseHasher.Hash(new SampleSubClassWithFields { AnEnum = SampleEnum.P }));
            PAssert.That(() =>
                FieldwiseHasher.Hash(new SampleSubClassWithFields { AnEnum = SampleEnum.Q })
                == FieldwiseHasher.Hash(new SampleSubClassWithFields { AnEnum = SampleEnum.Q }));
        }

        [Fact]
        public void StructIntFieldsAffectHash()
        {
            PAssert.That(() =>
                FieldwiseHasher.Hash(new CustomStruct { Bla = 1 })
                != FieldwiseHasher.Hash(new CustomStruct { Bla = 2 }));
            PAssert.That(() =>
                FieldwiseHasher.Hash(new CustomStruct { Bla = 3 })
                == FieldwiseHasher.Hash(new CustomStruct { Bla = 3 }));
        }

        [Fact]
        public void ClassNullableIntFieldsAffectHash() {
            PAssert.That(() =>
                FieldwiseHasher.Hash(new SampleClass { NullableField = null })
                != FieldwiseHasher.Hash(new SampleClass { NullableField = 1 }));
            PAssert.That(() =>
                FieldwiseHasher.Hash(new SampleClass { NullableField = 3 })
                == FieldwiseHasher.Hash(new SampleClass { NullableField = 3 }));
        }


        [Fact]
        public void ClassStructFieldsAffectHash() {
            var object1 = new SampleClass { PlainStruct = new CustomStruct { Bla = 1 } };
            var object2 = new SampleClass { PlainStruct = new CustomStruct { Bla = 2 } };
            var object3A = new SampleClass { PlainStruct = new CustomStruct { Bla = 3 } };
            var object3B = new SampleClass { PlainStruct = new CustomStruct { Bla = 3 } };
            PAssert.That(() => FieldwiseHasher.Hash(object1) != FieldwiseHasher.Hash(object2));
            PAssert.That(() => FieldwiseHasher.Hash(object3A) == FieldwiseHasher.Hash(object3B));
        }

        [Fact]
        public void ClassNullableStructFieldsAffectHash() {
            var object1 = new SampleClass { NullableStruct = null };
            var object2 = new SampleClass { NullableStruct = new CustomStruct { Bla = 2 } };
            var object3A = new SampleClass { NullableStruct = new CustomStruct { Bla = 3 } };
            var object3B = new SampleClass { NullableStruct = new CustomStruct { Bla = 3 } };
            PAssert.That(() => FieldwiseHasher.Hash(object1) != FieldwiseHasher.Hash(object2));
            PAssert.That(() => FieldwiseHasher.Hash(object3A) == FieldwiseHasher.Hash(object3B));
        }

    }
}
