using NUnit.Framework;
using SF.Reflection;

namespace SF.Tests.Reflection
{
    [TestFixture]
    public class ConvertExTests
    {
        private class Convert1
        {
            public string A;
        }

        private class Convert2 : Convert1
        {
            public string B;
        }

        [Test]
        public void TestExact()
        {
            var convert = ConvertEx.Converter<int, int>();
            Assert.AreEqual(2, convert(2));
            Assert.AreEqual(0, convert(0));
            Assert.AreEqual(5, convert(5));
            var c1 = new Convert1 { A = "2" };
            var convertClass = ConvertEx.Converter<Convert1, Convert1>();
            Assert.AreEqual(c1, convertClass(c1));
            Assert.IsTrue(ReferenceEquals(c1, convertClass(c1)));
            var c2 = new Convert2 { A = "2" };
            Assert.AreEqual(c2, convertClass(c2));
            Assert.IsTrue(ReferenceEquals(c2, convertClass(c2)));
            var convertClass2 = ConvertEx.Converter<Convert2, Convert2>();
            Assert.AreEqual(c2, convertClass(c2));
            Assert.IsTrue(ReferenceEquals(c2, convertClass(c2)));
            Assert.IsNull(convertClass(null));
            Assert.IsNull(convertClass2(null));
        }

        [Test]
        public void TestCast()
        {
            var convert = ConvertEx.Converter<int, object>();
            Assert.AreEqual(2, convert(2));
            Assert.AreEqual(0, convert(0));
            Assert.AreEqual(5, convert(5));
            var c2 = new Convert2 { A = "2" };
            var convertClass = ConvertEx.Converter<Convert2, Convert1>();
            Assert.AreEqual(c2, convertClass(c2));
            Assert.IsTrue(ReferenceEquals(c2, convertClass(c2)));
            Assert.IsNull(convertClass(null));
        }

        private static void TestConvert<TSource, TResult>(TResult expected, TSource value)
        {
            Assert.AreEqual(expected, ConvertEx.Converter<TSource, TResult>()(value));
        }

        [Test]
        public void TestToObject()
        {
            TestConvert((object)2, 2);
            TestConvert((object)"2", "2");
            TestConvert((object)null, (string)null);
        }

        [Test]
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

        [Test]
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