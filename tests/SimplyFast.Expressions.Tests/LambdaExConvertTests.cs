using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace SimplyFast.Expressions.Tests
{
    
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

        [Fact]
        public void ConvertCanAddByRef()
        {
            var i = 2;
            var c = Compile<RefInt>();
            Assert.Equal(4, c(ref i));
            Assert.Equal(2, i);
            i = 0;
            Assert.Equal(2, c(ref i));
            Assert.Equal(0, i);
        }

        [Fact]
        public void ConvertCanAddOutput()
        {
            Expression<Action<IList<int>>> a = x => x.Clear();
            var c = Compile<Func<IList<int>, int>>(a);
            Assert.Equal(0, c(new List<int>()));
        }

        [Fact]
        public void ConvertCanRemoveByRef()
        {
            var p = Expression.Parameter(typeof(int).MakeByRefType(), "p");
            Assert.True(p.IsByRef);
            var lambda0 = Expression.Lambda(typeof(RefInt), Expression.PreIncrementAssign(p), p);
            var lambda = Expression.Lambda(Expression.PreIncrementAssign(p), p);

            var c0 = (RefInt) lambda0.Compile();
            var i = 2;
            Assert.Equal(3, c0(ref i));
            Assert.Equal(3, i);

            var c = Compile<RefInt>(lambda);
            i = 2;
            Assert.Equal(3, c(ref i));
            Assert.Equal(3, i);

            var cc = Compile<Func<int, int>>(lambda);
            i = 2;
            Assert.Equal(3, cc(i));
            Assert.Equal(2, i);
        }

        [Fact]
        public void ConvertFailsIfNoArguments()
        {
            Assert.Throws<ArgumentException>(() => LambdaEx.Convert(_convert, typeof(Func<int>)));
        }

        [Fact]
        public void ConvertOkWithConvertInput()
        {
            var c = Compile<Func<decimal, int>>();
            Assert.Equal(4, c(2M));
            Assert.Equal(2, c(0M));

            var c2 = Compile<Func<object, int>>();
            Assert.Equal(4, c2(2));
            Assert.Equal(2, c2(0));
        }

        [Fact]
        public void ConvertOkWithConvertOutput()
        {
            var c = Compile<Func<int, decimal>>();
            Assert.Equal(4M, c(2));
            Assert.Equal(2M, c(0));

            var c2 = Compile<Func<int, object>>();
            Assert.Equal(4, c2(2));
            Assert.Equal(2, c2(0));
        }

        [Fact]
        public void ConvertOkWithIgnoreOutput()
        {
            var c = Compile<Action<int>>();
            c(2);
        }

        [Fact]
        public void ConvertOkWithMoreArgs()
        {
            var c = Compile<Func<int, int, int>>();
            Assert.Equal(4, c(2, 0));
            Assert.Equal(2, c(0, 2));
        }
    }
}