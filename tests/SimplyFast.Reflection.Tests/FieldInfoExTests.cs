using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit;
using SimplyFast.Reflection.Tests.TestData;

namespace SimplyFast.Reflection.Tests
{
    
    public class FieldInfoExTests
    {
        [Fact]
        public void ConstGettersWorks()
        {
            var type = typeof(TestClassWithConsts);
            Func<string, Func<object>> getGetter = x => type.Field(x).GetterAs<Func<object>>();
            Assert.Equal(TestClassWithConsts.TestBool, getGetter("TestBool")());

            Assert.Equal(TestClassWithConsts.TestC, getGetter("TestC")());
            Assert.Equal(TestClassWithConsts.TestStr, getGetter("TestStr")());

            Assert.Equal(TestClassWithConsts.TestB, getGetter("TestB")());
            Assert.Equal(TestClassWithConsts.TestSbyte, getGetter("TestSbyte")());
            Assert.Equal(TestClassWithConsts.TestS, getGetter("TestS")());
            Assert.Equal(TestClassWithConsts.TestUShort, getGetter("TestUShort")());
            Assert.Equal(TestClassWithConsts.TestI, getGetter("TestI")());
            Assert.Equal(TestClassWithConsts.TestUInt, getGetter("TestUInt")());
            Assert.Equal(TestClassWithConsts.TestL, getGetter("TestL")());
            Assert.Equal(TestClassWithConsts.TestULong, getGetter("TestULong")());

            Assert.Equal(TestClassWithConsts.TestF, getGetter("TestF")());
            Assert.Equal(TestClassWithConsts.TestD, getGetter("TestD")());
            Assert.Equal(TestClassWithConsts.TestDec, getGetter("TestDec")());
            Assert.Equal(TestClassWithConsts.TestEnum, getGetter("TestEnum")());
        }

        [Fact]
        public void ConstSettersDoesNotExists()
        {
            var type = typeof(TestClassWithConsts);
            Func<string, Action<object>> getSetter = x => type.Field(x).SetterAs<Action<object>>();
            Assert.Null(getSetter("TestBool"));

            Assert.Null(getSetter("TestC"));
            Assert.Null(getSetter("TestStr"));

            Assert.Null(getSetter("TestB"));
            Assert.Null(getSetter("TestSbyte"));
            Assert.Null(getSetter("TestS"));
            Assert.Null(getSetter("TestUShort"));
            Assert.Null(getSetter("TestI"));
            Assert.Null(getSetter("TestUInt"));
            Assert.Null(getSetter("TestL"));
            Assert.Null(getSetter("TestULong"));

            Assert.Null(getSetter("TestF"));
            Assert.Null(getSetter("TestD"));
            Assert.Null(getSetter("TestDec"));
            Assert.Null(getSetter("TestEnum"));
        }

        [Fact]
        public void GetterExists()
        {
            Assert.NotNull(typeof(TestClass1).Field("_f1").GetterAs<Func<object, object>>());
            Assert.NotNull(typeof(TestClass1).Field("F2").GetterAs<Func<object, object>>());
            Assert.NotNull(typeof(TestClass2).Field("_f3").GetterAs<Func<object>>());
            Assert.NotNull(typeof(TestClass2).Field("_f3").GetterAs(typeof(Func<object>)));
        }

        [Fact]
        public void GetterThrowsIfWrongType()
        {
            var d = new TestClass1();
            Assert.Throws<InvalidCastException>(() => typeof(TestClass2).Field("_f1").GetterAs<Func<object, object>>()(d));
        }

        [Fact]
        public void GetterWorksForPrivate()
        {
            var c = new TestClass1();
            Assert.Equal(1,
                            typeof(TestClass1).Field("_f1").GetterAs<Func<TestClass1, object>>()(c));
        }

        [Fact]
        public void GetterWorksForPrivateStatic()
        {
            Assert.Equal("_f3t", typeof(TestClass2).Field("_f3").GetterAs<Func<object>>()());
        }

        [Fact]
        public void GetterWorksForPublic()
        {
            var c = new TestClass1();
            Assert.Equal("test", typeof(TestClass1).Field("F2").GetterAs<Func<TestClass1, string>>()(c));
        }

        [Fact]
        public void SetterExists()
        {
            Assert.NotNull(typeof(TestClass1).Field("_f1").SetterAs<Action<object, object>>());
            Assert.NotNull(typeof(TestClass1).Field("F2").SetterAs<Action<object, object>>());
            Assert.NotNull(typeof(TestClass2).Field("_f3").SetterAs<Action<object>>());
            Assert.NotNull(typeof(TestClass2).Field("_f3").SetterAs(typeof(Action<object>)));
        }

        [Fact]
        public void SetterThrowsIfWrongType()
        {
            var c = new TestClass1();
            var d = new TestClass2();
            Assert.Throws<InvalidCastException>(
                () => typeof(TestClass2).Field("_f1").SetterAs<Action<object, object>>()(c, 123));
            Assert.Throws<InvalidCastException>(
                () => typeof(TestClass2).Field("_f1").SetterAs<Action<object, object>>()(d, "te"));
        }

        [Fact]
        public void SetterWorksForPrivate()
        {
            var c = new TestClass1();
            typeof(TestClass1).Field("_f1").SetterAs<Action<TestClass1, int>>()(c, 2);
            Assert.Equal(2, typeof(TestClass1).Field("_f1").GetterAs<Func<TestClass1, int>>()(c));
        }

        [Fact]
        public void SetterWorksForPrivateStatic()
        {
            try
            {
                typeof(TestClass2).Field("_f3").SetterAs<Action<int>>()(123);
                Assert.Equal(123, typeof(TestClass2).Field("_f3").GetterAs<Func<object>>()());
            }
            finally
            {
                TestClass2.F3 = "_f3t";
            }
        }

        [Fact]
        public void SetterWorksForPublic()
        {
            var c = new TestClass1();
            typeof(TestClass1).Field("F2").SetterAs<Action<TestClass1, object>>()(c, "te");
            Assert.Equal("te", typeof(TestClass1).Field("F2").GetterAs<Func<object, string>>()(c));
        }

        [Fact]
        public void FieldsCacheFine()
        {
            var fields = new HashSet<FieldInfo>(typeof(TestClass1).Fields());
            Assert.True(fields.SetEquals(typeof(TestClass1).GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)));
        }

        private class ClassWithFields
        {
#pragma warning disable 169
            public int Ok = 1;
            public readonly int Read = 2;
            [SuppressMessage("ReSharper", "UnusedMember.Local")]
            public const int Constant = 3;
#pragma warning restore 169
        }

        [Fact]
        public void CanWriteTests()
        {
            var type = typeof(ClassWithFields);
            Assert.True(type.Field("Ok").CanWrite());
            Assert.False(type.Field("Read").CanWrite());
            Assert.False(type.Field("Constant").CanWrite());
        }
    }
}