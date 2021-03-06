﻿using System;
using System.Collections;
using Xunit;
using SimplyFast.Comparers;

namespace SimplyFast.Tests.Comparers
{
    
    public class FuncEqualityComparerTests
    {
        [Fact]
        public void CompareInts()
        {
            var comparer = EqualityComparerEx.Func((int a, int b) => a == b);
            Assert.True(comparer.Equals(1, 1));
            Assert.False(comparer.Equals(1, 2));
            // ReSharper disable once EqualExpressionComparison
            Assert.True(comparer.GetHashCode(1) == comparer.GetHashCode(1));
            Assert.False(comparer.GetHashCode(1) == comparer.GetHashCode(2));
        }

        [Fact]
        public void CompareTestClassByOneProp()
        {
            var o1 = new ComparersTestClass {A = "test1", B = 1};
            var oe1 = new ComparersTestClass {A = "test1", B = 2};
            var o2 = new ComparersTestClass {A = "test2", B = 2};
            var comparer = EqualityComparerEx.Func<ComparersTestClass>((a, b) => a.A == b.A, a => a.A.GetHashCode());
            Assert.True(comparer.Equals(o1, o1));
            Assert.True(comparer.Equals(o1, oe1));
            Assert.False(comparer.Equals(o1, o2));
            // ReSharper disable once EqualExpressionComparison
            Assert.True(comparer.GetHashCode(o1) == comparer.GetHashCode(o1));
            Assert.True(comparer.GetHashCode(o1) == comparer.GetHashCode(oe1));
            Assert.False(comparer.GetHashCode(o1) == comparer.GetHashCode(o2));
        }

        [Fact]
        public void CompareTestClassByTwoProp()
        {
            var o1 = new ComparersTestClass {A = "test1", B = 1};
            var oe1 = new ComparersTestClass {A = "test1", B = 1};
            var o2 = new ComparersTestClass {A = "test1", B = 2};
            var comparer = EqualityComparerEx.Func<ComparersTestClass>((a, b) => a.A == b.A && a.B == b.B,
                                                                   a =>
                                                                   ((a.A != null ? a.A.GetHashCode() : 0)*397) ^
                                                                   a.B);
            Assert.True(comparer.Equals(o1, o1));
            Assert.True(comparer.Equals(o1, oe1));
            Assert.False(comparer.Equals(o1, o2));
            // ReSharper disable once EqualExpressionComparison
            Assert.True(comparer.GetHashCode(o1) == comparer.GetHashCode(o1));
            Assert.True(comparer.GetHashCode(o1) == comparer.GetHashCode(oe1));
            Assert.False(comparer.GetHashCode(o1) == comparer.GetHashCode(o2));
        }

        [Fact]
        public void NonGenericCompareInts()
        {
            var comparer = EqualityComparerEx.Func<int>((a, b) => a == b);
            object one = 1;
            object two = 2;
            var comparerObj = comparer as IEqualityComparer;
            Assert.True(comparerObj.Equals(one, 1));
            Assert.False(comparerObj.Equals(one, two));
            // ReSharper disable once EqualExpressionComparison
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentException>(() => comparerObj.Equals(one, "1"));
            Assert.True(comparerObj.GetHashCode(one) == comparer.GetHashCode(1));
            Assert.False(comparerObj.GetHashCode(one) == comparerObj.GetHashCode(two));
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<ArgumentException>(() => comparerObj.GetHashCode("1"));
        }
    }
}