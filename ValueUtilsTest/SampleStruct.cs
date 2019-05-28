namespace ValueUtilsTest
{
    struct SampleStruct
    {
        int value;
        public short shortvalue; //we want to check various underlying value types
        public readonly string hmm; //and at least one reference type member
        byte last; //also different combos of readonly/private/protected
        public SampleStruct(int a, short b, string c, byte d)
        {
            value = a;
            shortvalue = b;
            hmm = c;
            last = d;
        }
    }
}
