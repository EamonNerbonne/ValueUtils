using System.Linq;
using ExpressionToCodeLib;
using ValueUtils;
using Xunit;

namespace ValueUtilsTest
{
    public class GetAllFieldTheory
    {
        [Fact]
        public void SampleStructHasFourFields()
        {
            PAssert.That(() => ReflectionHelper.GetAllFields(typeof(SampleStruct))
                .OrderBy(f => f.MetadataToken)
                .Select(f => f.Name)
                .SequenceEqual(new[] { "value", "shortvalue", "hmm", "last" })
            );
        }

        [Fact]
        public void SampleClassHasFiveFields()
        {
            var expectedFields = new[] { typeof(SampleEnum), typeof(int?), typeof(CustomStruct), typeof(CustomStruct?), typeof(string) };
            PAssert.That(() =>
                ReflectionHelper.GetAllFields(typeof(SampleClass))
                    .OrderBy(f => f.MetadataToken)
                    .Select(f => f.FieldType)
                    .SequenceEqual(expectedFields)
            );
        }

        [Fact]
        public void SampleSubClassHasBaseFieldsThenOwnFields()
        {
            PAssert.That(() =>
                ReflectionHelper.GetAllFields(typeof(SampleSubClassWithFields)).Take(5)
                    .SequenceEqual(ReflectionHelper.GetAllFields(typeof(SampleClass)))
            );

            PAssert.That(() =>
                ReflectionHelper.GetAllFields(typeof(SampleSubClassWithFields)).Select(f => f.Name).Skip(5)
                    .SequenceEqual(new[] { "SubClassField" })
            );
        }

        [Fact]
        public void FieldsIncludeBaseProtectedFieldsExactlyOnce()
        {
            PAssert.That(() =>
                ReflectionHelper.GetAllFields(typeof(SampleSubWithProtectedInherited)).Select(f => f.Name)
                    .SequenceEqual(new[] { "SomeValue" })
            );
        }

        [Fact]
        public void FieldsIncludeBasePrivateFieldsExactlyOnce()
        {
            PAssert.That(() =>
                ReflectionHelper.GetAllFields(typeof(SampleSubWithPrivateInherited)).Select(f => f.Name)
                    .SequenceEqual(new[] { "SomeValue" })
            );
        }
    }
}
