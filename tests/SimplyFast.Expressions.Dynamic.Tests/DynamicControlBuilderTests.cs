using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SimplyFast.Disposables;
using static SimplyFast.Expressions.Dynamic.DynamicControlBuilder;

namespace SimplyFast.Expressions.Dynamic.Tests
{
    
    public class DynamicControlBuilderTests
    {
        [Fact]
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
            Assert.Equal(@"(Int32 a, Int32 b) => {
  Int32 t;
  (t = a);
  (a = b);
  (b = t);
}", lambda.ToDebugString());
        }


        [Fact]
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
            Assert.Equal(@"(Int32 a, Int32 b) => {
  Int32 t;
  (t = (a + b));
  t;
}", lambda.ToDebugString());
        }

        [Fact]
        public void BlockTestNoDeclaration()
        {
            var lambda = Lambda(p => Block(b => new[]
            {
                b.t = p(typeof(int), "a"),
                p.a = p(typeof(int), "b"),
                p.b = b.t
            }));
            Assert.Equal(@"(Int32 a, Int32 b) => {
  Int32 t;
  (t = a);
  (a = b);
  (b = t);
}", lambda.ToDebugString());
        }

        [Fact]
        public void IfElseTest()
        {
            var lambda = Lambda(p => If(p(typeof(int), "a") >= 0, p.a, -p.a));
            Assert.Equal("(Int32 a) => IF((a >= 0), a, -a)", lambda.ToDebugString());
            var compiled = (Func<int, int>) lambda.Compile();
            Assert.Equal(0, compiled(0));
            Assert.Equal(2, compiled(2));
            Assert.Equal(2, compiled(-2));
        }

        [Fact]
        public void IfNoElseTest()
        {
            var lambda = Lambda(p => Block(b =>
                new[]
                {
                    If(p(typeof(int), "a") >= 0, p.a = 2),
                    p.a * 3
                }));
            Assert.Equal(@"(Int32 a) => {
  IF((a >= 0), (a = 2), default(Int32));
  (a * 3);
}", lambda.ToDebugString());
            var compiled = lambda.CompileAs<Func<int, int>>();
            Assert.Equal(6, compiled(1));
            Assert.Equal(6, compiled(12));
            Assert.Equal(6, compiled(0));
            Assert.Equal(-3, compiled(-1));
            Assert.Equal(-6, compiled(-2));
        }

        [Fact]
        public void LambdaFancyTest()
        {
            var lambda = Lambda(x => x(typeof(int), "a") + x(typeof(int), "b") + x.a);
            Assert.Equal("(Int32 a, Int32 b) => ((a + b) + a)", lambda.ToDebugString());
        }

        [Fact]
        public void LambdaMultipleDeclaration()
        {
            var lambda = Lambda(p => p(typeof(int), "a") * p(typeof(int), "a"));
            Assert.Equal("(Int32 a) => (a * a)", lambda.ToDebugString());
        }

        [Fact]
        public void LambdaNotSoFancyTest()
        {
            var lambda = Lambda(x =>
            {
                x(typeof(int), "a", typeof(int), "b");
                return x.a + x.b;
            });
            Assert.Equal("(Int32 a, Int32 b) => (a + b)", lambda.ToDebugString());
        }

        [Fact]
        public void LambdaTest()
        {
            var lambda = Lambda(x => x(typeof(int), "a") + x(typeof(int), "b"));
            Assert.Equal("(Int32 a, Int32 b) => (a + b)", lambda.ToDebugString());
        }

        [Fact]
        public void CastOk()
        {
            var lambda = EBuilder.Lambda(typeof(EBuilderInstanceTests.SomeClass), a =>
            {
                var aexp = a.EBuilder();
                return Cast(aexp, typeof(object));
            });
            Assert.Equal("(SomeClass p_0) => (p_0 As Object)", lambda.ToDebugString());
        }

        [Fact]
        public void ConvertOk()
        {
            var lambda = EBuilder.Lambda(typeof(EBuilderInstanceTests.SomeClass), a =>
            {
                var aexp = a.EBuilder();
                return Convert(aexp, typeof(int));
            });
            Assert.Equal("(SomeClass p_0) => Convert(p_0 As Int32)", lambda.ToDebugString());
        }

        [Fact]
        public void ConvertToOk()
        {
            var lambda = EBuilder.Lambda(typeof(int), a =>
            {
                var aexp = a.EBuilder();
                return Convert(aexp, typeof(EBuilderInstanceTests.SomeClass));
            });
            Assert.Equal("(Int32 p_0) => Convert(p_0 As SomeClass)", lambda.ToDebugString());
        }

        [Fact]
        public void DefaultOk()
        {
            var lambda = Lambda(a => Default(typeof(int)));
            Assert.Equal("() => default(Int32)", lambda.ToDebugString());
        }

        [Fact]
        public void ForOk()
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
            Assert.Equal(Enumerable.Range(0, 6).Sum(), compiled(5));
        }

        [Fact]
        public void ForEachOk()
        {
            var lambda = Lambda(p => Block(b => new[]
            {
                b.s = 0,
                ForEach(() => p(typeof(IEnumerable<int>), "c"), l => b.s += l.Current),
                b.s
            }));
            var compiled = (Func<IEnumerable<int>, int>) lambda.Compile();
            Assert.Equal(Enumerable.Range(0, 6).Sum(), compiled(Enumerable.Range(0, 6)));
        }

        [Fact]
        public void ForEachEnumerableOk()
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
            Assert.Equal(Enumerable.Range(0, 6).Sum(), compiled(list));
        }

        [Fact]
        public void ForEachListOk()
        {
            var lambda = Lambda(p => Block(b => new[]
            {
                b.s = 0,
                ForEach(() => p(typeof(List<int>), "c"), l => b.s += l.Current),
                b.s
            }));
            var compiled = (Func<List<int>, int>) lambda.Compile();
            Assert.Equal(Enumerable.Range(0, 6).Sum(), compiled(Enumerable.Range(0, 6).ToList()));
        }

        [Fact]
        public void ForEachListAsEnumerableOk()
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
            Assert.Equal(Enumerable.Range(0, 6).Sum(), compiled(list));
        }

        [Fact]
        public void ForEachStaticOk()
        {
            var lambda = Lambda(p => Block(b => new[]
            {
                b.s = 0,
                ForEach(() => new[] {0, 1, 2, 3, 4, 5}, l => b.s += l.Current),
                b.s
            }));
            var compiled = (Func<int>) lambda.Compile();
            Assert.Equal(Enumerable.Range(0, 6).Sum(), compiled());
        }

        [Fact]
        public void IntParseOk()
        {
            var _int = typeof(int).EBuilder();
            var lambda = Lambda(p => _int.Parse(p(typeof(string), "s")));
            var compiled = lambda.CompileAs<Func<string, int>>();
            Assert.Equal(2, compiled("2"));
            Assert.Equal(-2, compiled("-2"));
            Assert.Equal(20, compiled("20"));
        }

        [Fact]
        public void LoopOk()
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
            Assert.Equal(Enumerable.Range(0, 6).Sum(), compiled(5));
        }

        [Fact]
        public void SwitchOk()
        {
            var lambda = Lambda(p => Switch(p(typeof(int), "a"), new[]
            {
                Case(1, "One"),
                Case(2, "Two"),
                Case(new object[] {3, 4, 5, 6, 7, 8, 9}, "Many")
            }, "Other"));
            Assert.Equal(@"(Int32 a) => switch (a){
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
            Assert.Equal("One", compiled(1));
            Assert.Equal("Two", compiled(2));
            Assert.Equal("Many", compiled(3));
            Assert.Equal("Many", compiled(8));
            Assert.Equal("Other", compiled(11));
        }

        [Fact]
        public void UsingOk()
        {
            var lambda = Lambda(p => Using(p(typeof(IDisposable), "a"), 2));
            var compiled = (Func<IDisposable, int>) lambda.Compile();
            var disposed = false;
            var dis = DisposableEx.Action(() => disposed = true);
            Assert.Equal(2, compiled(dis));
            Assert.True(disposed);
        }

        [Fact]
        public void WhileOk()
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
            Assert.Equal(Enumerable.Range(0, 6).Sum(), compiled(5));
        }
    }
}