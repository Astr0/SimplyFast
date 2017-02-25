using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using NUnit.Framework;

namespace SimplyFast.Expressions.Dynamic.Tests
{
    [TestFixture]
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

        [Test]
        public void TestGetSetField()
        {
            var lambda = EBuilder.Lambda(() =>
            {
                var testClass = typeof(TestClass).EBuilder();
                return testClass.TestField = testClass.TestField;
            });
            Assert.AreEqual("() => (TestClass.TestField = TestClass.TestField)", lambda.ToDebugString());
        }

        [Test]
        public void TestGetSetProperty()
        {
            var lambda = EBuilder.Lambda(() =>
                {
                    var testClass = typeof(TestClass).EBuilder();
                    return testClass.TestProp = testClass.TestProp;
            });
            Assert.AreEqual("() => (TestClass.TestProp = TestClass.TestProp)", lambda.ToDebugString());
        }

        [Test]
        public void TestMethod()
        {
            var lambda = EBuilder.Lambda(() =>
            {
                var testClass = typeof(TestClass).EBuilder();
                return testClass.TestMethod(2.0f);
            });
            Assert.AreEqual("() => TestClass.TestMethod(2F)", lambda.ToDebugString());
        }

        [Test]
        public void TestAction()
        {
            var lambda = EBuilder.Lambda(() =>
            {
                var testClass = typeof(TestClass).EBuilder();
                return testClass.TestAction(true);
            });
            Assert.AreEqual("() => TestClass.TestAction(True)", lambda.ToDebugString());
        }

        [Test]
        public void TestNew()
        {
            var lambda = EBuilder.Lambda(() =>
            {
                var testClass = typeof(TestClass).EBuilder();
                return testClass(2);
            });
            Assert.AreEqual("() => new TestClass(2)", lambda.ToDebugString());
        }

        [Test]
        public void TestNewArray()
        {
            var lambda = EBuilder.Lambda(() =>
            {
                var testClass = typeof(byte[]).EBuilder();
                return testClass(2);
            });
            Assert.AreEqual("() => new Byte[2]", lambda.ToDebugString());
        }
    }
}