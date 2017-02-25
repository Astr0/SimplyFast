using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using SimplyFast.Reflection.Tests.TestData;

namespace SimplyFast.Reflection.Tests
{
    [TestFixture]
    public class ConstructorInfoExTests: ReflectionTests
    {

        [Test]
        public void CanCreateObjectUsingCreateInstance()
        {
            var obj = typeof(TestClass1).CreateInstance();
            Assert.IsNotNull(obj);
            Assert.IsInstanceOf<TestClass1>(obj);

            var obj2 = typeof(TestClass2).CreateInstance<TestClass1>();
            Assert.IsNotNull(obj2);
            Assert.IsInstanceOf<TestClass2>(obj2);

            var obj3 = ConstructorInfoEx.CreateInstance<TestClass2>();
            Assert.IsNotNull(obj3);
            Assert.IsInstanceOf<TestClass2>(obj3);
        }

        [Test]
        public void CanCreateObjectUsingParametlessConstructor()
        {
            var obj = typeof(TestClass1).Constructor().InvokerAs<Func<object>>()();
            Assert.IsNotNull(obj);
            Assert.IsInstanceOf<TestClass1>(obj);
            var obj2 = typeof(TestClass2).Constructor().InvokerAs<Func<object>>()();
            Assert.IsNotNull(obj2);
            Assert.IsInstanceOf<TestClass2>(obj2);
        }

        [Test]
        public void CanCreateObjectUsingPrivateConstructor()
        {
            CheckPrivateAccess();
            var obj2 =
                typeof(TestClass2).Constructor(typeof(string), typeof(int))
                    .InvokerAs<Func<string, int, object>>()("test1", 88);
            Assert.IsNotNull(obj2);
            Assert.IsInstanceOf<TestClass2>(obj2);
            var t = (TestClass2) obj2;
            Assert.AreEqual(88, t.P1);
            Assert.AreEqual("test1", t.P2);
        }

        [Test]
        public void CanCreateObjectUsingParametlessConstructorAs()
        {
            var obj = typeof(TestClass1).Constructor().InvokerAs<Func<object>>()();
            Assert.IsNotNull(obj);
            Assert.IsInstanceOf<TestClass1>(obj);
            var obj2 = typeof(TestClass2).Constructor().InvokerAs<Func<object>>()();
            Assert.IsNotNull(obj2);
            Assert.IsInstanceOf<TestClass2>(obj2);
        }

        [Test]
        public void CanCreateObjectUsingPrivateConstructorAs()
        {
            CheckPrivateAccess();
            var obj2 = typeof(TestClass2).Constructor(typeof(string), typeof(int)).InvokerAs<Func<string, int, object>>()("test1", 88);
            Assert.IsNotNull(obj2);
            Assert.IsInstanceOf<TestClass2>(obj2);
            var t = (TestClass2)obj2;
            Assert.AreEqual(88, t.P1);
            Assert.AreEqual("test1", t.P2);
        }

        [Test]
        public void CanCreateObjectUsingPrivateConstructorInvoker()
        {
            CheckPrivateAccess();
            var obj2 = typeof(TestClass2).Constructor(typeof(string), typeof(int)).Invoker()("test1", 88);
            Assert.IsNotNull(obj2);
            Assert.IsInstanceOf<TestClass2>(obj2);
            var t = (TestClass2)obj2;
            Assert.AreEqual(88, t.P1);
            Assert.AreEqual("test1", t.P2);
        }

        [Test]
        public void CanCreateConstructorDelegate()
        {
            var invoker = typeof(TestClass1).Constructor().InvokerAs(typeof(Func<object>));
            Assert.IsNotNull(invoker);
            Assert.IsInstanceOf<Func<object>>(invoker);
            var obj = ((Func<object>)invoker)();
            Assert.IsNotNull(obj);
            Assert.IsInstanceOf<TestClass1>(obj);
        }

        [Test]
        public void ConstructorsCacheOk()
        {
            var constructors = new HashSet<ConstructorInfo>(typeof(string).Constructors());
           
            var reflectConstructors = typeof(string).GetTypeInfo().DeclaredConstructors.Where(x => !x.IsStatic);
            Assert.IsTrue(constructors.SetEquals(reflectConstructors));
        }
    }
}