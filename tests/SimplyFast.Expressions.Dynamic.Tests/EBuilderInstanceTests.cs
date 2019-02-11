using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Xunit;

namespace SimplyFast.Expressions.Dynamic.Tests
{
    public class EBuilderInstanceTests
    {
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public class SomeClass
        {
            public Action<bool> SomeAction;
            public int SomeField;

            public string SomeProp
            {
                get => SomeField.ToString(CultureInfo.InvariantCulture);
                set => SomeField = int.Parse(value);
            }

            public int this[int index]
            {
                get => SomeField * index;
                set => SomeField = value / index;
            }

            public bool IsOk()
            {
                return true;
            }

            public double SomeMethod(float param)
            {
                return SomeField * param;
            }

            public static explicit operator int(SomeClass a)
            {
                return a.SomeField;
            }

            public static explicit operator SomeClass(int a)
            {
                return new SomeClass {SomeField = a};
            }
        }

        [Fact]
        public void ActionOk()
        {
            var lambda = EBuilder.Lambda(typeof(SomeClass), a =>
            {
                var eBuilder = a.EBuilder();
                return eBuilder.SomeAction(true);
            });
            Assert.Equal("(SomeClass p_0) => p_0.SomeAction(True)", lambda.ToDebugString());
        }

        [Fact]
        public void BinaryOk()
        {
            var lambda = EBuilder.Lambda(typeof(SomeClass), a =>
            {
                var eBuilder = a.EBuilder();
                return eBuilder.SomeField + 2;
            });
            Assert.Equal("(SomeClass p_0) => (p_0.SomeField + 2)", lambda.ToDebugString());
        }

        [Fact]
        public void GetSetFieldOk()
        {
            var lambda = EBuilder.Lambda(typeof(SomeClass), typeof(SomeClass), (a, b) =>
            {
                var p0 = a.EBuilder();
                var p1 = b.EBuilder();
                return p0.SomeField = p1.SomeField;
            });
            Assert.Equal("(SomeClass p_0, SomeClass p_1) => (p_0.SomeField = p_1.SomeField)", lambda.ToDebugString());
        }

        [Fact]
        public void GetSetIndexOk()
        {
            var lambda = EBuilder.Lambda(typeof(SomeClass), typeof(SomeClass), (a, b) =>
            {
                var p0 = a.EBuilder();
                var p1 = b.EBuilder();
                return p0[3] = p1[2];
            });
            Assert.Equal("(SomeClass p_0, SomeClass p_1) => (p_0.Item[3] = p_1.Item[2])", lambda.ToDebugString());
        }

        [Fact]
        public void GetSetPropertyOk()
        {
            var lambda = EBuilder.Lambda(typeof(SomeClass), typeof(SomeClass), (a, b) =>
            {
                var p0 = a.EBuilder();
                var p1 = b.EBuilder();
                return p0.SomeProp = p1.SomeProp;
            });
            Assert.Equal("(SomeClass p_0, SomeClass p_1) => (p_0.SomeProp = p_1.SomeProp)", lambda.ToDebugString());
        }

        [Fact]
        public void InvokeOk()
        {
            var lambda = EBuilder.Lambda(typeof(Func<int, bool>), typeof(SomeClass), (a, b) =>
            {
                var p0 = a.EBuilder();
                var p1 = b.EBuilder();
                return !p0(p1.SomeField);
            });
            Assert.Equal("(Func<Int32, Boolean> p_0, SomeClass p_1) => !p_0(p_1.SomeField)", lambda.ToDebugString());
        }

        [Fact]
        public void MethodOk()
        {
            var lambda = EBuilder.Lambda(typeof(SomeClass), a =>
            {
                var p = a.EBuilder();
                return p.SomeMethod(2.0f);
            });
            Assert.Equal("(SomeClass p_0) => p_0.SomeMethod(2F)", lambda.ToDebugString());
        }

        [Fact]
        public void UnaryOk()
        {
            var lambda = EBuilder.Lambda(typeof(bool), a =>
            {
                var p = a.EBuilder();
                return !p;
            });
            Assert.Equal("(Boolean p_0) => !p_0", lambda.ToDebugString());
        }

        [Fact]
        public void UnaryOk2()
        {
            var lambda = EBuilder.Lambda(typeof(SomeClass), a =>
            {
                var p = a.EBuilder();
                return --p.SomeField;
            });
            Assert.Equal("(SomeClass p_0) => --p_0.SomeField", lambda.ToDebugString());
        }
    }
}