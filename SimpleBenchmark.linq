<Query Kind="Program">
  <Reference Relative="ValueUtils\bin\Release\ValueUtils.dll">C:\VCS\remote\ValueUtils\ValueUtils\bin\Release\ValueUtils.dll</Reference>
  <NuGetReference>ExpressionToCodeLib</NuGetReference>
  <NuGetReference>morelinq</NuGetReference>
  <Namespace>ExpressionToCodeLib</Namespace>
  <Namespace>MoreLinq</Namespace>
  <Namespace>System</Namespace>
  <Namespace>System.Collections.Generic</Namespace>
  <Namespace>System.Dynamic</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>System.Linq</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Xml.Linq</Namespace>
  <Namespace>ValueUtils</Namespace>
</Query>

void Main()
{
	
	var manuals = ( 
		from a in MoreEnumerable.Generate(-100, a => a + 4).TakeWhile(a => a < 100)
		from b in MoreEnumerable.Generate(1, b => b << 1).TakeWhile(b => b > 0)
		from c in MoreEnumerable.Generate(1000, c => (int) (c/1.1)).TakeWhile(c => c > 0)
		select new TestManual {
			AnEnum = SeekOrigin.Current,
			A = a,
			B = b,
			C = c,
			Label = Math.Min(a,c).ToString(),
			Time = new DateTime(2014,1,1) + TimeSpan.FromSeconds(a + c + Math.Log(b))
		}
		).ToArray();
	var valueObjects  = manuals.Select(m=>m.ToValueObject()).ToArray();
	var tuples  = manuals.Select(m=>m.ToTuple()).ToArray();
	var structs  = manuals.Select(m=>m.ToStruct()).ToArray();
	AnalyzeHashQuality(manuals);
	AnalyzeHashQuality(valueObjects);
	AnalyzeHashQuality(tuples);
	AnalyzeHashQuality(structs);

	Benchmark("Manual GetHashCode()", () => {
		foreach(var inst in manuals)
			inst.GetHashCode();
	});
	Benchmark("ValueObject<> GetHashCode",  () => {
		foreach(var inst in valueObjects)
			inst.GetHashCode();
	});
	Benchmark("struct GetHashCode()",  () => {
		foreach(var inst in structs)
			inst.GetHashCode();
	});
	Benchmark("Tuple<> GetHashCode()",  () => {
		foreach(var inst in tuples)
			inst.GetHashCode();
	});

	Benchmark("Manual Equals()", () => {
		var instances = manuals;
		for(int i=0; i< instances.Length; i++) {
			instances[i].Equals(instances[i]);
			instances[i].Equals(instances[(i+1)%instances.Length]);
		}
	});
	Benchmark("ValueObject<> Equals()", () => {
		var instances = valueObjects;
		for(int i=0; i< instances.Length; i++) {
			instances[i].Equals(instances[i]);
			instances[i].Equals(instances[(i+1)%instances.Length]);
		}
	});
	Benchmark("struct Equals()", () => {
		var instances = structs;
		for(int i=0; i< instances.Length; i++) {
			instances[i].Equals(instances[i]);
			instances[i].Equals(instances[(i+1)%instances.Length]);
		}
	});
	Benchmark("Tuple<> Equals()", () => {
		var instances = tuples;
		for(int i=0; i< instances.Length; i++) {
			instances[i].Equals(instances[i]);
			instances[i].Equals(instances[(i+1)%instances.Length]);
		}
	});
	
	Benchmark("Manual Distinct().Count()", () => {
		manuals.Distinct().Count();
	});
	Benchmark("ValueObject<> Distinct().Count()",  () => {
		valueObjects.Distinct().Count();
	});
	Benchmark("struct Distinct().Count()",  () => {
		structs.Distinct().Count();
	});
	Benchmark("Tuple<> Distinct().Count()",  () => {
		tuples.Distinct().Count();
	});

}
void AnalyzeHashQuality<T>(T[] objs)
{
	int distinctHashes = objs.Select(o=> o.GetHashCode()).Distinct().Count();
	double coverageRate = (distinctHashes - 1.0) / (objs.Length - 1.0);
	Console.WriteLine(ObjectToCode.GetCSharpFriendlyTypeName(typeof(T)) +" has a hash coverate rate of " + (coverageRate*100.0).ToString("f2")+"% ("+distinctHashes +" hashes over " + objs.Length+" distinct objects)." );
	
}

void Benchmark(string label, Action action) {
	Console.WriteLine((label + ": ").PadLeft(26)+Time(action).ToString("f3")+"ms");
}

double Time(Action action) {
	var initial = Stopwatch.StartNew();
	var timings =
		Enumerable.Range(0,1000000)
		.Select(_=> {
			var sw = Stopwatch.StartNew();
			action();
			return sw.Elapsed.TotalMilliseconds;
		})
		.TakeUntil( t => initial.ElapsedMilliseconds >= 1000)
		.OrderBy(t=>t)
		.ToArray();
	return timings.Take((timings.Length +3)/4).Average();
}

public sealed class TestValueObject : ValueObject<TestValueObject> {
	public SeekOrigin AnEnum;
	public int A;
	public int? NullableInt;
	public int B;
	public string Label;
	public DateTime Time;
	public int C;
}
public struct TestStruct {
	public SeekOrigin AnEnum;
	public int A;
	public int? NullableInt;
	public int B;
	public string Label;
	public DateTime Time;
	public int C;
}
public sealed class TestManual : IEquatable<TestManual> {
	public SeekOrigin AnEnum;
	public int A;
	public int? NullableInt;
	public int B;
	public string Label;
	public DateTime Time;
	public int C;
	

	public bool Equals(TestManual obj) {
		return obj != null 
			&& obj.AnEnum == AnEnum
			&& obj.A == A
			&& obj.NullableInt == NullableInt
			&& obj.B == B
			&& obj.Label == Label
			&& obj.Time == Time
			&& obj.C == C;
	}
	public override bool Equals(object obj) {
		return Equals(obj as TestManual);
	}
	public override int GetHashCode() {
		return 
			AnEnum.GetHashCode() * 3 
			+ A.GetHashCode() * 5
			+ NullableInt.GetHashCode() * 7 
			+ B.GetHashCode() * 9 
			+ (default(string) == Label ? -1 : Label.GetHashCode() * 11)
			+ Time.GetHashCode() * 13
			+ C.GetHashCode() * 15;
	}
	
	public TestStruct ToStruct() {
		return new TestStruct {
			A=A, B=B, C=C, Label=Label,Time=Time,
		};
	}
	public TestValueObject ToValueObject() {
		return new TestValueObject {
			A=A, B=B, C=C, Label=Label,Time=Time,
		};
	}
	public Tuple<int,int,int,string,DateTime> ToTuple() {
		return Tuple.Create(A, B, C, Label, Time);
	}
}


// Define other methods and classes here