using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace SimplyFast.Expressions.Tests
{
    
    public class LambdaExTests
    {
        [Fact]
        public void SignatureMatchWorks()
        {
            Expression<Func<string, int, string>> f = (s, i) => s + i;
            Assert.True(LambdaEx.SignatureMatch(f, typeof(string), typeof(string), typeof(int)));
            Assert.False(LambdaEx.SignatureMatch(f, typeof(string), typeof(int), typeof(string)));
            Assert.False(LambdaEx.SignatureMatch(f, typeof(string), typeof(int)));
            Assert.False(LambdaEx.SignatureMatch(f, typeof(string), typeof(string), typeof(int), typeof(string)));
            Assert.False(LambdaEx.SignatureMatch(f, typeof(void), typeof(string), typeof(int)));
            Assert.False(LambdaEx.SignatureMatch(f, typeof(string), typeof(string), typeof(string)));
            Assert.False(LambdaEx.SignatureMatch(f, typeof(int), typeof(string), typeof(string)));
            Expression<Action<IList<string>>> a = s => s.Clear();
            Assert.True(LambdaEx.SignatureMatch(a, typeof(void), typeof(IList<string>)));
            Assert.False(LambdaEx.SignatureMatch(a, typeof(IList<string>), typeof(IList<string>)));
            Assert.False(LambdaEx.SignatureMatch(a, typeof(IList<string>)));
            Assert.False(LambdaEx.SignatureMatch(a, typeof(void)));
        }

        [Fact]
        public void InlineWorks()
        {
            Expression<Func<int, int, int>> add = (a, b) => a + b;
            var param = Expression.Parameter(typeof (int), "p");
            var inline = LambdaEx.Inline(add, param, param);
            Assert.Equal("(p + p)", inline.ToDebugString());
            Assert.Throws<ArgumentException>(() => LambdaEx.Inline(add, param));
            Assert.Throws<ArgumentException>(() => LambdaEx.Inline(add, param, param, param));
        }

        [Fact]
        public void GetDelegateTypeWorks()
        {
            Assert.Equal(typeof(Action), LambdaEx.GetDelegateType(typeof(void)));
            Assert.Equal(typeof(Action<int>), LambdaEx.GetDelegateType(typeof(void), typeof(int)));
            Assert.Equal(typeof(Action<int, string>), LambdaEx.GetDelegateType(typeof(void), typeof(int), typeof(string)));
            Assert.Equal(typeof(Func<int>), LambdaEx.GetDelegateType(typeof(int)));
            Assert.Equal(typeof(Func<int, string>), LambdaEx.GetDelegateType(typeof(string), typeof(int)));
            Assert.Equal(typeof(Func<string, int, string>), LambdaEx.GetDelegateType(typeof(string), typeof(string), typeof(int)));
        }
    }
}