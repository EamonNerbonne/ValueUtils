using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueUtilsBenchmark {
    struct CollisionStats {
        public int DistinctHashCodes, DistinctValues;
        public double Rate { get { return (DistinctValues - DistinctHashCodes - 1) / (DistinctValues - 1.0); } }
    }
}
