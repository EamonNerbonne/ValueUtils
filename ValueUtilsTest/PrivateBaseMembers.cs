using ExpressionToCodeLib;
using ValueUtils;
using Xunit;

namespace ValueUtilsTest
{
    class SampleBaseWithPrivateMembers
    {
        int SomeValue;
        public SampleBaseWithPrivateMembers(int val) => SomeValue = val;
    }

    class SampleSubWithPrivateInherited : SampleBaseWithPrivateMembers
    {
        public SampleSubWithPrivateInherited(int val) : base(val) { }
    }

    public class PrivateBaseMembers
    {
        [Fact]
        public void IdenticalValuesAreEqual()
        {
            var a = new SampleSubWithPrivateInherited(1);
            var b = new SampleSubWithPrivateInherited(1);
            PAssert.That(() =>
                FieldwiseEquality.AreEqual(a, b)
                && !ReferenceEquals(a, b)
                );
        }

        [Fact]
        public void IdenticalValuesHaveSameHash()
        {
            var a = new SampleSubWithPrivateInherited(1);
            var b = new SampleSubWithPrivateInherited(1);
            PAssert.That(() =>
                FieldwiseHasher.Hash(a) == FieldwiseHasher.Hash(b)
                && !ReferenceEquals(a, b)
                );
        }

        [Fact]
        public void NonIdenticalValuesAreNotEqual()
        {
            var a = new SampleSubWithPrivateInherited(1);
            var b = new SampleSubWithPrivateInherited(2);
            PAssert.That(() =>
                !FieldwiseEquality.AreEqual(a, b)
                && !ReferenceEquals(a, b)
                );
        }

        [Fact]
        public void NonIdenticalValuesHaveDifferentHash()
        {
            var a = new SampleSubWithPrivateInherited(1);
            var b = new SampleSubWithPrivateInherited(2);
            PAssert.That(() =>
                FieldwiseHasher.Hash(a) != FieldwiseHasher.Hash(b)
                && !ReferenceEquals(a, b)
                );
        }
    }
}

