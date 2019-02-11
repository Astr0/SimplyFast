using System.Diagnostics.CodeAnalysis;
using Xunit;

#pragma warning disable 414
#pragma warning disable 169
#pragma warning disable 649

namespace SimplyFast.Reflection.Tests
{
    
    public class ConvertExTests
    {
        private class Convert1
        {
            [SuppressMessage("ReSharper", "NotAccessedField.Local")] 
            public string A;
        }

        private class Convert2 : Convert1
        {
            public string B;
        }

        [Fact]
        public void TestExact()
        {
            var convert = ConvertEx.Converter<int, int>();
            Assert.Equal(2, convert(2));
            Assert.Equal(0, convert(0));
            Assert.Equal(5, convert(5));
            var c1 = new Convert1 { A = "2" };
            var convertClass = ConvertEx.Converter<Convert1, Convert1>();
            Assert.Equal(c1, convertClass(c1));
            Assert.True(ReferenceEquals(c1, convertClass(c1)));
            var c2 = new Convert2 { A = "2" };
            Assert.Equal(c2, convertClass(c2));
            Assert.True(ReferenceEquals(c2, convertClass(c2)));
            var convertClass2 = ConvertEx.Converter<Convert2, Convert2>();
            Assert.Equal(c2, convertClass(c2));
            Assert.True(ReferenceEquals(c2, convertClass(c2)));
            Assert.Null(convertClass(null));
            Assert.Null(convertClass2(null));
        }

        [Fact]
        public void TestCast()
        {
            var convert = ConvertEx.Converter<int, object>();
            Assert.Equal(2, convert(2));
            Assert.Equal(0, convert(0));
            Assert.Equal(5, convert(5));
            var c2 = new Convert2 { A = "2" };
            var convertClass = ConvertEx.Converter<Convert2, Convert1>();
            Assert.Equal(c2, convertClass(c2));
            Assert.True(ReferenceEquals(c2, convertClass(c2)));
            Assert.Null(convertClass(null));
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
        private static void TestConvert<TSource, TResult>(TResult expected, TSource value)
        {
            Assert.Equal(expected, ConvertEx.Converter<TSource, TResult>()(value));
        }

        [Fact]
        public void TestToObject()
        {
            TestConvert((object)2, 2);
            TestConvert((object)"2", "2");
            TestConvert((object)null, (string)null);
        }

        [Fact]
        public void TestObject()
        {
            TestConvert(2, (object)2);
            TestConvert("2", (object)2);
            TestConvert("2", (object)"2");
            TestConvert(2, (object)"2");
            TestConvert(2L, (object)"2");
            TestConvert(2.0, (object)"2");
            TestConvert(2.0d, (object)"2");
            TestConvert(true, (object)1);
            TestConvert(false, (object)0);
        }

        [Fact]
        public void TestBuildIn()
        {
            TestConvert(2, "2");
            TestConvert("2", 2);
            TestConvert(2.0, 2);
            TestConvert(2, 2.0);
            TestConvert(2.0d, 2);
            TestConvert(2, 2.0d);
            TestConvert(true, 1);
            TestConvert(1, true);
            TestConvert(false, 0);
            TestConvert(0, false);
        }
    }
}