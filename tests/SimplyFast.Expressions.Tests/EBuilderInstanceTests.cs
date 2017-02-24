using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using NUnit.Framework;
using SimplyFast.Expressions;
using SimplyFast.Expressions.Dynamic;

namespace SF.Tests
{
    [TestFixture]
    public class EBuilderInstanceTests
    {
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public class TestClass
        {
            public int TestField;
            public string TestProp
            {
                get { return TestField.ToString(CultureInfo.InvariantCulture); }
                set { TestField = int.Parse(value); }
            }

            public bool IsOk()
            {
                return true;
            }

            public double TestMethod(float param)
            {
                return TestField*param;
            }

            public Action<bool> TestAction;

            public int this[int index]
            {
                get { return TestField*index; }
                set { TestField = value/index; }
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

        [Test]
        public void TestGetSetField()
        {
            var lambda = EBuilder.Lambda(typeof (TestClass), typeof (TestClass), (a, b) =>
                {
                    var aexp = a.EBuilder();
                    var bexp = b.EBuilder();
                    return aexp.TestField = bexp.TestField;
                });
            Assert.AreEqual("(TestClass p_0, TestClass p_1) => (p_0.TestField = p_1.TestField)", lambda.ToDebugString());
        }

        [Test]
        public void TestGetSetProperty()
        {
            var lambda = EBuilder.Lambda(typeof(TestClass), typeof(TestClass), (a, b) =>
            {
                var aexp = a.EBuilder();
                var bexp = b.EBuilder();
                return aexp.TestProp = bexp.TestProp;
            });
            Assert.AreEqual("(TestClass p_0, TestClass p_1) => (p_0.TestProp = p_1.TestProp)", lambda.ToDebugString());
        }

        [Test]
        public void TestGetSetIndex()
        {
            var lambda = EBuilder.Lambda(typeof(TestClass), typeof(TestClass), (a, b) =>
            {
                var aexp = a.EBuilder();
                var bexp = b.EBuilder();
                return aexp[3] = bexp[2];
            });
            Assert.AreEqual("(TestClass p_0, TestClass p_1) => (p_0.Item[3] = p_1.Item[2])", lambda.ToDebugString());            
        }

        [Test]
        public void TestMethod()
        {
            var lambda = EBuilder.Lambda(typeof(TestClass), a =>
            {
                var aexp = a.EBuilder();
                return aexp.TestMethod(2.0f);
            });
            Assert.AreEqual("(TestClass p_0) => p_0.TestMethod(2F)", lambda.ToDebugString());
        }

        [Test]
        public void TestAction()
        {
            var lambda = EBuilder.Lambda(typeof(TestClass), a =>
            {
                var aexp = a.EBuilder();
                return aexp.TestAction(true);
            });
            Assert.AreEqual("(TestClass p_0) => p_0.TestAction(True)", lambda.ToDebugString());
        }

        [Test]
        public void TestBinary()
        {
            var lambda = EBuilder.Lambda(typeof(TestClass), a =>
            {
                var aexp = a.EBuilder();
                return aexp.TestField + 2;
            });
            Assert.AreEqual("(TestClass p_0) => (p_0.TestField + 2)", lambda.ToDebugString());
        }
        
        [Test]
        public void TestUnary()
        {
            var lambda = EBuilder.Lambda(typeof(bool), a =>
            {
                var aexp = a.EBuilder();
                return !aexp;
            });
            Assert.AreEqual("(Boolean p_0) => !p_0", lambda.ToDebugString());
        }

        [Test]
        public void TestUnary2()
        {
            var lambda = EBuilder.Lambda(typeof(TestClass), a =>
            {
                var aexp = a.EBuilder();
                return --aexp.TestField;
            });
            Assert.AreEqual("(TestClass p_0) => --p_0.TestField", lambda.ToDebugString());
        }

        [Test]
        public void TestInvoke()
        {
            var lambda = EBuilder.Lambda(typeof(Func<int, bool>), typeof(TestClass), (a, b) =>
            {
                var aexp = a.EBuilder();
                var bexp = b.EBuilder();
                return !aexp(bexp.TestField);
            });
            Assert.AreEqual("(Func<Int32, Boolean> p_0, TestClass p_1) => !p_0(p_1.TestField)", lambda.ToDebugString());
        }
    }
}