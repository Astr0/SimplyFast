using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit;
using SimplyFast.Reflection.Tests.TestData;

namespace SimplyFast.Reflection.Tests
{
    
    [SuppressMessage("ReSharper", "ValueParameterNotUsed")]
    public class PropertyInfoExTests
    {
        [Fact]
        public void GetterExists()
        {
            Assert.NotNull(typeof (SomeClass3).Property("P1").GetterAs<Func<object, object>>());
            Assert.NotNull(typeof(SomeClass3).Property("Item", typeof(int)).GetterAs<Func<object, object, object>>());
            Assert.NotNull(typeof(SomeClass3).Property("CanGet").GetterAs<Func<object, object>>());
            Assert.NotNull(typeof(SomeClass3).Property("CanGet").GetterAs(typeof(Func<object, object>)));
            Assert.Null(typeof(SomeClass3).Property("CanSet").GetterAs<Func<object, object>>());
        }

        [Fact]
        public void GetterWorksForIndexed()
        {
            var c = new SomeClass3 {CanSet = 2, P2 = "test"};
            var prop = typeof (SomeClass3).Property("Item");
            var getter = prop.GetterAs<Func<object, int, object>>();
            Assert.NotNull(getter);
            Assert.Equal(15, getter(c, 14));
        }

        [Fact]
        public void GetterWorksForPrivate()
        {
            var c = new SomeClass3();
            Assert.Equal(0, typeof (SomeClass3).Property("Priv").GetterAs<Func<object, object>>()(c));
        }

        private class SomePrivateStatic
        {
            [SuppressMessage("ReSharper", "UnusedMember.Local")]
            private static int Priv => 11;
        }

        [Fact]
        public void GetterWorksForPrivateStatic()
        {
            Assert.Equal(11, typeof (SomePrivateStatic).Property("Priv").GetterAs<Func<object>>()());
        }

        [Fact]
        public void GetterWorksForPublic()
        {
            var c = new SomeClass3 {CanSet = 2, P2 = "test"};
            Assert.Equal("test", typeof (SomeClass3).Property("P2").GetterAs<Func<object, string>>()(c));
            Assert.Equal(2, typeof (SomeClass3).Property("CanGet").GetterAs<Func<object, object>>()(c));
        }

        [Fact]
        public void SetterExists()
        {
            Assert.NotNull(typeof (SomeClass3).Property("P1").SetterAs<Action<object, object>>());
            Assert.NotNull(typeof(SomeClass3).Property("Item", typeof(int)).SetterAs<Action<object, object, object>>());
            Assert.Null(typeof(SomeClass3).Property("CanGet").SetterAs<Action<object, object>>());
            Assert.NotNull(typeof(SomeClass3).Property("CanSet").SetterAs<Action<object, object>>());
            Assert.NotNull(typeof(SomeClass3).Property("CanSet").SetterAs(typeof(Action<object, object>)));
        }

        [Fact]
        public void SetterThrowsIfWrongType()
        {
            var c = new SomeClass3 {CanSet = 2, P2 = "test"};

            Assert.Throws<NullReferenceException>(
                () => typeof (SomeClass3).Property("CanSet").SetterAs<Action<SomeClass3, object>>()(c, null));
            Assert.Throws<InvalidCastException>(
                () => typeof (SomeClass3).Property("P2").SetterAs<Action<SomeClass3, object>>()(c, 2.5));
        }

        [Fact]
        public void SetterWorksForIndexed()
        {
            var c = new SomeClass3 {CanSet = 2, P2 = "test"};
            var prop = typeof (SomeClass3).Property("Item");
            var setter = prop.SetterAs<Action<object, int, int>>();
            Assert.NotNull(setter);
            setter(c, 4, 3);
            Assert.Equal(12, c.CanGet);
        }

        [Fact]
        public void SetterWorksForPrivate()
        {
            var c = new SomeClass3();
            typeof (SomeClass3).Property("Priv").SetterAs<Action<SomeClass3, int>>()(c, 5);
            Assert.Equal(5, typeof (SomeClass3).Property("Priv").GetterAs<Func<SomeClass3, object>>()(c));
        }

