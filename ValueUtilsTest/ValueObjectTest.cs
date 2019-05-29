using System;
using ExpressionToCodeLib;
using ValueUtils;
using Xunit;

namespace ValueUtilsTest
{
    public class ValueObjectTest
    {
        [Fact]
        public void IdenticalValuesAreEqual()
        {
            var a = new SampleValueObject { ShortValue = 2000, StringValue = "A", Value = -1 };
            var b = new SampleValueObject { ShortValue = 2000, StringValue = "A", Value = -1 };
            PAssert.That(() =>
                a.Equals(b)
                && a.Equals((object)b)
                && a == b
                && a.GetHashCode() == b.GetHashCode()
                && !ReferenceEquals(a, b)
            );
        }

        [Fact]
        public void DifferentValuesAreUnequal()
        {
            var a = new SampleValueObject { ShortValue = 2000, StringValue = "A", Value = -1 };
            // ReSharper disable once SuspiciousTypeConversion.Global
            PAssert.That(() => !a.Equals("test"));
        }

        [Fact]
        public void NullIsNotEqual()
        {
            var a = new SampleValueObject { ShortValue = 2000, StringValue = "A", Value = -1 };
            var typedNull = default(SampleValueObject);
            var untypedNull = default(object);
            // ReSharper disable once NegativeEqualityExpression
            PAssert.That(() =>
                !a.Equals(typedNull)
                && !a.Equals(untypedNull)
                && a != typedNull
                && !(a == typedNull)
                && typedNull != a
                // ReSharper disable once EqualExpressionComparison
                && !(a != a)
            );
        }

        [Fact]
        public void DifferentTypesAreUnequal()
        {
            var a = new SampleValueObject { ShortValue = 2000, StringValue = "A", Value = -1 };
            var b = new SampleValueObject { ShortValue = 2000, StringValue = "a", Value = -1 };
            PAssert.That(() =>
                !a.Equals(b)
                && !a.Equals((object)b)
                && a != b
                && a.GetHashCode() != b.GetHashCode()
                && !ReferenceEquals(a, b)
            );
        }

        [Fact]
        public void CanCompareWithNull()
        {
            var a = new SampleValueObject { ShortValue = 2000, StringValue = "A", Value = -1 };
            SampleValueObject b = null;
            PAssert.That(() =>
                b == null && null == b && a != b
            );
        }


        [Fact]
        public void NonCyclicalSelfReferentialTypesWork()
        {
            var valueObjectA = new SampleSelfReferentialValueObject {
                Value = 1,
                SameTypeReference = new SampleSelfReferentialValueObject {
                    Value = 2
                }
            };
            var valueObjectB = new SampleSelfReferentialValueObject {
                Value = 1,
                SameTypeReference = new SampleSelfReferentialValueObject {
                    Value = 2
                }
            };
            var valueObjectX = new SampleSelfReferentialValueObject {
                Value = 1,
                SameTypeReference = new SampleSelfReferentialValueObject {
                    Value = 3
                }
            };

            PAssert.That(() =>
                valueObjectA.Equals(valueObjectB)
                && valueObjectA.GetHashCode() == valueObjectB.GetHashCode()
                && valueObjectA == valueObjectB
            );
            PAssert.That(() =>
                !valueObjectA.Equals(valueObjectX)
                && valueObjectA.GetHashCode() != valueObjectX.GetHashCode()
                && valueObjectA != valueObjectX
            );
        }

        public class MyOverridableClass : ValueObject<MyOverridableClass>
        {
            public int SomeContents;
        }

        [Fact]
        public void NonSealedValueObjectsCannotBeInstantiated()
            => Assert.ThrowsAny<Exception>(() => new MyOverridableClass());

        class NotQuiteTheSampleValueObject : ValueObject<SampleValueObject> { }

        [Fact]
        public void CrtpViolatingTypesCannotBeInstantiated()
        {
            Assert.ThrowsAny<Exception>(() => new NotQuiteTheSampleValueObject()
            );
        }
    }
}
