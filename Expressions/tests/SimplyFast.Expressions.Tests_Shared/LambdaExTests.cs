using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;

namespace SF.Expressions.Tests
{
    [TestFixture]
    public class LambdaExTests
    {
        [Test]
        public void SignatureMatchWorks()
        {
            Expression<Func<string, int, string>> f = (s, i) => s + i;
            Assert.IsTrue(LambdaEx.SignatureMatch(f, typeof(string), typeof(string), typeof(int)));
            Assert.IsFalse(LambdaEx.SignatureMatch(f, typeof(string), typeof(int), typeof(string)));
            Assert.IsFalse(LambdaEx.SignatureMatch(f, typeof(string), typeof(int)));
            Assert.IsFalse(LambdaEx.SignatureMatch(f, typeof(string), typeof(string), typeof(int), typeof(string)));
            Assert.IsFalse(LambdaEx.SignatureMatch(f, typeof(void), typeof(string), typeof(int)));
            Assert.IsFalse(LambdaEx.SignatureMatch(f, typeof(string), typeof(string), typeof(string)));
            Assert.IsFalse(LambdaEx.SignatureMatch(f, typeof(int), typeof(string), typeof(string)));
            Expression<Action<IList<string>>> a = s => s.Clear();
            Assert.IsTrue(LambdaEx.SignatureMatch(a, typeof(void), typeof(IList<string>)));
            Assert.IsFalse(LambdaEx.SignatureMatch(a, typeof(IList<string>), typeof(IList<string>)));
            Assert.IsFalse(LambdaEx.SignatureMatch(a, typeof(IList<string>)));
            Assert.IsFalse(LambdaEx.SignatureMatch(a, typeof(void)));
        }

        [Test]
        public void InlineWorks()
        {
            Expression<Func<int, int, int>> add = (a, b) => a + b;
            var param = Expression.Parameter(typeof (int), "p");
            var inline = LambdaEx.Inline(add, param, param);
            Assert.AreEqual("(p + p)", inline.ToDebugString());
            Assert.Throws<ArgumentException>(() => LambdaEx.Inline(add, param));
            Assert.Throws<ArgumentException>(() => LambdaEx.Inline(add, param, param, param));
        }

        [Test]
        public void GetDelegateTypeWorks()
        {
            Assert.AreEqual(typeof(Action), LambdaEx.GetDelegateType(typeof(void)));
            Assert.AreEqual(typeof(Action<int>), LambdaEx.GetDelegateType(typeof(void), typeof(int)));
            Assert.AreEqual(typeof(Action<int, string>), LambdaEx.GetDelegateType(typeof(void), typeof(int), typeof(string)));
            Assert.AreEqual(typeof(Func<int>), LambdaEx.GetDelegateType(typeof(int)));
            Assert.AreEqual(typeof(Func<int, string>), LambdaEx.GetDelegateType(typeof(string), typeof(int)));
            Assert.AreEqual(typeof(Func<string, int, string>), LambdaEx.GetDelegateType(typeof(string), typeof(string), typeof(int)));
        }
    }
}