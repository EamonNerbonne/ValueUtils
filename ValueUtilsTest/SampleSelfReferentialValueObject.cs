using System;
using System.Collections.Generic;
using System.Linq;
using ValueUtils;

namespace ValueUtilsTest {
    sealed class SampleSelfReferentialValueObject : ValueObject<SampleSelfReferentialValueObject> {
        public int Value;
        public SampleSelfReferentialValueObject SameTypeReference;
    }
}
