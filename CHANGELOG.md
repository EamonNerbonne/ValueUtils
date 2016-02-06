1.3.0
---
 - Consider private fields of base classes when computing hashcodes or comparing for equality (PR #4)
 - Added `FieldwiseEqualityComparer<> : IEqualityComparer<>` for convenience.

1.2.2
---
 - Tune hash mixing coefficients to minimize collisions (based on actual testing, now).

1.2.1
---
 - Bugfix: remove Console logging from unusual code generator paths.
 - Tune hash mixing coefficients to minimize collisions.

1.2
---
Bugfix release; add support for "unusual" fields
 - nullables
 - structs
 - enums

1.1
---
Improve ValueObject<>.Equals performance by more that a factor 10 by using 
operator == or a specialized Equals method when available for a field.

1.0
---
Added ValueObject<> to simplify the creation of value-objects.  (1.0.1 is purely a documentation update)

0.2
---
Added support for a field-wise equality comparer.

0.1
---
Initial release; included a field-wise hashcode generator.
