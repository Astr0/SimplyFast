using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using SF.Reflection.Tests.TestData;

namespace SF.Reflection.Tests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "ValueParameterNotUsed")]
    public class PropertyInfoExTests
    {
        [Test]
        public void GetterExists()
        {
            Assert.IsNotNull(typeof (TestClass3).Property("P1").GetterAs<Func<object, object>>());
            Assert.IsNotNull(typeof(TestClass3).Property("Item", typeof(int)).GetterAs<Func<object, object, object>>());
            Assert.IsNotNull(typeof(TestClass3).Property("CanGet").GetterAs<Func<object, object>>());
            Assert.IsNotNull(typeof(TestClass3).Property("CanGet").GetterAs(typeof(Func<object, object>)));
            Assert.IsNull(typeof(TestClass3).Property("CanSet").GetterAs<Func<object, object>>());
        }

        [Test]
        public void GetterWorksForIndexed()
        {
            var c = new TestClass3 {CanSet = 2, P2 = "test"};
            var prop = typeof (TestClass3).Property("Item");
            var getter = prop.GetterAs<Func<object, int, object>>();
            Assert.IsNotNull(getter);
            Assert.AreEqual(15, getter(c, 14));
        }

        [Test]
        public void GetterWorksForPrivate()
        {
            var c = new TestClass3();
            Assert.AreEqual(0, typeof (TestClass3).Property("Priv").GetterAs<Func<object, object>>()(c));
        }

        [Test]
        public void GetterWorksForPrivateStatic()
        {
            Assert.AreEqual(0, typeof (TestClass3).Property("Priv2").GetterAs<Func<object>>()());
        }

        [Test]
        public void GetterWorksForPublic()
        {
            var c = new TestClass3 {CanSet = 2, P2 = "test"};
            Assert.AreEqual("test", typeof (TestClass3).Property("P2").GetterAs<Func<object, string>>()(c));
            Assert.AreEqual(2, typeof (TestClass3).Property("CanGet").GetterAs<Func<object, object>>()(c));
        }

        [Test]
        public void SetterExists()
        {
            Assert.IsNotNull(typeof (TestClass3).Property("P1").SetterAs<Action<object, object>>());
            Assert.IsNotNull(typeof(TestClass3).Property("Item", typeof(int)).SetterAs<Action<object, object, object>>());
            Assert.IsNull(typeof(TestClass3).Property("CanGet").SetterAs<Action<object, object>>());
            Assert.IsNotNull(typeof(TestClass3).Property("CanSet").SetterAs<Action<object, object>>());
            Assert.IsNotNull(typeof(TestClass3).Property("CanSet").SetterAs(typeof(Action<object, object>)));
        }

        [Test]
        public void SetterThrowsIfWrongType()
        {
            var c = new TestClass3 {CanSet = 2, P2 = "test"};

            Assert.Throws<NullReferenceException>(
                () => typeof (TestClass3).Property("CanSet").SetterAs<Action<TestClass3, object>>()(c, null));
            Assert.Throws<InvalidCastException>(
                () => typeof (TestClass3).Property("P2").SetterAs<Action<TestClass3, object>>()(c, 2.5));
        }

        [Test]
        public void SetterWorksForIndexed()
        {
            var c = new TestClass3 {CanSet = 2, P2 = "test"};
            var prop = typeof (TestClass3).Property("Item");
            var setter = prop.SetterAs<Action<object, int, int>>();
            Assert.IsNotNull(setter);
            setter(c, 4, 3);
            Assert.AreEqual(12, c.CanGet);
        }

        [Test]
        public void SetterWorksForPrivate()
        {
            var c = new TestClass3();
            typeof (TestClass3).Property("Priv").SetterAs<Action<TestClass3, int>>()(c, 5);
            Assert.AreEqual(5, typeof (TestClass3).Property("Priv").GetterAs<Func<TestClass3, object>>()(c));
        }

        [Test]
        public void SetterWorksForPrivateStatic()
        {
            typeof (TestClass3).Property("Priv2").SetterAs<Action<int>>()(5);
            Assert.AreEqual(5, typeof (TestClass3).Property("Priv2").GetterAs<Func<int>>()());
        }

        [Test]
        public void SetterWorksForPublic()
        {
            var c = new TestClass3 {CanSet = 2, P2 = "test"};
            typeof (TestClass3).Property("P2").SetterAs<Action<object, string>>()(c, "test1");
            typeof (TestClass3).Property("CanSet").SetterAs<Action<object, int>>()(c, 5);
            Assert.AreEqual("test1", c.P2);
            Assert.AreEqual(5, c.CanGet);
        }

        [Test]
        public void IsStaticWorks()
        {
            Assert.IsTrue(typeof(TestClass3).Property("Priv2").IsStatic());
            Assert.IsFalse(typeof(TestClass3).Property("Priv").IsStatic());
        }

        [Test]
        public void IsPublicWorks()
        {
            Assert.IsTrue(typeof(TestClass3).Property("CanGet").IsPublic());
            Assert.IsTrue(typeof(TestClass3).Property("CanSet").IsPublic());
            Assert.IsTrue(typeof(TestClass3).Property("P1").IsPublic());
            Assert.IsFalse(typeof(TestClass3).Property("Priv").IsPublic());
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private class MultiIndexed
        {
            public int this[int index] { get { return 0; } set{} }
            public int this[string index] { get { return 0; } set { } }
            public int this[int i, string s] { get { return 0; } set { } }
        }

        [Test]
        public void GetIndexedWorks()
        {
            Assert.IsNotNull(typeof(MultiIndexed).Property("Item", typeof(int)));
            Assert.IsNotNull(typeof(MultiIndexed).Property("Item", typeof(string)));
            Assert.IsNotNull(typeof(MultiIndexed).Property("Item", typeof(int), typeof(string)));
            Assert.AreEqual(3, typeof(MultiIndexed).Properties("Item").Length);
        }

        [Test]
        public void GetDefaultIndexerWorks()
        {
            Assert.IsNotNull(typeof(MultiIndexed).Indexer(typeof(int)));
            Assert.IsNotNull(typeof(MultiIndexed).Indexer(typeof(string)));
            Assert.IsNotNull(typeof(MultiIndexed).Indexer(typeof(int), typeof(string)));
        }

        [Test]
        public void PropertyFromMethodWorks()
        {
            var prop = typeof (MultiIndexed).Property("Item", typeof (int));
            var method = prop.GetGetMethod();
            Assert.AreEqual(prop, PropertyInfoEx.Property(method));
            Assert.IsNull(PropertyInfoEx.Property(typeof(PropertyInfoExTests).GetMethod("PropertyFromMethodWorks")));
        }
    }
}