using System.Collections.Generic;
using ExpressionToCodeLib;
using ValueUtils;
using Xunit;

namespace ValueUtilsTest
{
    public sealed class FieldwiseEqualityComparerTest
    {
        static readonly IEqualityComparer<SampleStruct> eqStruct = FieldwiseEqualityComparer<SampleStruct>.Instance;
        static readonly IEqualityComparer<SampleClass> eqClass = FieldwiseEqualityComparer<SampleClass>.Instance;

        [Fact]
        public void IdenticalValuesAreEqual()
            => PAssert.That(() => eqStruct.Equals(new SampleStruct(1, 2, "3", 4), new SampleStruct(1, 2, "3", 4)));

        [Fact]
        public void IdenticalValuesHaveEqualHashCode()
            // ReSharper disable once EqualExpressionComparison
            => PAssert.That(() => eqStruct.GetHashCode(new SampleStruct(1, 2, "3", 4)) == eqStruct.GetHashCode(new SampleStruct(1, 2, "3", 4)));

        [Fact]
        public void IdenticallyValuedClassesHaveEqualHashCodeAndAreEqual()
            // ReSharper disable once EqualExpressionComparison
        {
            var objA = new SampleClass { AnEnum = SampleEnum.P, NullableField = 4, PlainStruct = { Bla = 2 } };
            var objB = new SampleClass { AnEnum = SampleEnum.P, NullableField = 4, PlainStruct = { Bla = 2 } };
            PAssert.That(() => objA != objB && eqClass.Equals(objA, objB) && eqClass.GetHashCode(objA) == eqClass.GetHashCode(objB));
        }

        [Fact]
        public void OneChangedMemberCausesInequality()
        {
            PAssert.That(() => !eqStruct.Equals(new SampleStruct(1, 2, "3", 4), new SampleStruct(11, 2, "3", 4)));
            PAssert.That(() => !eqStruct.Equals(new SampleStruct(1, 2, "3", 4), new SampleStruct(1, 12, "3", 4)));
            PAssert.That(() => !eqStruct.Equals(new SampleStruct(1, 2, "3", 4), new SampleStruct(1, 2, "13", 4)));
            PAssert.That(() => !eqStruct.Equals(new SampleStruct(1, 2, "3", 4), new SampleStruct(1, 2, "3", 14)));
        }

        [Fact]
        public void OneChangedMemberCausesHashCodeInequality()
        {
            PAssert.That(() => eqStruct.GetHashCode(new SampleStruct(1, 2, "3", 4)) != eqStruct.GetHashCode(new SampleStruct(11, 2, "3", 4)));
            PAssert.That(() => eqStruct.GetHashCode(new SampleStruct(1, 2, "3", 4)) != eqStruct.GetHashCode(new SampleStruct(1, 12, "3", 4)));
            PAssert.That(() => eqStruct.GetHashCode(new SampleStruct(1, 2, "3", 4)) != eqStruct.GetHashCode(new SampleStruct(1, 2, "13", 4)));
            PAssert.That(() => eqStruct.GetHashCode(new SampleStruct(1, 2, "3", 4)) != eqStruct.GetHashCode(new SampleStruct(1, 2, "3", 14)));
        }


        [Fact]
        public void CanCheckEqualityWithNull()
        {
            PAssert.That(() => !eqClass.Equals(new SampleClass(), null));
            PAssert.That(() => !eqClass.Equals(null, new SampleClass()));
        }
    }
}
