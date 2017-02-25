using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SimplyFast.Disposables;
using SimplyFast.Expressions.Dynamic;
using static SimplyFast.Expressions.Dynamic.DynamicControlBuilder;

namespace SimplyFast.Expressions.Tests
{
    [TestFixture]
    public class DynamicControlBuilderTests
    {
        [Test]
        public void BlockTest()
        {
            var lambda = Lambda(p =>
            {
                p(typeof(int), "a", typeof(int), "b");
                return Block(b => new[]
                {
                    b.t = p.a,
                    p.a = p.b,
                    p.b = b.t
                });
            });
            Assert.AreEqual(@"(Int32 a, Int32 b) => {
  Int32 t;
  (t = a);
  (a = b);
  (b = t);
}", lambda.ToDebugString());
        }


        [Test]
        public void BlockTestLastFine()
        {
            var lambda = Lambda(p =>
            {
                p(typeof(int), "a");
                p(typeof(int), "b");
                return Block(b => new[]
                {
                    b.t = p.a + p.b,
                    b.t
                });
            });
            Assert.AreEqual(@"(Int32 a, Int32 b) => {
  Int32 t;
  (t = (a + b));
  t;
}", lambda.ToDebugString());
        }

        [Test]
        public void BlockTestNoDeclaration()
        {
            var lambda = Lambda(p => Block(b => new[]
            {
                b.t = p(typeof(int), "a"),
                p.a = p(typeof(int), "b"),
                p.b = b.t
            }));
            Assert.AreEqual(@"(Int32 a, Int32 b) => {
  Int32 t;
  (t = a);
  (a = b);
  (b = t);
}", lambda.ToDebugString());
        }

        [Test]
        public void IfElseTest()
        {
            var lambda = Lambda(p => If(p(typeof(int), "a") >= 0, p.a, -p.a));
            Assert.AreEqual("(Int32 a) => IF((a >= 0), a, -a)", lambda.ToDebugString());
            var compiled = (Func<int, int>) lambda.Compile();
            Assert.AreEqual(0, compiled(0));
            Assert.AreEqual(2, compiled(2));
            Assert.AreEqual(2, compiled(-2));
        }

        [Test]
        public void IfNoElseTest()
        {
            var lambda = Lambda(p => Block(b =>
                new[]
                {
                    If(p(typeof(int), "a") >= 0, p.a = 2),
                    p.a * 3
                }));
            Assert.AreEqual(@"(Int32 a) => {
  IF((a >= 0), (a = 2), default(Int32));
  (a * 3);
}", lambda.ToDebugString());
            var compiled = lambda.CompileAs<Func<int, int>>();
            Assert.AreEqual(6, compiled(1));
            Assert.AreEqual(6, compiled(12));
            Assert.AreEqual(6, compiled(0));
            Assert.AreEqual(-3, compiled(-1));
            Assert.AreEqual(-6, compiled(-2));
        }

        [Test]
        public void LambdaFancyTest()
        {
            var lambda = Lambda(x => x(typeof(int), "a") + x(typeof(int), "b") + x.a);
            Assert.AreEqual("(Int32 a, Int32 b) => ((a + b) + a)", lambda.ToDebugString());
        }

        [Test]
        public void LambdaMultipleDeclaration()
        {
            var lambda = Lambda(p => p(typeof(int), "a") * p(typeof(int), "a"));
            Assert.AreEqual("(Int32 a) => (a * a)", lambda.ToDebugString());
        }

        [Test]
        public void LambdaNotSoFancyTest()
        {
            var lambda = Lambda(x =>
            {
                x(typeof(int), "a", typeof(int), "b");
                return x.a + x.b;
            });
            Assert.AreEqual("(Int32 a, Int32 b) => (a + b)", lambda.ToDebugString());
        }

        [Test]
        public void LambdaTest()
        {
            var lambda = Lambda(x => x(typeof(int), "a") + x(typeof(int), "b"));
            Assert.AreEqual("(Int32 a, Int32 b) => (a + b)", lambda.ToDebugString());
        }

        [Test]
        public void TestCast()
        {
            var lambda = EBuilder.Lambda(typeof(EBuilderInstanceTests.TestClass), a =>
            {
                var aexp = a.EBuilder();
                return Cast(aexp, typeof(object));
            });
            Assert.AreEqual("(TestClass p_0) => (p_0 As Object)", lambda.ToDebugString());
        }

        [Test]
        public void TestConvert()
        {
            var lambda = EBuilder.Lambda(typeof(EBuilderInstanceTests.TestClass), a =>
            {
                var aexp = a.EBuilder();
                return Convert(aexp, typeof(int));
            });
            Assert.AreEqual("(TestClass p_0) => Convert(p_0 As Int32)", lambda.ToDebugString());
        }

        [Test]
        public void TestConvertTo()
        {
            var lambda = EBuilder.Lambda(typeof(int), a =>
            {
                var aexp = a.EBuilder();
                return Convert(aexp, typeof(EBuilderInstanceTests.TestClass));
            });
            Assert.AreEqual("(Int32 p_0) => Convert(p_0 As TestClass)", lambda.ToDebugString());
        }

        [Test]
        public void TestDefault()
        {
            var lambda = Lambda(a => Default(typeof(int)));
            Assert.AreEqual("() => default(Int32)", lambda.ToDebugString());
        }

        [Test]
        public void TestFor()
        {
            var lambda = Lambda(p =>
                Block(b => new[]
                {
                    b.s = 0,
                    b.i = p(typeof(int), "c"),
                    For(() => b.i > 0, () => --b.i,
                        l => b.s += b.i),
                    b.s
                })
            );
            var compiled = (Func<int, int>) lambda.Compile();
            Assert.AreEqual(Enumerable.Range(0, 6).Sum(), compiled(5));
        }

