using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueUtilsTest {
    struct CustomStruct {
        public int Bla;
    }

    class SampleClass {
        public SampleEnum AnEnum;
        public int? NullableField;
        public CustomStruct PlainStruct;
        public CustomStruct? NullableStruct;

        public string AutoPropWithPrivateBackingField { get; set; }
    }

}
