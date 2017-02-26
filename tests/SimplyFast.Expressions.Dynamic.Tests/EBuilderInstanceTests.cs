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
        public class TestClass
        {
            public Action<bool> TestAction;
            public int TestField;

            public string TestProp
            {
                get { return TestField.ToString(CultureInfo.InvariantCulture); }
                set { TestField = int.Parse(value); }
            }

            public int this[int index]
            {
                get { return TestField * index; }
                set { TestField = value / index; }
            }

            public bool IsOk()
            {
                return true;
            }

            public double TestMethod(float param)
            {
                return TestField * param;
            }

            public static explicit operator int(TestClass a)
            {
                return a.TestField;
            }

            public static explicit operator TestClass(int a)
            {
                return new TestClass {TestField = a};
            }
        }

        [Fact]
        public void TestAction()
        {
            var lambda = EBuilder.Lambda(typeof(TestClass), a =>
            {
                var aexp = a.EBuilder();
                return aexp.TestAction(true);
            });
            Assert.Equal("(TestClass p_0) => p_0.TestAction(True)", lambda.ToDebugString());
        }

        [Fact]
        public void TestBinary()
        {
            var lambda = EBuilder.Lambda(typeof(TestClass), a =>
            {
                var aexp = a.EBuilder();
                return aexp.TestField + 2;
            });
            Assert.Equal("(TestClass p_0) => (p_0.TestField + 2)", lambda.ToDebugString());
        }

        [Fact]
        public void TestGetSetField()
        {
            var lambda = EBuilder.Lambda(typeof(TestClass), typeof(TestClass), (a, b) =>
            {
                var aexp = a.EBuilder();
                var bexp = b.EBuilder();
                return aexp.TestField = bexp.TestField;
            });
            Assert.Equal("(TestClass p_0, TestClass p_1) => (p_0.TestField = p_1.TestField)", lambda.ToDebugString());
        }

        [Fact]
        public void TestGetSetIndex()
        {
            var lambda = EBuilder.Lambda(typeof(TestClass), typeof(TestClass), (a, b) =>
            {
                var aexp = a.EBuilder();
                var bexp = b.EBuilder();
                return aexp[3] = bexp[2];
            });
            Assert.Equal("(TestClass p_0, TestClass p_1) => (p_0.Item[3] = p_1.Item[2])", lambda.ToDebugString());
        }

        [Fact]
        public void TestGetSetProperty()
        {
            var lambda = EBuilder.Lambda(typeof(TestClass), typeof(TestClass), (a, b) =>
            {
                var aexp = a.EBuilder();
                var bexp = b.EBuilder();
                return aexp.TestProp = bexp.TestProp;
            });
            Assert.Equal("(TestClass p_0, TestClass p_1) => (p_0.TestProp = p_1.TestProp)", lambda.ToDebugString());
        }

        [Fact]
        public void TestInvoke()
        {
            var lambda = EBuilder.Lambda(typeof(Func<int, bool>), typeof(TestClass), (a, b) =>
            {
                var aexp = a.EBuilder();
                var bexp = b.EBuilder();
                return !aexp(bexp.TestField);
            });
            Assert.Equal("(Func<Int32, Boolean> p_0, TestClass p_1) => !p_0(p_1.TestField)", lambda.ToDebugString());
        }

        [Fact]
        public void TestMethod()
        {
            var lambda = EBuilder.Lambda(typeof(TestClass), a =>
            {
                var aexp = a.EBuilder();
                return aexp.TestMethod(2.0f);
            });
            Assert.Equal("(TestClass p_0) => p_0.TestMethod(2F)", lambda.ToDebugString());
        }

        [Fact]
        public void TestUnary()
        {
            var lambda = EBuilder.Lambda(typeof(bool), a =>
            {
                var aexp = a.EBuilder();
                return !aexp;
            });
            Assert.Equal("(Boolean p_0) => !p_0", lambda.ToDebugString());
        }

        [Fact]
        public void TestUnary2()
        {
            var lambda = EBuilder.Lambda(typeof(TestClass), a =>
            {
                var aexp = a.EBuilder();
                return --aexp.TestField;
            });
            Assert.Equal("(TestClass p_0) => --p_0.TestField", lambda.ToDebugString());
        }
    }
}