ValueUtils
==========

A C# library to make work with value-semantics easier by e.g. (efficiently) implementing Equals and GetHashCode for you.

The library is available on nuget (for import or direct download) as [ValueUtils](https://www.nuget.org/packages/ValueUtils/)

Usage:

```C#
using ValueUtils;
class ExampleClass {
	public string myMember;
	protected readonly DateTime supports_readonly_too;
	int private_int;
// ...
}


// ...
Func<ExampleClass, int> hashfunc = FieldwiseHasher<ExampleClass>.Instance;
//or simply
FieldwiseHasher.Get(my_example_object); //returns an int hashcode
```