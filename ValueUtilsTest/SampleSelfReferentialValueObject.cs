using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueUtils;

namespace ValueUtilsTest {
    sealed class SampleSelfReferentialValueObject : ValueObject<SampleSelfReferentialValueObject> {
        public int Value;
        public SampleSelfReferentialValueObject SameTypeReference;
    }
}
