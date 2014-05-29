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
	
	var instances = ( 
		from a in MoreEnumerable.Generate(-100, a => a + 4).TakeWhile(a => a < 100)
		from b in MoreEnumerable.Generate(1, b => b << 1).TakeWhile(b => b > 0)
		from c in MoreEnumerable.Generate(1000, c => (int) (c/1.1)).TakeWhile(c => c > 0)
		select new TestManual {
			A = a,
			B = b,
			C = c,
			Label = Math.Min(a,c).ToString(),
			Time = new DateTime(2014,1,1) + TimeSpan.FromSeconds(a + c + Math.Log(b))
		}
		into s select s
			.ToValueObject()
		).ToArray();
	instances.Length.Dump();
	BestTime( () => {
//		for(int i=1; i< instances.Length; i++)
//			instances[i-1].Equals(instances[i]);
		foreach(var inst in instances)
			inst.GetHashCode();
//		instances.Distinct().Count();	
	}).Dump();
}

double BestTime(Action action) {
	TimeSpan best = TimeSpan.MaxValue;
	return 
		Enumerable.Range(0,400)
		.Select(_=> {
			var sw = Stopwatch.StartNew();
			action();
			return sw.Elapsed.TotalMilliseconds;
		})
		.OrderBy(t=>t)
		.Take(100)
		.Average();
}

public sealed class TestValueObject : ValueObject<TestValueObject> {
	public int A,B,C;
	public DateTime Time;
	public string Label;
}
public struct TestStruct {
	public int A,B,C;
	public DateTime Time;
	public string Label;
}
public sealed class TestManual : IEquatable<TestManual> {
	public int A,B,C;
	public DateTime Time;
	public string Label;
	public bool Equals(TestManual obj) {
		return obj != null 
		&& obj.A == A
		&& obj.B == B
		&& obj.C == C
		&& obj.Time == Time
		&& obj.Label == Label;
	}
	public override bool Equals(object obj) {
		return Equals(obj as TestManual);
	}
	public override int GetHashCode() {
		return A.GetHashCode() * 3 
			+ B.GetHashCode() * 5
			+ C.GetHashCode() * 7 
			+ Time.GetHashCode() * 9 
			+ (default(string) == Label ? -1 : Label.GetHashCode() * 11);
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