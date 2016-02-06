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

let maxIdx = 50u
let rec gcd a b = 
	if b = 0u then 
		a
	else
		gcd b (a % b)
		
let cost offset factor maxcost = 
	let rec offsetcost n idx sum = 
		if sum >= maxcost || idx > maxIdx then 
			sum
		else
			let m = offset + factor * idx
			let newcost = gcd m n
			offsetcost n (idx + 1u) (sum + newcost)
	
	let rec paircost2 idx sum =
		if sum >= maxcost || idx >= maxIdx  then 
			sum
		else
			let n = offset + factor * idx
			paircost2 (idx+1u) (offsetcost n (idx+1u) sum)


			
	paircost2 1u 0u

let rec findBest (best, offsets, factors, allfactors) = 
	match (offsets, factors) with
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
		
let maxFactor = 50000002u
let maxOffset = UInt32.MaxValue - maxIdx*maxFactor
let pMax = min maxOffset (uint32 Int32.MaxValue) |> int32

let primes max = 
    let lastp = Math.Sqrt(max+1 |> float) |> int
    let array = new BitArray(max/2, true);
	[|	
		yield 2
		for p in 3..2..lastp do
			if array.Get(p /2) then
				yield p
				for pm in p*p..p*2..max-1 do
					array.Set(pm/2, false);
		for i in (lastp+1 ||| 1)..2..max-1 do if array.Get(i/2) then yield i
	|]



let reasonableOffsets = primes pMax |> Array.map uint32 |> Array.rev |> List.ofArray
printfn "done preparing, running"
let reasonableFactors = [2u..2u..maxFactor] 
		
findBest ( (1000000u,0u,0u), reasonableOffsets, reasonableFactors, reasonableFactors)
