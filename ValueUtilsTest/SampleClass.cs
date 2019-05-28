using JetBrains.Annotations;

namespace ValueUtilsTest
{
    struct CustomStruct
    {
        [UsedImplicitly]
        public int Bla;
    }

    class SampleClass
    {
        [UsedImplicitly]
        public SampleEnum AnEnum;

        [UsedImplicitly]
        public int? NullableField;

        [UsedImplicitly]
        public CustomStruct PlainStruct;

        [UsedImplicitly]
        public CustomStruct? NullableStruct;

        public string AutoPropWithPrivateBackingField
        {
            [UsedImplicitly]
            get;
            set;
        }
    }
}
