using System;
using System.Collections.Generic;
using System.Linq;
using ExpressionToCodeLib;
using ValueUtils;
using Xunit;

namespace ValueUtilsTest
{
    public class InheritanceWalkerTheory
    {
        [Fact]
        public void BaseTypesOfIntAreJustInt()
        {
            PAssert.That(() => ReflectionHelper.WalkMeaningfulInheritanceChain(typeof(int))
                .SequenceEqual(new[] { typeof(int) }));
        }

        [Fact]
        public void BaseTypesOfObjectAreEmpty()
        {
            PAssert.That(() => ReflectionHelper.WalkMeaningfulInheritanceChain(typeof(object))
                .SequenceEqual(new Type[0]));
        }

        [Fact]
        public void BaseTypesOfAnEnumAreJustThatEnum()
        {
            PAssert.That(() => ReflectionHelper.WalkMeaningfulInheritanceChain(typeof(SampleEnum))
                .SequenceEqual(new[] { typeof(SampleEnum) }));
        }

        [Fact]
        public void BaseTypesOfAStructAreJustThatStruct()
        {
            PAssert.That(() => ReflectionHelper.WalkMeaningfulInheritanceChain(typeof(SampleStruct))
                .SequenceEqual(new[] { typeof(SampleStruct) }));
        }

        [Fact]
        public void BaseTypesOfAGenericStructAreJustThatStruct()
        {
            PAssert.That(() => ReflectionHelper.WalkMeaningfulInheritanceChain(typeof(KeyValuePair<int, string>))
                .SequenceEqual(new[] { typeof(KeyValuePair<int, string>) }));
        }

        [Fact]
        public void BaseTypesOfAPlainClassAreJustThatClass()
        {
            PAssert.That(() => ReflectionHelper.WalkMeaningfulInheritanceChain(typeof(SampleClass))
                .SequenceEqual(new[] { typeof(SampleClass) }));
        }

        [Fact]
        public void BaseTypesOfASubClassAreFirstSubClassThenBaseClass()
        {
            PAssert.That(() => ReflectionHelper.WalkMeaningfulInheritanceChain(typeof(SampleSubClass))
                .SequenceEqual(new[] { typeof(SampleSubClass), typeof(SampleClass) }));
        }

        [Fact]
        public void BaseTypesOfASubSubClassIncludeTheCompleteChain()
        {
            PAssert.That(() => ReflectionHelper.WalkMeaningfulInheritanceChain(typeof(SampleSubSubClass))
                .SequenceEqual(new[] { typeof(SampleSubSubClass), typeof(SampleSubClass), typeof(SampleClass) }));
        }
    }
}
