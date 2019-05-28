using ValueUtils;

namespace ValueUtilsTest
{
    sealed class SampleSelfReferentialValueObject : ValueObject<SampleSelfReferentialValueObject>
    {
        public int Value;
        public SampleSelfReferentialValueObject SameTypeReference;
    }
}
