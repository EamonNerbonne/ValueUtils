<Query Kind="FSharpExpression">
  <Reference Relative="..\..\emn\programs\EmnExtensions\bin\Release\EmnExtensions.dll">C:\VCS\emn\programs\EmnExtensions\bin\Release\EmnExtensions.dll</Reference>
  <NuGetReference>AvsAn</NuGetReference>
  <NuGetReference>ExpressionToCodeLib</NuGetReference>
  <NuGetReference>FSPowerPack.Core.Community</NuGetReference>
  <NuGetReference>morelinq</NuGetReference>
  <NuGetReference>ValueUtils</NuGetReference>
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
  <Namespace>ValueUtils</Namespace>
</Query>

let rec gcd a b = 
	if b = 0 then 
		a
	else
		gcd b (a % b)
let cost offset factor maxcost = 
//	let nums = [
//		for i in 1..30 ->
//			offset + factor * i
//	]
	let rec offsetcost n i sum = 
		if i > 50 then 
			sum
		else
			let m = offset + factor * i
			offsetcost n (i+1) (sum + gcd m n)
//			if gcd m n > 1 then
//				offsetcost n (i+1) (sum+1)
//			else
//				offsetcost n (i+1) sum
	
	let rec paircost2 i sum =
		if sum >= maxcost || i >= 50  then 
			sum
		else
			let n = offset + factor * i
			paircost2 (i+1) (offsetcost n (i+1) sum)


//	let rec paircost (sum:int) (xs:int list) =
//		match xs with
//		| [] -> sum
//		| n::tail ->
//			paircost ((tail |> List.filter (fun m -> gcd m n > 1) |> List.length) + sum) tail
			
	paircost2 1 0
//	let badpairs = [
//		for n in nums do
//			for m in nums do
//				if n < m && gcd m n > 1 then
//					yield 0
//	]
//	badpairs |> List.length

let rec findBest (best, offsets, factors, allfactors) = 
	match (offsets, factors) with
	//(bestcost, offsets, factors, allfactors)
	| (offset::_, factor::tailfactors) ->
		let (bestcost, _, _) = best
		let currcost = cost offset factor bestcost
		
		let nextbest =
			if currcost < bestcost then
				let curr = (currcost, offset, factor)
				printfn "%A" curr
				curr
			else
				best
		findBest (nextbest, offsets, tailfactors, allfactors)
	| (offset::tailoffsets, []) ->
		findBest (best, tailoffsets, allfactors, allfactors)
	| ([], _) ->
		best
//let primes max = 
//    let array = new BitArray(max, true);
//    let lastp = Math.Sqrt(float max) |> int
//    for p in 2..lastp+1 do
//        if array.Get(p) then
//            for pm in p*2..p..max-1 do
//                array.Set(pm, false);
//    seq { for i in 2..max-1 do if array.Get(i) then yield i }
//
//		
//let reasonableOffsets =  primes 65536 |> List.ofSeq  |> Seq.take 2000 |> List.ofSeq
let reasonableOffsets = [1..2..50000001] 
let reasonableFactors = [2..2..50000002] 
		
findBest ( (10000,0,0), reasonableOffsets, reasonableFactors, reasonableFactors)
