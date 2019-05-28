namespace ValueUtilsBenchmark
{
    struct CollisionStats
    {
        public int DistinctHashCodes, DistinctValues;
        public double Rate => (DistinctValues - DistinctHashCodes - 1) / (DistinctValues - 1.0);
    }
}