        [Fact]
        public void SetterWorksForPrivateStatic()
        {
            typeof (SomeClass3).Property("Priv2").SetterAs<Action<int>>()(5);
            Assert.Equal(5, typeof (SomeClass3).Property("Priv2").GetterAs<Func<int>>()());
        }

        [Fact]
        public void SetterWorksForPublic()
        {
            var c = new SomeClass3 {CanSet = 2, P2 = "test"};
            typeof (SomeClass3).Property("P2").SetterAs<Action<object, string>>()(c, "test1");
            typeof (SomeClass3).Property("CanSet").SetterAs<Action<object, int>>()(c, 5);
            Assert.Equal("test1", c.P2);
            Assert.Equal(5, c.CanGet);
        }

        [Fact]
        public void IsStaticWorks()
        {
            Assert.True(typeof(SomeClass3).Property("Priv2").IsStatic());
            Assert.False(typeof(SomeClass3).Property("Priv").IsStatic());
        }

        [Fact]
        public void IsPublicWorks()
        {
            Assert.True(typeof(SomeClass3).Property("CanGet").IsPublic());
            Assert.True(typeof(SomeClass3).Property("CanSet").IsPublic());
            Assert.True(typeof(SomeClass3).Property("P1").IsPublic());
            Assert.False(typeof(SomeClass3).Property("Priv").IsPublic());
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        private class SomeProtected
        {
            protected int Full { get; set; }
            protected int Get => 0;
            protected int Set { set{} }
            protected int PSet { get { return 0; } private set{}}
            protected int PGet { private get { return 0; } set { } }
        }

        [Fact]
        public void IsPrivateWorks()
        {
            Assert.False(typeof(SomeClass3).Property("CanGet").IsPrivate());
            Assert.False(typeof(SomeClass3).Property("CanSet").IsPrivate());
            Assert.False(typeof(SomeClass3).Property("P1").IsPrivate());
            Assert.True(typeof(SomeClass3).Property("Priv").IsPrivate());
            Assert.Equal(5, typeof(SomeProtected).Properties().Length);
            Assert.False(typeof(SomeProtected).Property("Full").IsPrivate());
            Assert.False(typeof(SomeProtected).Property("Get").IsPrivate());
            Assert.False(typeof(SomeProtected).Property("Set").IsPrivate());
            Assert.False(typeof(SomeProtected).Property("PSet").IsPrivate());
            Assert.False(typeof(SomeProtected).Property("PSet").IsPrivate());
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private class MultiIndexed
        {
            public int this[int index] { get { return 0; } set{} }
            public int this[string index] { get { return 0; } set { } }
            public int this[int i, string s] { get { return 0; } set { } }
        }

        [Fact]
        public void GetIndexedWorks()
        {
            Assert.NotNull(typeof(MultiIndexed).Property("Item", typeof(int)));
            Assert.NotNull(typeof(MultiIndexed).Property("Item", typeof(string)));
            Assert.NotNull(typeof(MultiIndexed).Property("Item", typeof(int), typeof(string)));
            Assert.Equal(3, typeof(MultiIndexed).Properties("Item").Length);
        }

        [Fact]
        public void GetDefaultIndexerWorks()
        {
            Assert.NotNull(typeof(MultiIndexed).Indexer(typeof(int)));
            Assert.NotNull(typeof(MultiIndexed).Indexer(typeof(string)));
            Assert.NotNull(typeof(MultiIndexed).Indexer(typeof(int), typeof(string)));
        }

        [Fact]
        public void PropertyFromMethodWorks()
        {
            var prop = typeof (MultiIndexed).Property("Item", typeof (int));
            var method = prop.GetGetMethod();
            Assert.Equal(prop, PropertyInfoEx.Property(method));
            Assert.Null(PropertyInfoEx.Property(typeof(PropertyInfoExTests).GetTypeInfo().GetMethod("PropertyFromMethodWorks")));
        }
    }
}