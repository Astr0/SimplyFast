using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using SF.Reflection;
using SF.Tests.Reflection.TestData;

namespace SF.Tests.Reflection
{
    [TestFixture]
    public class FieldInfoExTests
    {
        [Test]
        public void ConstGettersWorks()
        {
            var type = typeof(TestClassWithConsts);
            Func<string, Func<object>> getGetter = x => type.Field(x).GetterAs<Func<object>>();
            Assert.AreEqual(TestClassWithConsts.TestBool, getGetter("TestBool")());

            Assert.AreEqual(TestClassWithConsts.TestC, getGetter("TestC")());
            Assert.AreEqual(TestClassWithConsts.TestStr, getGetter("TestStr")());

            Assert.AreEqual(TestClassWithConsts.TestB, getGetter("TestB")());
            Assert.AreEqual(TestClassWithConsts.TestSbyte, getGetter("TestSbyte")());
            Assert.AreEqual(TestClassWithConsts.TestS, getGetter("TestS")());
            Assert.AreEqual(TestClassWithConsts.TestUShort, getGetter("TestUShort")());
            Assert.AreEqual(TestClassWithConsts.TestI, getGetter("TestI")());
            Assert.AreEqual(TestClassWithConsts.TestUInt, getGetter("TestUInt")());
            Assert.AreEqual(TestClassWithConsts.TestL, getGetter("TestL")());
            Assert.AreEqual(TestClassWithConsts.TestULong, getGetter("TestULong")());

            Assert.AreEqual(TestClassWithConsts.TestF, getGetter("TestF")());
            Assert.AreEqual(TestClassWithConsts.TestD, getGetter("TestD")());
            Assert.AreEqual(TestClassWithConsts.TestDec, getGetter("TestDec")());
            Assert.AreEqual(TestClassWithConsts.TestEnum, getGetter("TestEnum")());
        }

        [Test]
        public void ConstSettersDoesNotExists()
        {
            var type = typeof(TestClassWithConsts);
            Func<string, Action<object>> getSetter = x => type.Field(x).SetterAs<Action<object>>();
            Assert.IsNull(getSetter("TestBool"));

            Assert.IsNull(getSetter("TestC"));
            Assert.IsNull(getSetter("TestStr"));

            Assert.IsNull(getSetter("TestB"));
            Assert.IsNull(getSetter("TestSbyte"));
            Assert.IsNull(getSetter("TestS"));
            Assert.IsNull(getSetter("TestUShort"));
            Assert.IsNull(getSetter("TestI"));
            Assert.IsNull(getSetter("TestUInt"));
            Assert.IsNull(getSetter("TestL"));
            Assert.IsNull(getSetter("TestULong"));

            Assert.IsNull(getSetter("TestF"));
            Assert.IsNull(getSetter("TestD"));
            Assert.IsNull(getSetter("TestDec"));
            Assert.IsNull(getSetter("TestEnum"));
        }

        [Test]
        public void GetterExists()
        {
            Assert.IsNotNull(typeof(TestClass1).Field("_f1").GetterAs<Func<object, object>>());
            Assert.IsNotNull(typeof(TestClass1).Field("F2").GetterAs<Func<object, object>>());
            Assert.IsNotNull(typeof(TestClass2).Field("_f3").GetterAs<Func<object>>());
        }

        [Test]
        public void GetterThrowsIfWrongType()
        {
            var d = new TestClass1();
            Assert.Throws<InvalidCastException>(() => typeof(TestClass2).Field("_f1").GetterAs<Func<object, object>>()(d));
        }

        [Test]
        public void GetterWorksForPrivate()
        {
            var c = new TestClass1();
            Assert.AreEqual(1,
                            typeof(TestClass1).Field("_f1").GetterAs<Func<TestClass1, object>>()(c));
        }

        [Test]
        public void GetterWorksForPrivateStatic()
        {
            Assert.AreEqual("_f3t", typeof(TestClass2).Field("_f3").GetterAs<Func<object>>()());
        }

        [Test]
        public void GetterWorksForPublic()
        {
            var c = new TestClass1();
            Assert.AreEqual("test", typeof(TestClass1).Field("F2").GetterAs<Func<TestClass1, string>>()(c));
        }

        [Test]
        public void SetterExists()
        {
            Assert.IsNotNull(typeof(TestClass1).Field("_f1").SetterAs<Action<object,object>>());
            Assert.IsNotNull(typeof(TestClass1).Field("F2").SetterAs<Action<object, object>>());
            Assert.IsNotNull(typeof(TestClass2).Field("_f3").SetterAs<Action<object>>());
        }

        [Test]
        public void SetterThrowsIfWrongType()
        {
            var c = new TestClass1();
            var d = new TestClass2();
            Assert.Throws<InvalidCastException>(
                () => typeof(TestClass2).Field("_f1").SetterAs<Action<object, object>>()(c, 123));
            Assert.Throws<InvalidCastException>(
                () => typeof(TestClass2).Field("_f1").SetterAs<Action<object, object>>()(d, "te"));
        }

        [Test]
        public void SetterWorksForPrivate()
        {
            var c = new TestClass1();
            typeof(TestClass1).Field("_f1").SetterAs<Action<TestClass1, int>>()(c, 2);
            Assert.AreEqual(2, typeof(TestClass1).Field("_f1").GetterAs<Func<TestClass1, int>>()(c));
        }

        [Test]
        public void SetterWorksForPrivateStatic()
        {
            try
            {
                typeof(TestClass2).Field("_f3").SetterAs<Action<int>>()(123);
                Assert.AreEqual(123, typeof(TestClass2).Field("_f3").GetterAs<Func<object>>()());
            }
            finally
            {
                TestClass2.F3 = "_f3t";
            }
        }

        [Test]
        public void SetterWorksForPublic()
        {
            var c = new TestClass1();
            typeof(TestClass1).Field("F2").SetterAs<Action<TestClass1, object>>()(c, "te");
            Assert.AreEqual("te", typeof(TestClass1).Field("F2").GetterAs<Func<object, string>>()(c));
        }

        [Test]
        public void FieldsCacheFine()
        {
            var fields = new HashSet<FieldInfo>(typeof (TestClass1).Fields());
            Assert.IsTrue(fields.SetEquals(typeof(TestClass1).GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)));
        }

        public class ClassWithFields
        {
            public int Ok = 1;
            public readonly int Read = 2; 
            public const int Constant = 3;
        }

        [Test]
        public void CanWriteTests()
        {
            var type = typeof (ClassWithFields);
            Assert.IsTrue(type.Field("Ok").CanWrite());
            Assert.IsFalse(type.Field("Read").CanWrite());
            Assert.IsFalse(type.Field("Constant").CanWrite());
        }
    }
}