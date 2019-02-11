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
        private class SomeClass
        {
            public SomeClass(int value)
            {
                SomeField = value;
            }

            public static int SomeField;
            public static string SomeProp
            {
                get => SomeField.ToString(CultureInfo.InvariantCulture);
                set => SomeField = int.Parse(value);
            }

            public static bool IsOk()
            {
                return true;
            }

            public static double SomeMethod(float param)
            {
                return SomeField * param;
            }

            #pragma warning disable 169
            public static Action<bool> SomeAction = b => {};
            #pragma warning restore 169
        }

        [Fact]
        public void GetSetFieldOk()
        {
            var lambda = EBuilder.Lambda(() =>
            {
                var testClass = typeof(SomeClass).EBuilder();
                return testClass.TestField = testClass.TestField;
            });
            Assert.Equal("() => (SomeClass.SomeField = SomeClass.SomeField)", lambda.ToDebugString());
        }

        [Fact]
        public void GetSetPropertyOk()
        {
            var lambda = EBuilder.Lambda(() =>
                {
                    var testClass = typeof(SomeClass).EBuilder();
                    return testClass.TestProp = testClass.TestProp;
            });
            Assert.Equal("() => (SomeClass.SomeProp = SomeClass.SomeProp)", lambda.ToDebugString());
        }

        [Fact]
        public void MethodOk()
        {
            var lambda = EBuilder.Lambda(() =>
            {
                var testClass = typeof(SomeClass).EBuilder();
                return testClass.TestMethod(2.0f);
            });
            Assert.Equal("() => SomeClass.SomeMethod(2F)", lambda.ToDebugString());
        }

        [Fact]
        public void ActionOk()
        {
            var lambda = EBuilder.Lambda(() =>
            {
                var testClass = typeof(SomeClass).EBuilder();
                return testClass.TestAction(true);
            });
            Assert.Equal("() => SomeClass.SomeAction(True)", lambda.ToDebugString());
        }

        [Fact]
        public void NewOk()
        {
            var lambda = EBuilder.Lambda(() =>
            {
                var testClass = typeof(SomeClass).EBuilder();
                return testClass(2);
            });
            Assert.Equal("() => new SomeClass(2)", lambda.ToDebugString());
        }

        [Fact]
        public void NewArrayOk()
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