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
        public void ExactOk()
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
        public void CastOk()
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
        private static void ConvertOk<TSource, TResult>(TResult expected, TSource value)
        {
            Assert.Equal(expected, ConvertEx.Converter<TSource, TResult>()(value));
        }

        [Fact]
        public void ToObjectOk()
        {
            ConvertOk((object)2, 2);
            ConvertOk((object)"2", "2");
            ConvertOk((object)null, (string)null);
        }

        [Fact]
        public void ObjectOk()
        {
            ConvertOk(2, (object)2);
            ConvertOk("2", (object)2);
            ConvertOk("2", (object)"2");
            ConvertOk(2, (object)"2");
            ConvertOk(2L, (object)"2");
            ConvertOk(2.0, (object)"2");
            ConvertOk(2.0d, (object)"2");
            ConvertOk(true, (object)1);
            ConvertOk(false, (object)0);
        }

        [Fact]
        public void BuildInOk()
        {
            ConvertOk(2, "2");
            ConvertOk("2", 2);
            ConvertOk(2.0, 2);
            ConvertOk(2, 2.0);
            ConvertOk(2.0d, 2);
            ConvertOk(2, 2.0d);
            ConvertOk(true, 1);
            ConvertOk(1, true);
            ConvertOk(false, 0);
            ConvertOk(0, false);
        }
    }
}