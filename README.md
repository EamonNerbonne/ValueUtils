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
using ValueUtils;
class ExampleClass {
	public string myMember;
	protected readonly DateTime supports_readonly_too;
	int private_int;
// ...
}
```

Hash code usage is as follows:

```C#
Func<ExampleClass, int> hashfunc = FieldwiseHasher<ExampleClass>.Instance;
//or directly 
int hashcode = FieldwiseHasher.Hash(my_example_object);
```

Equality usage is as follows:
```C#
Func<ExampleClass, ExampleClass, bool> equalityComparer = FieldwiseEquality<ExampleClass>.Instance;
//or directly 
bool areEqual = FieldwiseEquality.AreEqual(my_example_object, another_example_object);
```

The above implementations can be considerably faster that the built-in `ValueType`-provided defaults for `struct`s (which use reflection every call), which is why they're a good fit to help implement `GetHashCode` and `Equals` for structs.

Limitations and gotcha's
----
`ValueObject<>` supports self-referential types (like tree structures or a singly linked list), but does not support cyclical types - such as a doubly linked list.  Whenever a cycle is encountered, the hash function and equals operations will not terminate (until the stack overflows).

Equality is implemented on a per-type basis, and that means inheritance gets confusing.  It's OK to *have* a base class (and base class fields will affect hash and equality), but if you use the base-class's equality and/or hash implementation on a subclass *instance* the code will seem to work but only consider the fields of the base class.  Best practice: don't create sub-classes that add new fields; and if you do then at least never use the base-class equality+hashcode implementations.  This is why ValueObject verifies that its subclasses must be sealed.
