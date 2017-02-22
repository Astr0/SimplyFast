using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SF.IoC;
using SF.Tests.IoC.TestData;

namespace SF.Tests.IoC
{
    [TestFixture]
    public class InjectionTests
    {
        [SetUp]
        public void SetUp()
        {
            _kernel = new FastKernel();
        }

        private IKernel _kernel;

        [Test]
        public void GetInjects()
        {
            var test = _kernel.Get<InjectTestClass>();
            Assert.IsNotNull(test.Longs);
            Assert.AreEqual(0, test.Longs.Count);
            Assert.AreEqual(0, test.Long);
            Assert.AreEqual(null, test.String);
        }

        [Test]
        public void CanInjectMultipleTimes()
        {
            var test = _kernel.Get<InjectTestClass>();
            Assert.IsNotNull(test.Longs);
            Assert.AreEqual(0, test.Longs.Count);
            Assert.AreEqual(0, test.Long);
            Assert.AreEqual(null, test.String);
            test.Longs = null;
            _kernel.Inject(test);
            Assert.IsNotNull(test.Longs);
            Assert.AreEqual(0, test.Longs.Count);
            Assert.AreEqual(0, test.Long);
            Assert.AreEqual(null, test.String);
        }

        [Test]
        public void CanInjectNonGetObject()
        {
            var test = new InjectTestClass();
            Assert.IsNull(test.Longs);
            _kernel.Inject(test);
            Assert.IsNotNull(test.Longs);
            Assert.AreEqual(0, test.Longs.Count);
            Assert.AreEqual(0, test.Long);
            Assert.AreEqual(null, test.String);
        }

        [Test]
        public void InjectUsesBinding()
        {
            var list = new List<long>(){1,2,3,4};
            _kernel.Bind<List<long>>().ToConstant(list);
            var test = new InjectTestClass();
            Assert.IsNull(test.Longs);
            _kernel.Inject(test);
            Assert.IsTrue(ReferenceEquals(test.Longs, list));
            Assert.AreEqual(0, test.Long);
            Assert.AreEqual(null, test.String);
        }

        [Test]
        public void InjectUsesBestMethod()
        {
            _kernel.Bind<long>().ToConstant(5);
            var test = new InjectTestClass();
            Assert.IsNull(test.Longs);
            _kernel.Inject(test);
            Assert.AreEqual(null, test.String);
            Assert.AreEqual(5, test.Long);
            Assert.IsNotNull(test.Longs);
            Assert.IsTrue(test.Longs.SequenceEqual(new long[]{5}));
        }

        [Test]
        public void InjectGetsAlways()
        {
            _kernel.Bind<long>().ToConstant(5);
            var test = new InjectTestClass();
            Assert.IsNull(test.Longs);
            _kernel.Inject(test);
            Assert.AreEqual(null, test.String);
            Assert.AreEqual(5, test.Long);
            Assert.IsNotNull(test.Longs);
            Assert.IsTrue(test.Longs.SequenceEqual(new long[] { 5 }));

            _kernel.Bind<long>().ToConstant(10);
            _kernel.Inject(test);
            Assert.AreEqual(null, test.String);
            Assert.AreEqual(10, test.Long);
            Assert.AreEqual(2, test.Longs.Count);
            Assert.IsTrue(new HashSet<long>{5, 10}.SetEquals(test.Longs));
        }

        [Test]
        public void InjectUpgradesMethodWhenNewBindingAvailable()
        {
            var test = new InjectTestClass();
            Assert.IsNull(test.Longs);
            _kernel.Inject(test);
            Assert.AreEqual(null, test.String);
            Assert.AreEqual(0, test.Long);
            Assert.IsNotNull(test.Longs);
            Assert.AreEqual(0, test.Longs.Count);

            _kernel.Bind<long>().ToConstant(5);
            _kernel.Inject(test);
            Assert.AreEqual(null, test.String);
            Assert.AreEqual(5, test.Long);
            Assert.IsNotNull(test.Longs);
            Assert.IsTrue(test.Longs.SequenceEqual(new long[] { 5 }));

            _kernel.Bind<string>().ToConstant("test");
            _kernel.Inject(test);
            Assert.AreEqual("test", test.String);
            Assert.AreEqual(5, test.Long);
            Assert.IsNotNull(test.Longs);
            Assert.IsTrue(test.Longs.SequenceEqual(new long[] { 5 }));
        }
    }
}