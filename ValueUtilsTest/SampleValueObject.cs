using ValueUtils;

namespace ValueUtilsTest
{
    sealed class SampleValueObject : ValueObject<SampleValueObject>
    {
        public int Value;
        public short ShortValue;
        public string StringValue;
    }
}
