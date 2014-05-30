using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressionToCodeLib;
using MoreLinq;

namespace ValueUtilsBenchmark {
    class Program {
        static void Main(string[] args) {
            BenchmarkComplicatedCases();
            Console.WriteLine();
            BenchmarkNastyNestedCases();
            Console.WriteLine();
            BenchmarkDuplicatePairCases();
            Console.WriteLine();
            BenchmarkSymmetricalCases();
            Console.WriteLine();
            BenchmarkIntPairCases();
        }

        static void BenchmarkNastyNestedCases() {
            Console.WriteLine("NastyNested:");
            var manuals = (
                from a in MoreEnumerable.Generate(-10000, a => a + 6).TakeWhile(a => a < 14100)
                from b0 in MoreEnumerable.Generate(int.MaxValue, c => (int)(c *0.95)).TakeWhile(c => c > 0)
                from sign in new[]{-1, 1}
                let b = sign*b0
                select new NastyNestedManual {
                    Nested = new NastyNestedManual {
                        A = b,
                        B = a,
                    },
                    A = a,
                    B = b,
                }
            ).ToArray();

            var valueObjects = manuals.Select(m => m.ToValueObject()).ToArray();
            var tuples = manuals.Select(m => m.ToTuple()).ToArray();

            ProcessList(manuals);
            ProcessList(valueObjects);
            ProcessList(tuples);
        }


        static void BenchmarkIntPairCases() {
            Console.WriteLine("IntPair:");
            var manuals = (
                from a in MoreEnumerable.Generate(-8000, a => a + 4).TakeWhile(a => a < 8000)
                from b0 in MoreEnumerable.Generate(int.MaxValue, c => (int)(c * 0.95)).TakeWhile(c => c > 0)
                from sign in new[] { -1, 1 }
                let b = sign * b0
                select new IntPairManual {
                    A = a,
                    B = b,
                }
            ).ToArray();

            var valueObjects = manuals.Select(m => m.ToValueObject()).ToArray();
            var tuples = manuals.Select(m => m.ToTuple()).ToArray();
            var structs = manuals.Select(m => m.ToStruct()).ToArray();

            ProcessList(manuals);
            ProcessList(valueObjects);
            ProcessList(tuples);
            ProcessList(structs);
        }


        static void BenchmarkSymmetricalCases() {
            Console.WriteLine("IntPair with Symmetry:");
            var manuals = (
                from a in MoreEnumerable.Generate(-5100, a => a + 5).TakeWhile(a => a < 5100)
                from b0 in MoreEnumerable.Generate(int.MaxValue, c => (int)(c * 0.95)).TakeWhile(c => c > 0)
                from sign in new[] { -1, 1 }
                let b = sign * b0
                from swap in new[] { true, false }
                select new IntPairManual {
                    A = swap ? a : b,
                    B = swap ? b : a,
                }
            ).Distinct().ToArray();

            var valueObjects = manuals.Select(m => m.ToValueObject()).ToArray();
            var tuples = manuals.Select(m => m.ToTuple()).ToArray();
            var structs = manuals.Select(m => m.ToStruct()).ToArray();

            ProcessList(manuals);
            ProcessList(valueObjects);
            ProcessList(tuples);
            ProcessList(structs);
        }

        static void BenchmarkDuplicatePairCases() {
            Console.WriteLine("IntPair with Duplication:");
            var manuals = (
                from a in Enumerable.Range(0, 3000000)
                select new IntPairManual {
                    A = a * 8,
                    B = a * 8,
                }
            ).Distinct().ToArray();

            var valueObjects = manuals.Select(m => m.ToValueObject()).ToArray();
            var tuples = manuals.Select(m => m.ToTuple()).ToArray();
            var structs = manuals.Select(m => m.ToStruct()).ToArray();

            ProcessList(manuals);
            ProcessList(valueObjects);
            ProcessList(tuples);
            ProcessList(structs);
        }

        static void BenchmarkComplicatedCases() {
            Console.WriteLine("Complicated:");
            var manuals = (
                from a in MoreEnumerable.Generate(-500, a => a + 4).TakeWhile(a => a < 500)
                from b0 in MoreEnumerable.Generate(int.MaxValue, b => (int)(b * 0.9)).TakeWhile(b => b > 0)
                from sign in new[] { -1, 1 }
                let b = sign * b0
                from c in MoreEnumerable.Generate(1, c => c << 1).TakeWhile(c => c > 0)
                select new ComplicatedManual {
                    AnEnum = (SeekOrigin)(c%3),
                    A = a,
                    B = b,
                    C = c,
                    NullableInt = a< b && b< c ? default(int?) : a&b&c,
                    Label = Math.Min(a, c).ToString(),
                    Time = new DateTime(2014, 1, 1) + TimeSpan.FromSeconds(a + c + Math.Log(b0))
                }
            ).ToArray();
            var valueObjects = manuals.Select(m => m.ToValueObject()).ToArray();
            var tuples = manuals.Select(m => m.ToTuple()).ToArray();
            var structs = manuals.Select(m => m.ToStruct()).ToArray();

            ProcessList(manuals);
            ProcessList(valueObjects);
            ProcessList(tuples);
            ProcessList(structs);
        }

        static void ProcessList<T>(T[] objs) {
            string name = ObjectToCode.GetCSharpFriendlyTypeName(typeof(T));
            AnalyzeHashQuality(objs);
            Benchmark(name + " GetHashCode()", () => {
                foreach (var inst in objs)
                    inst.GetHashCode();
            });
            Benchmark(name + " Equals()", () => {
                for (int i = 0; i < objs.Length; i++) {
                    objs[i].Equals(objs[i]);
                    objs[i].Equals(objs[(i + 1) % objs.Length]);
                }
            });
            if (CountDistinctHashes(objs) >= 500) //at only 500 hash values, we'll have lots and lots of hash collisions.
                Benchmark(name + " Distinct().Count()", () => {
                    objs.Distinct().Count();
                });
            else
                Console.WriteLine(name + " Distinct().Count(): <too slow to complete>");
        }
        static int CountDistinctHashes<T>(T[] objs) {
            return objs.Select(o => o.GetHashCode()).Distinct().Count();
        }
        static void AnalyzeHashQuality<T>(T[] objs) {
            int distinctHashes = CountDistinctHashes(objs);
            double coverageRate = (distinctHashes - 1.0) / (objs.Length - 1.0);
            Console.WriteLine(ObjectToCode.GetCSharpFriendlyTypeName(typeof(T)) + " has a hash coverate rate of " + (coverageRate * 100.0).ToString("f2") + "% (" + distinctHashes + " hashes over " + objs.Length + " distinct objects).");
        }

        static void Benchmark(string label, Action action) {
            Console.WriteLine((label + ": ").PadLeft(26) + Time(action).ToString("f3") + "ms");
        }

        static double Time(Action action) {
            var initial = Stopwatch.StartNew();
            var timings =
                Enumerable.Range(0, 1000000)
                .Select(_ => {
                    var sw = Stopwatch.StartNew();
                    action();
                    return sw.Elapsed.TotalMilliseconds;
                })
                .TakeUntil(t => initial.ElapsedMilliseconds >= 100)
                .OrderBy(t => t)
                .ToArray();
            return timings.Take((timings.Length + 3) / 4).Average();
        }
    }
}
