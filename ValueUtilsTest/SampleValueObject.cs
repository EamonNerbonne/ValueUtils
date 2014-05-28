using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueUtils;

namespace ValueUtilsTest {
    sealed class SampleValueObject : ValueObject<SampleValueObject> {
        public int Value;
        public short ShortValue;
        public string StringValue;
    }
}
