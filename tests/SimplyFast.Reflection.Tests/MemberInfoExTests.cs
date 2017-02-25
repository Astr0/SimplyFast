using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using NUnit.Framework;

namespace SimplyFast.Reflection.Tests
{
    [TestFixture]
    public class MemberInfoExTests
    {
        public class Test
        {
            public readonly int F1 = 1;
            public const int F2 = 2;
            public int F3 = 3;
            public int P1 { get { return 2; } }
            public int P2 { set { F3 = value; } }
            public int P3 { get { return P1; } protected set { P2 = value; } }
            public int M1()
            {
                return 1;
            }

            public void SetM1(int value) { }
        }

        [Test]
        public void CanReadWorksWitoutPrivateAccess()
        {
            var pa = MemberInfoEx.PrivateAccess;
            MemberInfoEx.PrivateAccess = false;
            try
            {
                CanReadWorks();
            }
            finally
            {
                MemberInfoEx.PrivateAccess = pa;
            }
        }

        [Test]
        public void CanWriteWorks()
        {
            var t = typeof (Test);
            Assert.IsFalse(MemberInfoEx.CanWrite(t.Field("F1")));
            Assert.IsFalse(MemberInfoEx.CanWrite(t.Field("F2")));
            Assert.IsTrue(MemberInfoEx.CanWrite(t.Field("F3")));
            Assert.IsFalse(t.Property("P1").CanWrite());
            Assert.IsTrue(t.Property("P2").CanWrite());
            Assert.IsTrue(t.Property("P3").CanWrite());
            Assert.IsFalse(t.Method("M1").CanWrite());
            Assert.IsFalse(t.Method("SetM1").CanWrite());
        }

        [Test]
        public void CanReadWorks()
        {
            var t = typeof(Test);
            Assert.IsTrue(t.Field("F1").CanRead());
            Assert.IsTrue(t.Field("F2").CanRead());
            Assert.IsTrue(t.Field("F3").CanRead());
            Assert.IsTrue(t.Property("P1").CanRead());
            Assert.IsFalse(t.Property("P2").CanRead());
            Assert.IsTrue(t.Property("P3").CanRead());
            Assert.IsFalse(t.Method("M1").CanRead());
            Assert.IsFalse(t.Method("SetM1").CanRead());
        }

        [Test]
        public void FieldOrPropertyWorks()
        {
            var t = typeof(Test);
            Assert.AreEqual("F1", t.FieldOrProperty("F1").Name);
            Assert.AreEqual("F2", t.FieldOrProperty("F2").Name);
            Assert.AreEqual("F3", t.FieldOrProperty("F3").Name);
            Assert.AreEqual("P1", t.FieldOrProperty("P1").Name);
            Assert.AreEqual("P2", t.FieldOrProperty("P2").Name);
            Assert.AreEqual("P3", t.FieldOrProperty("P3").Name);
            Assert.IsNull(t.FieldOrProperty("M1"));
            Assert.IsNull(t.FieldOrProperty("SetM1"));
            Assert.IsNull(t.FieldOrProperty("Foo"));
        }

        [Test]
        public void ValueTypeWorks()
        {
            var t = typeof(Test);
            Assert.AreEqual(typeof(int), t.FieldOrProperty("F1").ValueType());
            Assert.AreEqual(typeof(int), t.FieldOrProperty("F2").ValueType());
            Assert.AreEqual(typeof(int), t.FieldOrProperty("F3").ValueType());
            Assert.AreEqual(typeof(int), t.FieldOrProperty("P1").ValueType());
            Assert.AreEqual(typeof(int), t.FieldOrProperty("P2").ValueType());
            Assert.AreEqual(typeof(int), t.FieldOrProperty("P3").ValueType());
            Assert.Throws<ArgumentException>(() =>t.Method("M1").ValueType());
            Assert.Throws<ArgumentException>(() => t.Method("SetM1").ValueType());
        }

        [SuppressMessage("ReSharper", "ValueParameterNotUsed")]
        public class TestInvokable
        {
            public int One()
            {
                return 1;
            }

            public void One(int one) { }
            public string One(string one)
            {
                return null;
            }
            public Action<int> Two;
            public Action<int, int> Three;
            public Func<int> Four { get { return null; } }
            public Func<int> Five { set { } }
            public int Six;
            public string Seven { get { return null; } }

            public int Eight<T>(T input)
            {
                return 0;
            }

            public double Eight(int one, int two)
            {
                return 0;
            }

            public string Eight<T, TSec>(T one, TSec two)
            {
                return null;
            }
        }

        [Test]
        public void FindInvokableMemberWorks()
        {
            var t = typeof (TestInvokable);
            Assert.AreEqual(typeof(int), ((MethodInfo)t.FindInvokableMember("One")).ReturnType);
            Assert.AreEqual(typeof(void), ((MethodInfo)t.FindInvokableMember("One", typeof(int))).ReturnType);
            Assert.AreEqual(typeof(string), ((MethodInfo)t.FindInvokableMember("One", typeof(string))).ReturnType);
            Assert.AreEqual(typeof(Action<int>), t.FindInvokableMember("Two", typeof(int)).ValueType());
            Assert.AreEqual(typeof(Action<int, int>), t.FindInvokableMember("Three", typeof(int), typeof(int)).ValueType());
            Assert.AreEqual(typeof(Func<int>), t.FindInvokableMember("Four").ValueType());
            Assert.IsNull(t.FindInvokableMember("Five"));
            Assert.IsNull(t.FindInvokableMember("Six"));
            Assert.IsNull(t.FindInvokableMember("Seven"));
            Assert.IsNull(t.FindInvokableMember("Eight"));
            Assert.AreEqual(typeof(int), ((MethodInfo)t.FindInvokableMember("Eight", typeof(int))).ReturnType);
            Assert.AreEqual(typeof(double), ((MethodInfo)t.FindInvokableMember("Eight", typeof(int), typeof(int))).ReturnType);
            Assert.AreEqual(typeof(string), ((MethodInfo)t.FindInvokableMember("Eight", typeof(string), typeof(int))).ReturnType);


            Assert.IsNull(t.FindInvokableMember("One", typeof(object)));
            Assert.IsNull(t.FindInvokableMember("Two", typeof(int), typeof(int)));
            Assert.IsNull(t.FindInvokableMember("One", typeof(int), typeof(int)));
            Assert.IsNull(t.FindInvokableMember("Three", typeof(int), typeof(string)));
            Assert.IsNull(t.FindInvokableMember("Four", typeof(void)));
        }
    }
}