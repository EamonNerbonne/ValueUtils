<Query Kind="Statements">
  <Reference Relative="..\..\emn\programs\EmnExtensions\bin\Release\EmnExtensions.dll">C:\VCS\emn\programs\EmnExtensions\bin\Release\EmnExtensions.dll</Reference>
  <NuGetReference>AvsAn</NuGetReference>
  <NuGetReference>ExpressionToCodeLib</NuGetReference>
  <NuGetReference>FSPowerPack.Core.Community</NuGetReference>
  <NuGetReference>morelinq</NuGetReference>
  <Namespace>AvsAnLib</Namespace>
  <Namespace>EmnExtensions</Namespace>
  <Namespace>EmnExtensions.Algorithms</Namespace>
  <Namespace>EmnExtensions.MathHelpers</Namespace>
  <Namespace>ExpressionToCodeLib</Namespace>
  <Namespace>Microsoft.FSharp.Collections</Namespace>
  <Namespace>Microsoft.FSharp.Core</Namespace>
  <Namespace>MoreLinq</Namespace>
  <Namespace>System</Namespace>
  <Namespace>System.Collections.Generic</Namespace>
  <Namespace>System.Dynamic</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>System.Linq</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Xml.Linq</Namespace>
</Query>

Func<uint,int,uint> rotate  = (v, count) => (v << count) | (v >> (32 - count));
Func<byte,int,byte> rotate8  = (v, count) => (byte)((v << count) | (v >> (8 - count)));
Func<uint,int,uint> rotate16  = (v, count) => 65535u & (uint)((v << count) | (v >> (16 - count)));

//var scaler = Enumerable.Range(0,8).Select(i =>
//	Enumerable.Range(0,10).Select(b=> 1u << (i*(b/3+1) + b*b)%16)
//		//.Pipe(s=>Convert.ToString(s,2).PadLeft(16,'0').Dump())
//		.Aggregate((x,y)=>x|y)
//	
//	)
//	.Pipe(s=>
//		("---\n" + Convert.ToString(s,2).PadLeft(16,'0') + "\n").Dump())
//		
//	.ToArray();
//scaler = Enumerable.Repeat(new Random(0),8).Select(r=> (uint)r.Next()&65536u).ToArray();
//Convert.ToString(scaler.Aggregate((x,y)=>x|y),2).PadLeft(16,'0').Dump();

//Enumerable.Range(0,65536).Select(i=>(uint)i)
//	.Select(x=>  x * scaler[2] ^ x * scaler[6])
// .Select(x=>(x ^ x>>16) &65535).Distinct().Count().Dump();
//


Enumerable.Range(0,65536).Select(i=>(uint)i)
	.Select(x=> ((x<<1) + x  ) + ((x<<4) + x  ))
 .Select(x=>(x ^ x>>16) &65535).Distinct().Count().Dump();

//Enumerable.Range(0,65536).Select(i=>(uint)i)
//	.Select(x=> x*((1<<3)-1) + x*((1<<7)-1) )
// .Select(x=>x &65535).Distinct().Count().Dump();

//Enumerable.Range(0,65536).Select(i=>(uint)i)
//	.Select(x=> (rotate16(x,2) ^ rotate16(x,3) ^ rotate16(x,4)) ^ ( rotate16(x,8) ^ rotate16(x,10)) )
// .Select(x=>x &65535).Distinct().Count().Dump();


Enumerable.Range(0,65536).Select(i=>(uint)i)
	.Select(x=> ((x*31 + 7)*31 +2)*31+x)
 .Select(x=>(x>>16 ^ x ) &65535).Distinct().Count().Dump();
 
// Enumerable.Range(0,65536).Select(i=>(uint)i)
//	.Select(x=> ((((((0*31 + 0)*31+ x) *31+0)*31+0)*31 + 0)*31+x))
// .Select(x=>x &65535).Distinct().Count().Dump();

Enumerable.Range(0,65536).Select(i=>(uint)i)
	.Select(x=> x*3 + x*9)
 .Select(x=>(x ^ x>>16) &65535).Distinct().Count().Dump();
 
 Enumerable.Range(0,65536).Select(i=>(uint)i)
	.Select(x=> x*2 + (x>>0) + x*8 + (x>>3))
 .Select(x=>(x ^ x>>16) &65535).Distinct().Count().Dump();

