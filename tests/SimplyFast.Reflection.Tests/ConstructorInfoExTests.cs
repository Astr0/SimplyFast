using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using SimplyFast.Reflection.Tests.TestData;

namespace SimplyFast.Reflection.Tests
{
    
    public class ConstructorInfoExTests: ReflectionTests
    {

        [Fact]
        public void CanCreateObjectUsingCreateInstance()
        {
            var obj = typeof(SomeClass1).CreateInstance();
            Assert.NotNull(obj);
            Assert.IsType<SomeClass1>(obj);

            var obj2 = typeof(SomeClass2).CreateInstance<SomeClass1>();
            Assert.NotNull(obj2);
            Assert.IsType<SomeClass2>(obj2);

            var obj3 = ConstructorInfoEx.CreateInstance<SomeClass2>();
            Assert.NotNull(obj3);
            Assert.IsType<SomeClass2>(obj3);
        }

        [Fact]
        public void CanCreateObjectUsingParametlessConstructor()
        {
            var obj = typeof(SomeClass1).Constructor().InvokerAs<Func<object>>()();
            Assert.NotNull(obj);
            Assert.IsType<SomeClass1>(obj);
            var obj2 = typeof(SomeClass2).Constructor().InvokerAs<Func<object>>()();
            Assert.NotNull(obj2);
            Assert.IsType<SomeClass2>(obj2);
        }

        [Fact]
        public void CanCreateObjectUsingPrivateConstructor()
        {
            var obj2 =
                typeof(SomeClass2).Constructor(typeof(string), typeof(int))
                    .InvokerAs<Func<string, int, object>>()("test1", 88);
            Assert.NotNull(obj2);
            Assert.IsType<SomeClass2>(obj2);
            var t = (SomeClass2) obj2;
            Assert.Equal(88, t.P1);
            Assert.Equal("test1", t.P2);
        }

        [Fact]
        public void CanCreateObjectUsingParametlessConstructorAs()
        {
            var obj = typeof(SomeClass1).Constructor().InvokerAs<Func<object>>()();
            Assert.NotNull(obj);
            Assert.IsType<SomeClass1>(obj);
            var obj2 = typeof(SomeClass2).Constructor().InvokerAs<Func<object>>()();
            Assert.NotNull(obj2);
            Assert.IsType<SomeClass2>(obj2);
        }

        [Fact]
        public void CanCreateObjectUsingPrivateConstructorAs()
        {
            var obj2 = typeof(SomeClass2).Constructor(typeof(string), typeof(int)).InvokerAs<Func<string, int, object>>()("test1", 88);
            Assert.NotNull(obj2);
            Assert.IsType<SomeClass2>(obj2);
            var t = (SomeClass2)obj2;
            Assert.Equal(88, t.P1);
            Assert.Equal("test1", t.P2);
        }

        [Fact]
        public void CanCreateObjectUsingPrivateConstructorInvoker()
        {
            var obj2 = typeof(SomeClass2).Constructor(typeof(string), typeof(int)).Invoker()("test1", 88);
            Assert.NotNull(obj2);
            Assert.IsType<SomeClass2>(obj2);
            var t = (SomeClass2)obj2;
            Assert.Equal(88, t.P1);
            Assert.Equal("test1", t.P2);
        }

        [Fact]
        public void CanCreateConstructorDelegate()
        {
            var invoker = typeof(SomeClass1).Constructor().InvokerAs(typeof(Func<object>));
            Assert.NotNull(invoker);
            Assert.IsType<Func<object>>(invoker);
            var obj = ((Func<object>)invoker)();
            Assert.NotNull(obj);
            Assert.IsType<SomeClass1>(obj);
        }

        [Fact]
        public void ConstructorsCacheOk()
        {
            var constructors = new HashSet<ConstructorInfo>(typeof(string).Constructors());
           
            var reflectConstructors = typeof(string).GetTypeInfo().DeclaredConstructors.Where(x => !x.IsStatic);
            Assert.True(constructors.SetEquals(reflectConstructors));
        }
    }
}