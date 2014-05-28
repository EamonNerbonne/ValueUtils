using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressionToCodeLib;
using Xunit;

namespace ValueUtilsTest {
    public class ValueObjectTest {
        [Fact]
        public void IdenticalValuesAreEqual() {
            var a = new SampleValueObject { ShortValue = 2000, StringValue = "A", Value = -1 };
            var b = new SampleValueObject { ShortValue = 2000, StringValue = "A", Value = -1 };
            PAssert.That(() =>
                a.Equals(b)
                && a.GetHashCode() == b.GetHashCode()
                && !ReferenceEquals(a, b)
                );
        }

        [Fact]
        public void DifferentValuesAreEqual() {
            var a = new SampleValueObject { ShortValue = 2000, StringValue = "A", Value = -1 };
            var b = new SampleValueObject { ShortValue = 2000, StringValue = "a", Value = -1 };
            PAssert.That(() =>
                !a.Equals(b)
                && a.GetHashCode() != b.GetHashCode()
                && !ReferenceEquals(a, b)
                );
        }

        [Fact]
        public void NonCyclicalSelfReferentialTypesWork() {
            PAssert.That(() =>
                new SampleSelfReferentialValueObject {
                    SameTypeReference = new SampleSelfReferentialValueObject {
                        Value = 1
                    }
                }.Equals(new SampleSelfReferentialValueObject {
                    SameTypeReference = new SampleSelfReferentialValueObject {
                        Value = 1
                    }
                }));
            PAssert.That(() =>
                new SampleSelfReferentialValueObject {
                    SameTypeReference = new SampleSelfReferentialValueObject {
                        Value = 1
                    }
                }.GetHashCode()
                == new SampleSelfReferentialValueObject {
                    SameTypeReference = new SampleSelfReferentialValueObject {
                        Value = 1
                    }
                }.GetHashCode());
            PAssert.That(() =>
                !new SampleSelfReferentialValueObject {
                    SameTypeReference = new SampleSelfReferentialValueObject {
                        Value = 1
                    }
                }.Equals(new SampleSelfReferentialValueObject {
                    SameTypeReference = new SampleSelfReferentialValueObject {
                        Value = 2
                    }
                }));

            PAssert.That(() =>
                new SampleSelfReferentialValueObject {
                    SameTypeReference = new SampleSelfReferentialValueObject {
                        Value = 1
                    }
                }.GetHashCode()
                != new SampleSelfReferentialValueObject {
                    SameTypeReference = new SampleSelfReferentialValueObject {
                        Value = 2
                    }
                }.GetHashCode());
        }

    }
}