        [Test]
        public void TestForEach()
        {
            var lambda = Lambda(p => Block(b => new[]
            {
                b.s = 0,
                ForEach(() => p(typeof(IEnumerable<int>), "c"), l => b.s += l.Current),
                b.s
            }));
            var compiled = (Func<IEnumerable<int>, int>) lambda.Compile();
            Assert.AreEqual(Enumerable.Range(0, 6).Sum(), compiled(Enumerable.Range(0, 6)));
        }

        [Test]
        public void TestForEachEnumerable()
        {
            // TODO: This is not a C# behavior, C# will throw?!
            var lambda = Lambda(p => Block(b => new[]
            {
                b.s = 0,
                ForEach(typeof(int), () => p(typeof(IEnumerable), "c"), l => b.s += l.Current),
                b.s
            }));
            var compiled = (Func<IEnumerable, int>) lambda.Compile();
            var list = new List<object>(Enumerable.Range(0, 6).Cast<object>());
            list.Insert(2, "test");
            list.Insert(4, 2.3);
            list.Add(null);
            Assert.AreEqual(Enumerable.Range(0, 6).Sum(), compiled(list));
        }

        [Test]
        public void TestForEachList()
        {
            var lambda = Lambda(p => Block(b => new[]
            {
                b.s = 0,
                ForEach(() => p(typeof(List<int>), "c"), l => b.s += l.Current),
                b.s
            }));
            var compiled = (Func<List<int>, int>) lambda.Compile();
            Assert.AreEqual(Enumerable.Range(0, 6).Sum(), compiled(Enumerable.Range(0, 6).ToList()));
        }

        [Test]
        public void TestForEachListAsEnumerable()
        {
            var lambda = Lambda(p => Block(b => new[]
            {
                b.s = 0,
                ForEach(typeof(int), () => p(typeof(List<object>), "c"), l => b.s += l.Current),
                b.s
            }));
            var compiled = (Func<List<object>, int>) lambda.Compile();
            var list = new List<object>(Enumerable.Range(0, 6).Cast<object>());
            list.Insert(2, "test");
            list.Insert(4, 2.3);
            list.Add(null);
            Assert.AreEqual(Enumerable.Range(0, 6).Sum(), compiled(list));
        }

        [Test]
        public void TestForEachStatic()
        {
            var lambda = Lambda(p => Block(b => new[]
            {
                b.s = 0,
                ForEach(() => new[] {0, 1, 2, 3, 4, 5}, l => b.s += l.Current),
                b.s
            }));
            var compiled = (Func<int>) lambda.Compile();
            Assert.AreEqual(Enumerable.Range(0, 6).Sum(), compiled());
        }

        [Test]
        public void TestIntParse()
        {
            var _int = typeof(int).EBuilder();
            var lambda = Lambda(p => _int.Parse(p(typeof(string), "s")));
            var compiled = lambda.CompileAs<Func<string, int>>();
            Assert.AreEqual(2, compiled("2"));
            Assert.AreEqual(-2, compiled("-2"));
            Assert.AreEqual(20, compiled("20"));
        }

        [Test]
        public void TestLoop()
        {
            var lambda = Lambda(p =>
            {
                p(typeof(int), "c");
                return Block(b => new[]
                {
                    b.s = 0,
                    b.i = p.c,
                    Loop(l => Block(b2 => new[]
                    {
                        If(b.i <= 0, l.Break(b.s)),
                        b.s += b.i,
                        --b.i
                    }))
                });
            });
            var compiled = (Func<int, int>) lambda.Compile();
            Assert.AreEqual(Enumerable.Range(0, 6).Sum(), compiled(5));
        }

        [Test]
        public void TestSwitch()
        {
            var lambda = Lambda(p => Switch(p(typeof(int), "a"), new[]
            {
                Case(1, "One"),
                Case(2, "Two"),
                Case(new object[] {3, 4, 5, 6, 7, 8, 9}, "Many")
            }, "Other"));
            Assert.AreEqual(@"(Int32 a) => switch (a){
  case 1: 
    ""One"";
  case 2: 
    ""Two"";
  case 3: 
  case 4: 
  case 5: 
  case 6: 
  case 7: 
  case 8: 
  case 9: 
    ""Many"";
  default:
    ""Other""
}", lambda.ToDebugString());
            var compiled = (Func<int, string>) lambda.Compile();
            Assert.AreEqual("One", compiled(1));
            Assert.AreEqual("Two", compiled(2));
            Assert.AreEqual("Many", compiled(3));
            Assert.AreEqual("Many", compiled(8));
            Assert.AreEqual("Other", compiled(11));
        }

        [Test]
        public void TestUsing()
        {
            var lambda = Lambda(p => Using(p(typeof(IDisposable), "a"), 2));
            var compiled = (Func<IDisposable, int>) lambda.Compile();
            var disposed = false;
            var dis = DisposableEx.Action(() => disposed = true);
            Assert.AreEqual(2, compiled(dis));
            Assert.IsTrue(disposed);
        }

        [Test]
        public void TestWhile()
        {
            var lambda = Lambda(p => Block(b => new[]
            {
                b.s = 0,
                b.i = p(typeof(int), "c"),
                While(() => b.i > 0, l => Block(b2 => new[]
                {
                    b.s += b.i,
                    --b.i
                })),
                b.s
            }));
            var compiled = (Func<int, int>) lambda.Compile();
            Assert.AreEqual(Enumerable.Range(0, 6).Sum(), compiled(5));
        }
    }
}