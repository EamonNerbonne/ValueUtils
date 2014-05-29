<Query Kind="Program" />

void Main()
{
	var maybenull  = new Random().Next() %2==0? new AClass() : null;
	var elsenull = maybenull==null?new AClass():null;
	bool dce = false;
	Benchmark("NullCheck", () => {
		for(int i=0; i< 1000000; i++) {
			dce ^= NullCheck(maybenull) == NullCheck(elsenull);
		}
	});
	Benchmark("NullCheck2", () => {
		for(int i=0; i< 1000000; i++) {
			dce ^= NullCheck2(maybenull) == NullCheck2(elsenull);
		}
	});
	Benchmark("NullCheck3", () => {
		for(int i=0; i< 1000000; i++) {
			dce ^= NullCheck3(maybenull) == NullCheck3(elsenull);
		}
	});
	Console.WriteLine(dce);//make life difficult for dead code eliminator.
}

class AClass {
	public int Method() {return 13;}
}
AClass other = new AClass();
bool NullCheck(AClass a) {
	return a is AClass && A(a, other);
}
bool NullCheck2(AClass a) {
	return (object)a != null && A(a, other);
}
bool NullCheck3(AClass a) {
	return !ReferenceEquals(a, null)  && A(a, other);
}
//Note that if the method body contains nothing but the null check there's no difference; this suggests it's some kind of inlining issue.

Func<AClass, AClass, bool> A = (x,y) => x.Method() == y.Method();

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
		.TakeWhile( t => initial.ElapsedMilliseconds < 1000)
		.OrderBy(t=>t)
		.ToArray();
	return timings.Take((timings.Length +3)/4).Average();
}
