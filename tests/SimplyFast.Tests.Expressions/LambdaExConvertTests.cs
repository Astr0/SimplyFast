using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;
using SF.Expressions;

namespace SF.Tests.Expressions
{
    [TestFixture]
    public class LambdaExConvertTests
    {
        private readonly Expression<Func<int, int>> _convert = x => x + 2;

        private static T Compile<T>(LambdaExpression ex)
            where T : class
        {
            var lam = LambdaEx.Convert(ex, typeof(T));
            return lam.Compile() as T;
        }

        private T Compile<T>()
            where T : class
        {
            return Compile<T>(_convert);
        }

        private delegate int RefInt(ref int v);

        [Test]
        public void ConvertCanAddByRef()
        {
            var i = 2;
            var c = Compile<RefInt>();
            Assert.AreEqual(4, c(ref i));
            Assert.AreEqual(2, i);
            i = 0;
            Assert.AreEqual(2, c(ref i));
            Assert.AreEqual(0, i);
        }

        [Test]
        public void ConvertCanAddOutput()
        {
            Expression<Action<IList<int>>> a = x => x.Clear();
            var c = Compile<Func<IList<int>, int>>(a);
            Assert.AreEqual(0, c(new List<int>()));
        }

        [Test]
        public void ConvertCanRemoveByRef()
        {
            var p = Expression.Parameter(typeof(int).MakeByRefType(), "p");
            Assert.IsTrue(p.IsByRef);
            var lambda0 = Expression.Lambda(typeof(RefInt), Expression.PreIncrementAssign(p), p);
            var lambda = Expression.Lambda(Expression.PreIncrementAssign(p), p);

            var c0 = (RefInt) lambda0.Compile();
            var i = 2;
            Assert.AreEqual(3, c0(ref i));
            Assert.AreEqual(3, i);

            var c = Compile<RefInt>(lambda);
            i = 2;
            Assert.AreEqual(3, c(ref i));
            Assert.AreEqual(3, i);

            var cc = Compile<Func<int, int>>(lambda);
            i = 2;
            Assert.AreEqual(3, cc(i));
            Assert.AreEqual(2, i);
        }

        [Test]
        public void ConvertFailsIfNoArguments()
        {
            Assert.Throws<ArgumentException>(() => LambdaEx.Convert(_convert, typeof(Func<int>)));
        }

        [Test]
        public void ConvertOkWithConvertInput()
        {
            var c = Compile<Func<decimal, int>>();
            Assert.AreEqual(4, c(2M));
            Assert.AreEqual(2, c(0M));

            var c2 = Compile<Func<object, int>>();
            Assert.AreEqual(4, c2(2));
            Assert.AreEqual(2, c2(0));
        }

        [Test]
        public void ConvertOkWithConvertOutput()
        {
            var c = Compile<Func<int, decimal>>();
            Assert.AreEqual(4M, c(2));
            Assert.AreEqual(2M, c(0));

            var c2 = Compile<Func<int, object>>();
            Assert.AreEqual(4, c2(2));
            Assert.AreEqual(2, c2(0));
        }

        [Test]
        public void ConvertOkWithIgnoreOutput()
        {
            var c = Compile<Action<int>>();
            Assert.DoesNotThrow(() => c(2));
        }

        [Test]
        public void ConvertOkWithMoreArgs()
        {
            var c = Compile<Func<int, int, int>>();
            Assert.AreEqual(4, c(2, 0));
            Assert.AreEqual(2, c(0, 2));
        }
    }
}