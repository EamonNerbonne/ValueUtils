using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressionToCodeLib;
using ValueUtils;
using Xunit;

namespace ValueUtilsTest
{
    class SampleBaseWithProtectedMembers
    {
        protected int SomeValue;
        public SampleBaseWithProtectedMembers(int val) { SomeValue = val; }
    }

    class SampleSubWithProtectedInherited : SampleBaseWithProtectedMembers
    {
        public SampleSubWithProtectedInherited(int val) : base(val) { }
    }

    public class ProtectedBaseMembers
    {
        [Fact]
        public void IdenticalValuesAreEqual()
        {
            var a = new SampleSubWithProtectedInherited(1);
            var b = new SampleSubWithProtectedInherited(1);
            PAssert.That(() =>
                FieldwiseEquality.AreEqual(a, b)
                && !ReferenceEquals(a, b)
                );
        }

        [Fact]
        public void IdenticalValuesHaveSameHash()
        {
            var a = new SampleSubWithProtectedInherited(1);
            var b = new SampleSubWithProtectedInherited(1);
            PAssert.That(() =>
                FieldwiseHasher.Hash(a) == FieldwiseHasher.Hash(b)
                && !ReferenceEquals(a, b)
                );
        }

        [Fact]
        public void NonIdenticalValuesAreNotEqual()
        {
            var a = new SampleSubWithProtectedInherited(1);
            var b = new SampleSubWithProtectedInherited(2);
            PAssert.That(() =>
                !FieldwiseEquality.AreEqual(a, b)
                && !ReferenceEquals(a, b)
                );
        }

        [Fact]
        public void NonIdenticalValuesHaveDifferentHash()
        {
            var a = new SampleSubWithProtectedInherited(1);
            var b = new SampleSubWithProtectedInherited(2);
            PAssert.That(() =>
                FieldwiseHasher.Hash(a) != FieldwiseHasher.Hash(b)
                && !ReferenceEquals(a, b)
                );
        }
    }
}
    
