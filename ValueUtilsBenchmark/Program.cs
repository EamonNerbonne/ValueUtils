﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ExpressionToCodeLib;
using MoreLinq;

namespace ValueUtilsBenchmark {
    struct CollisionStats {
        public int DistinctHashCodes, DistinctValues;
        public double Rate { get { return (DistinctValues - DistinctHashCodes - 1) / (DistinctValues - 1.0); } }
    }
    class HashAnalysisResult {
        public string Name;
        public CollisionStats Collisions;
        public double GetHashCodeNS, EqualsNS, DistinctCountNS, DictionaryNS;
        public XElement ToTableRow() {
            return new XElement("tr",
                new XElement("td", Name),
                new XElement("td", (Collisions.Rate * 100).ToString("f2") + "%"),
                new XElement("td", Collisions.DistinctHashCodes + " / " + Collisions.DistinctValues),
                new XElement("td", DictionaryNS.ToString("f1")),
                new XElement("td", DistinctCountNS.ToString("f1")),
                new XElement("td", EqualsNS.ToString("f1")),
                new XElement("td", GetHashCodeNS.ToString("f1"))
                );
        }
        public static XElement ToTableHead(string title) {
            return
                new XElement("thead",
                    new XElement("tr",
                        new XElement("th",
                             new XAttribute("colspan", 7),
                             title
                        )
                    ),
                    new XElement("tr",
                        new XElement("th", "Name"),
                        new XElement("th", "Collisions"),
                        new XElement("th", "Distinct Hashcodes"),
                        new XElement("th", ".ToDictionary()"),
                        new XElement("th", ".Distinct().Count()"),
                        new XElement("th", ".Equals()"),
                        new XElement("th", ".GetHashCode()")
                    )
                );
        }
        public static XElement ToTable(string title, IEnumerable<HashAnalysisResult> results) {
            return
                new XElement("table",
                    ToTableHead(title),
                    new XElement("tbody",
                        results.Select(r => r.ToTableRow())
                    )
                );
        }
    }

    class Program {
        static void Main(string[] args) {
            var complicatedTable = HashAnalysisResult.ToTable(
                "Complicated Case with enums, nullables, and strings", 
                BenchmarkComplicatedCases()
                );
            var intpairTable = HashAnalysisResult.ToTable(
                "A simple pair of ints",
                BenchmarkIntPairCases()
                );
            var duplicationTable = HashAnalysisResult.ToTable(
                "int-pair with both ints having the same value",
                BenchmarkDuplicatePairCases()
                );
            var symmetricalTable = HashAnalysisResult.ToTable(
                "int-pair with a dataset in which both the object and it's mirror image are present",
                BenchmarkSymmetricalCases()
                );
            var nestedTable = HashAnalysisResult.ToTable(
                "int-pair with self-reference; data set contains one level of nesting with nested values being mirror images of their containers",
                BenchmarkNastyNestedCases()
                );

            var tables = 
                new XElement("div",
                    complicatedTable, intpairTable, duplicationTable, symmetricalTable, nestedTable);
            Console.WriteLine(tables.ToString());
            tables.Save("BenchResults.xml");
        }

        static IEnumerable<HashAnalysisResult> BenchmarkNastyNestedCases() {
            var manuals = (
                from a in MoreEnumerable.Generate(-10000, a => a + 6).TakeWhile(a => a < 14100)
                from b0 in MoreEnumerable.Generate(int.MaxValue, c => (int)(c * 0.95)).TakeWhile(c => c > 0)
                from sign in new[] { -1, 1 }
                let b = sign * b0
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

            yield return ProcessList(manuals);
            yield return ProcessList(valueObjects);
            yield return ProcessList(tuples);
        }


        static IEnumerable<HashAnalysisResult> BenchmarkIntPairCases() {
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

            yield return ProcessList(manuals);
            yield return ProcessList(valueObjects);
            yield return ProcessList(tuples);
            yield return ProcessList(structs);
        }


        static IEnumerable<HashAnalysisResult> BenchmarkSymmetricalCases() {
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

            yield return ProcessList(manuals);
            yield return ProcessList(valueObjects);
            yield return ProcessList(tuples);
            yield return ProcessList(structs);
        }

        static IEnumerable<HashAnalysisResult> BenchmarkDuplicatePairCases() {
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

            yield return ProcessList(manuals);
            yield return ProcessList(valueObjects);
            yield return ProcessList(tuples);
            yield return ProcessList(structs);
        }

        static IEnumerable<HashAnalysisResult> BenchmarkComplicatedCases() {
            var manuals = (
                from a in MoreEnumerable.Generate(-500, a => a + 4).TakeWhile(a => a < 500)
                from b0 in MoreEnumerable.Generate(int.MaxValue, b => (int)(b * 0.9)).TakeWhile(b => b > 0)
                from sign in new[] { -1, 1 }
                let b = sign * b0
                from c in MoreEnumerable.Generate(1, c => c << 1).TakeWhile(c => c > 0)
                select new ComplicatedManual {
                    AnEnum = (SeekOrigin)(c % 3),
                    A = a,
                    B = b,
                    C = c,
                    NullableInt = a < b && b < c ? default(int?) : a & b & c,
                    Label = Math.Min(a, c).ToString(),
                    Time = new DateTime(2014, 1, 1) + TimeSpan.FromSeconds(a + c + Math.Log(b0))
                }
            ).ToArray();
            var valueObjects = manuals.Select(m => m.ToValueObject()).ToArray();
            var tuples = manuals.Select(m => m.ToTuple()).ToArray();
            var structs = manuals.Select(m => m.ToStruct()).ToArray();

            yield return ProcessList(manuals);
            yield return ProcessList(valueObjects);
            yield return ProcessList(tuples);
            yield return ProcessList(structs);
        }

        static HashAnalysisResult ProcessList<T>(T[] objs) {
            string name = ObjectToCode.GetCSharpFriendlyTypeName(typeof(T));
            var collisions = AnalyzeHashCollisions(objs);
            return new HashAnalysisResult {
                Name = name,
                Collisions = collisions,
                GetHashCodeNS = TimeInNS(() => {
                    foreach (var inst in objs)
                        inst.GetHashCode();
                }) / objs.Length,
                EqualsNS = TimeInNS(() => {
                    for (int i = 0; i < objs.Length; i++) {
                        objs[i].Equals(objs[i]);
                        objs[i].Equals(objs[(i + 1) % objs.Length]);
                    }
                }) / objs.Length / 2,
                DistinctCountNS = collisions.Rate > 0.99 ? double.NaN : TimeInNS(() => {
                    objs.Distinct().Count();
                }) / objs.Length,
                DictionaryNS = collisions.Rate > 0.99 ? double.NaN : TimeInNS(() => {
                    int idx = 0;
                    objs.ToDictionary(o => o, _ => idx++);
                }) / objs.Length,
            };
        }
        static CollisionStats AnalyzeHashCollisions<T>(T[] objs) {
            return new CollisionStats {
                DistinctValues = objs.Length,
                DistinctHashCodes = objs.Select(o => o.GetHashCode()).Distinct().Count()
            };
        }

        static double TimeInNS(Action action) {
            var initial = Stopwatch.StartNew();
            var timings =
                Enumerable.Range(0, 1000000)
                .Select(_ => {
                    var sw = Stopwatch.StartNew();
                    action();
                    return sw.Elapsed.TotalMilliseconds;
                })
                .TakeUntil(t => initial.ElapsedMilliseconds >= 10000)
                .OrderBy(t => t)
                .ToArray();
            return 1000000.0*timings.Take((timings.Length + 3) / 4).Average();
        }
    }
}