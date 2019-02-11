using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit;

namespace SimplyFast.Reflection.Tests
{
    
    public class MemberInfoExTests
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "NotAccessedField.Global")]
        [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public class Test
        {
            public readonly int F1 = 1;
            public const int F2 = 2;
            public int F3 = 3;
            public int P1 => 2;
            public int P2 { set { F3 = value; } }
            public int P3 { get { return P1; } protected set { P2 = value; } }
            public int M1()
            {
                return 1;
            }

            [SuppressMessage("ReSharper", "UnusedParameter.Global")]
            public void SetM1(int value) { }
        }

        [Fact]
        public void CanReadWorksWithPrivateAccess()
        {
            var pa = MemberInfoEx.PrivateAccess;
            MemberInfoEx.PrivateAccess = true;
            try
            {
                CanReadWorks();
            }
            finally
            {
                MemberInfoEx.PrivateAccess = pa;
            }
        }

        [Fact]
        public void CanWriteWorks()
        {
            var t = typeof (Test);
            Assert.False(MemberInfoEx.CanWrite(t.Field("F1")));
            Assert.False(MemberInfoEx.CanWrite(t.Field("F2")));
            Assert.True(MemberInfoEx.CanWrite(t.Field("F3")));
            Assert.False(t.Property("P1").CanWrite());
            Assert.True(t.Property("P2").CanWrite());
            Assert.True(t.Property("P3").CanWrite());
            Assert.False(t.Method("M1").CanWrite());
            Assert.False(t.Method("SetM1").CanWrite());
        }

        [Fact]
        public void CanReadWorks()
        {
            var t = typeof(Test);
            Assert.True(t.Field("F1").CanRead());
            Assert.True(t.Field("F2").CanRead());
            Assert.True(t.Field("F3").CanRead());
            Assert.True(t.Property("P1").CanRead());
            Assert.False(t.Property("P2").CanRead());
            Assert.True(t.Property("P3").CanRead());
            Assert.False(t.Method("M1").CanRead());
            Assert.False(t.Method("SetM1").CanRead());
        }

        [Fact]
        public void FieldOrPropertyWorks()
        {
            var t = typeof(Test);
            Assert.Equal("F1", t.FieldOrProperty("F1").Name);
            Assert.Equal("F2", t.FieldOrProperty("F2").Name);
            Assert.Equal("F3", t.FieldOrProperty("F3").Name);
            Assert.Equal("P1", t.FieldOrProperty("P1").Name);
            Assert.Equal("P2", t.FieldOrProperty("P2").Name);
            Assert.Equal("P3", t.FieldOrProperty("P3").Name);
            Assert.Null(t.FieldOrProperty("M1"));
            Assert.Null(t.FieldOrProperty("SetM1"));
            Assert.Null(t.FieldOrProperty("Foo"));
        }

        [Fact]
        public void ValueTypeWorks()
        {
            var t = typeof(Test);
            Assert.Equal(typeof(int), t.FieldOrProperty("F1").ValueType());
            Assert.Equal(typeof(int), t.FieldOrProperty("F2").ValueType());
            Assert.Equal(typeof(int), t.FieldOrProperty("F3").ValueType());
            Assert.Equal(typeof(int), t.FieldOrProperty("P1").ValueType());
            Assert.Equal(typeof(int), t.FieldOrProperty("P2").ValueType());
            Assert.Equal(typeof(int), t.FieldOrProperty("P3").ValueType());
            Assert.Throws<ArgumentException>(() =>t.Method("M1").ValueType());
            Assert.Throws<ArgumentException>(() => t.Method("SetM1").ValueType());
        }

        [SuppressMessage("ReSharper", "ValueParameterNotUsed")]
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
#pragma warning disable 169
#pragma warning disable 649
        private class TestInvokable
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
            public Func<int> Four => null;
            public Func<int> Five { set { } }
            public int Six;
            public string Seven => null;

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
#pragma warning restore 649
#pragma warning restore 169

        [Fact]
        public void FindInvokableMemberWorks()
        {
            var t = typeof (TestInvokable);
            Assert.Equal(typeof(int), ((MethodInfo)t.FindInvokableMember("One")).ReturnType);
            Assert.Equal(typeof(void), ((MethodInfo)t.FindInvokableMember("One", typeof(int))).ReturnType);
            Assert.Equal(typeof(string), ((MethodInfo)t.FindInvokableMember("One", typeof(string))).ReturnType);
            Assert.Equal(typeof(Action<int>), t.FindInvokableMember("Two", typeof(int)).ValueType());
            Assert.Equal(typeof(Action<int, int>), t.FindInvokableMember("Three", typeof(int), typeof(int)).ValueType());
            Assert.Equal(typeof(Func<int>), t.FindInvokableMember("Four").ValueType());
            Assert.Null(t.FindInvokableMember("Five"));
            Assert.Null(t.FindInvokableMember("Six"));
            Assert.Null(t.FindInvokableMember("Seven"));
            Assert.Null(t.FindInvokableMember("Eight"));
            Assert.Equal(typeof(int), ((MethodInfo)t.FindInvokableMember("Eight", typeof(int))).ReturnType);
            Assert.Equal(typeof(double), ((MethodInfo)t.FindInvokableMember("Eight", typeof(int), typeof(int))).ReturnType);
            Assert.Equal(typeof(string), ((MethodInfo)t.FindInvokableMember("Eight", typeof(string), typeof(int))).ReturnType);


            Assert.Null(t.FindInvokableMember("One", typeof(object)));
            Assert.Null(t.FindInvokableMember("Two", typeof(int), typeof(int)));
            Assert.Null(t.FindInvokableMember("One", typeof(int), typeof(int)));
            Assert.Null(t.FindInvokableMember("Three", typeof(int), typeof(string)));
            Assert.Null(t.FindInvokableMember("Four", typeof(void)));
        }
    }
}