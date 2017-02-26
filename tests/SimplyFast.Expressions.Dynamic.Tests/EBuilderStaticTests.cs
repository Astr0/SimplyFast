using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Xunit;

namespace SimplyFast.Expressions.Dynamic.Tests
{
    
    public class EBuilderStaticTests
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private class TestClass
        {
            public TestClass(int value)
            {
                TestField = value;
            }

            public static int TestField;
            public static string TestProp
            {
                get { return TestField.ToString(CultureInfo.InvariantCulture); }
                set { TestField = int.Parse(value); }
            }

            public static bool IsOk()
            {
                return true;
            }

            public static double TestMethod(float param)
            {
                return TestField * param;
            }

            #pragma warning disable 169
            public static Action<bool> TestAction = b => {};
            #pragma warning restore 169
        }

        [Fact]
        public void TestGetSetField()
        {
            var lambda = EBuilder.Lambda(() =>
            {
                var testClass = typeof(TestClass).EBuilder();
                return testClass.TestField = testClass.TestField;
            });
            Assert.Equal("() => (TestClass.TestField = TestClass.TestField)", lambda.ToDebugString());
        }

        [Fact]
        public void TestGetSetProperty()
        {
            var lambda = EBuilder.Lambda(() =>
                {
                    var testClass = typeof(TestClass).EBuilder();
                    return testClass.TestProp = testClass.TestProp;
            });
            Assert.Equal("() => (TestClass.TestProp = TestClass.TestProp)", lambda.ToDebugString());
        }

        [Fact]
        public void TestMethod()
        {
            var lambda = EBuilder.Lambda(() =>
            {
                var testClass = typeof(TestClass).EBuilder();
                return testClass.TestMethod(2.0f);
            });
            Assert.Equal("() => TestClass.TestMethod(2F)", lambda.ToDebugString());
        }

        [Fact]
        public void TestAction()
        {
            var lambda = EBuilder.Lambda(() =>
            {
                var testClass = typeof(TestClass).EBuilder();
                return testClass.TestAction(true);
            });
            Assert.Equal("() => TestClass.TestAction(True)", lambda.ToDebugString());
        }

        [Fact]
        public void TestNew()
        {
            var lambda = EBuilder.Lambda(() =>
            {
                var testClass = typeof(TestClass).EBuilder();
                return testClass(2);
            });
            Assert.Equal("() => new TestClass(2)", lambda.ToDebugString());
        }

        [Fact]
        public void TestNewArray()
        {
            var lambda = EBuilder.Lambda(() =>
            {
                var testClass = typeof(byte[]).EBuilder();
                return testClass(2);
            });
            Assert.Equal("() => new Byte[2]", lambda.ToDebugString());
        }
    }
}