﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueUtilsTest {
    class SampleClass {
        public SampleEnum AnEnum;
        public SampleClass SelfReference;
        
        public string AutoPropWithPrivateBackingField { get; set; }
    }

}