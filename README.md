ValueUtils
==========

A C# library to make work with value-semantics easier by e.g. (efficiently) implementing Equals and GetHashCode for you.

The library is available on nuget (for import or direct download) as [ValueUtils](https://www.nuget.org/packages/ValueUtils/)


Usage:
---

The easiest way to use value semantics is to derive from `ValueObject<>`, for example:

```C#
using ValueUtils;
sealed class MyValueObject : ValueObject<MyValueObject> {
	public int A, B, C;
	public string X,Y, Z;
// ...
}
```
A class deriving from `ValueObject<T>` implements `IEquatable<T>` and has `Equals(object)`, `Equals(T)`, `GetHashCode()` and the == and != operators implemented in terms of their fields.


Given the following example class:
```C#
class ExampleClass {
	public string myMember;
	protected readonly DateTime supports_readonly_too;
	int private_int;
// ...
}
```

Hash code usage is as follows:

```C#
using ValueUtils;

Func<ExampleClass, int> hashfunc = FieldwiseHasher<ExampleClass>.Instance;
//or directly 
int hashcode = FieldwiseHasher.Hash(my_example_object);
```

Equality usage is as follows:
```C#
using ValueUtils;

Func<ExampleClass, ExampleClass, bool> equalityComparer = FieldwiseEquality<ExampleClass>.Instance;
//or directly 
bool areEqual = FieldwiseEquality.AreEqual(my_example_object, another_example_object);
```

The above implementations can be considerably faster that the built-in `ValueType`-provided defaults for `struct`s (which use reflection every call), which is why they're a good fit to help implement `GetHashCode` and `Equals` for structs.

Limitations and gotcha's
----
`ValueObject<>` supports self-referential types (like tree structures or a singly linked list), but does not support cyclical types - such as a doubly linked list.  Whenever a cycle is encountered, the hash function and equals operations will not terminate (until the stack overflows).

Equality is implemented on a per-type basis, and that means inheritance gets confusing.  It's OK to *have* a base class (and base class fields will affect hash and equality), but if you use the base-class's equality and/or hash implementation on a subclass *instance* the code will seem to work but only consider the fields of the base class.  Best practice: don't create sub-classes that add new fields; and if you do then at least never use the base-class equality+hashcode implementations.  This is why ValueObject verifies that its subclasses must be sealed.

FieldwiseHasher and FieldwiseEquality "work" on almost all types, including types with private members in other assemblies - however, if you don't know the internals, you can't be sure what's being included in the equality computations.  In particular, if an object is lazily initialized, two semantically equivalent objects might compute as unequal simply because one is initialized and the other is not.  In practice this is rarely a problem.


Performance and hash-quality
----
All performance measurements were done on an i7-4770k at a fixed clock rate of 4.0GHz.    Timings are in nanoseconds per object.  Datasets are all approximately 3000000 objects in size. Loops over the dataset were repeated until 10 seconds were up, then the fastest quartile average reported (this minimizes interference by other processes on my dev machine since random interference is almost always bad for performance, not good).  Some hash generators (notably `struct`) are so poor that this wasn't feasible, those timings are omitted (NaN) below.

Note that even a perfect hash mix is expected to have 0.03-0.04% colliding buckets, so if you see numbers like that in the data below, a hash if functioning as expected.  Numbers better(lower) than that are actually worrisome, because that means some kind of structure in the input is being exploited, and that likely means similar but slightly different data exists that will have lots of collisions.  And of course, number much higher that that directly impact performance.

<div>
  <table>
    <thead>
      <tr>
        <th colspan="7">Realistic scenario with an enum, a string, a DateTime, an int? and 3 int fields.</th>
      </tr>
      <tr>
        <th>Name</th>
        <th>Collisions</th>
        <th>Distinct Hashcodes</th>
        <th>.ToDictionary()</th>
        <th>.Distinct().Count()</th>
        <th>.Equals()</th>
        <th>.GetHashCode()</th>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td>ComplicatedManual</td>
        <td>0.04%</td>
        <td>2912961 / 2914000</td>
        <td>218.8</td>
        <td>199.6</td>
        <td>6.9</td>
        <td>17.4</td>
      </tr>
      <tr>
        <td>ComplicatedValueObject</td>
        <td>0.04%</td>
        <td>2912977 / 2914000</td>
        <td>250.2</td>
        <td>230.5</td>
        <td>21.4</td>
        <td>42.1</td>
      </tr>
      <tr>
        <td>Tuple</td>
        <td>0.03%</td>
        <td>2913001 / 2914000</td>
        <td>482.2</td>
        <td>494.8</td>
        <td>257.6</td>
        <td>263.5</td>
      </tr>
      <tr>
        <td>ComplicatedStruct</td>
        <td>100.00%</td>
        <td>2 / 2914000</td>
        <td>NaN</td>
        <td>NaN</td>
        <td>1002.3</td>
        <td>97.2</td>
      </tr>
      <tr>
        <td>Anonymous Type</td>
        <td>0.03%</td>
        <td>2913022 / 2914000</td>
        <td>261.0</td>
        <td>247.8</td>
        <td>31.5</td>
        <td>52.9</td>
      </tr>
    </tbody>
  </table>
  <table>
    <thead>
      <tr>
        <th colspan="7">A simple pair of ints</th>
      </tr>
      <tr>
        <th>Name</th>
        <th>Collisions</th>
        <th>Distinct Hashcodes</th>
        <th>.ToDictionary()</th>
        <th>.Distinct().Count()</th>
        <th>.Equals()</th>
        <th>.GetHashCode()</th>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td>IntPairManual</td>
        <td>0.02%</td>
        <td>2975318 / 2976000</td>
        <td>159.3</td>
        <td>133.6</td>
        <td>3.8</td>
        <td>1.7</td>
      </tr>
      <tr>
        <td>IntPairValueObject</td>
        <td>0.03%</td>
        <td>2974963 / 2976000</td>
        <td>199.3</td>
        <td>181.9</td>
        <td>20.0</td>
        <td>16.9</td>
      </tr>
      <tr>
        <td>Tuple</td>
        <td>38.31%</td>
        <td>1835788 / 2976000</td>
        <td>353.7</td>
        <td>289.2</td>
        <td>98.4</td>
        <td>54.9</td>
      </tr>
      <tr>
        <td>IntPairStruct</td>
        <td>56.61%</td>
        <td>1291168 / 2976000</td>
        <td>864.7</td>
        <td>812.6</td>
        <td>31.4</td>
        <td>36.8</td>
      </tr>
      <tr>
        <td>Anonymous Type</td>
        <td>4.69%</td>
        <td>2836344 / 2976000</td>
        <td>185.2</td>
        <td>158.2</td>
        <td>15.3</td>
        <td>13.5</td>
      </tr>
    </tbody>
  </table>
  <table>
    <thead>
      <tr>
        <th colspan="7">Two ints with both the same value</th>
      </tr>
      <tr>
        <th>Name</th>
        <th>Collisions</th>
        <th>Distinct Hashcodes</th>
        <th>.ToDictionary()</th>
        <th>.Distinct().Count()</th>
        <th>.Equals()</th>
        <th>.GetHashCode()</th>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td>IntPairManual</td>
        <td>0.37%</td>
        <td>2988915 / 3000000</td>
        <td>188.5</td>
        <td>155.6</td>
        <td>3.6</td>
        <td>1.7</td>
      </tr>
      <tr>
        <td>IntPairValueObject</td>
        <td>0.03%</td>
        <td>2999012 / 3000000</td>
        <td>200.8</td>
        <td>194.6</td>
        <td>19.7</td>
        <td>16.5</td>
      </tr>
      <tr>
        <td>Tuple</td>
        <td>22.07%</td>
        <td>2337827 / 3000000</td>
        <td>145.2</td>
        <td>140.5</td>
        <td>76.4</td>
        <td>55.1</td>
      </tr>
      <tr>
        <td>IntPairStruct</td>
        <td>100.00%</td>
        <td>1 / 3000000</td>
        <td>NaN</td>
        <td>NaN</td>
        <td>31.0</td>
        <td>36.7</td>
      </tr>
      <tr>
        <td>Anonymous Type</td>
        <td>0.00%</td>
        <td>3000000 / 3000000</td>
        <td>144.5</td>
        <td>106.3</td>
        <td>12.1</td>
        <td>13.5</td>
      </tr>
    </tbody>
  </table>
  <table>
    <thead>
      <tr>
        <th colspan="7">Two ints such that (x,y) is present iif (y,x) is present in the dataset</th>
      </tr>
      <tr>
        <th>Name</th>
        <th>Collisions</th>
        <th>Distinct Hashcodes</th>
        <th>.ToDictionary()</th>
        <th>.Distinct().Count()</th>
        <th>.Equals()</th>
        <th>.GetHashCode()</th>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td>IntPairManual</td>
        <td>0.62%</td>
        <td>3014881 / 3033584</td>
        <td>154.3</td>
        <td>140.5</td>
        <td>3.6</td>
        <td>1.7</td>
      </tr>
      <tr>
        <td>IntPairValueObject</td>
        <td>0.03%</td>
        <td>3032561 / 3033584</td>
        <td>192.8</td>
        <td>174.6</td>
        <td>19.6</td>
        <td>17.0</td>
      </tr>
      <tr>
        <td>Tuple</td>
        <td>41.47%</td>
        <td>1775545 / 3033584</td>
        <td>457.5</td>
        <td>432.1</td>
        <td>76.0</td>
        <td>54.8</td>
      </tr>
      <tr>
        <td>IntPairStruct</td>
        <td>74.50%</td>
        <td>773500 / 3033584</td>
        <td>804.6</td>
        <td>775.8</td>
        <td>31.0</td>
        <td>36.6</td>
      </tr>
      <tr>
        <td>Anonymous Type</td>
        <td>0.79%</td>
        <td>3009536 / 3033584</td>
        <td>175.2</td>
        <td>161.2</td>
        <td>12.1</td>
        <td>13.5</td>
      </tr>
    </tbody>
  </table>
  <table>
    <thead>
      <tr>
        <th colspan="7">A reference to the type itself and two int fields.  The dataset contains exactly one level of nesting such that the outer object is (x,y) when the inner is (y,x).</th>
      </tr>
      <tr>
        <th>Name</th>
        <th>Collisions</th>
        <th>Distinct Hashcodes</th>
        <th>.ToDictionary()</th>
        <th>.Distinct().Count()</th>
        <th>.Equals()</th>
        <th>.GetHashCode()</th>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td>NastyNestedManual</td>
        <td>24.14%</td>
        <td>2267216 / 2988648</td>
        <td>225.1</td>
        <td>181.5</td>
        <td>6.3</td>
        <td>5.0</td>
      </tr>
      <tr>
        <td>NastyNestedValueObject</td>
        <td>0.03%</td>
        <td>2987634 / 2988648</td>
        <td>239.7</td>
        <td>208.8</td>
        <td>30.9</td>
        <td>33.0</td>
      </tr>
      <tr>
        <td>Tuple</td>
        <td>57.80%</td>
        <td>1261193 / 2988648</td>
        <td>489.5</td>
        <td>491.8</td>
        <td>103.3</td>
        <td>132.0</td>
      </tr>
    </tbody>
  </table>
</div>