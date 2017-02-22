using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SF.IoC;
using SF.IoC.Modules;
using SF.Tests.IoC.Modules;

namespace SF.Tests.IoC
{
    [TestFixture]
    public class ModulesTests
    {
        private static void TestFewTimes(Action action, int times)
        {
            for (var i = 0; i < times; i++)
            {
                action();
            }
        }

        [Test]
        public void CanLoadOneModule()
        {
            var kernel = new FastKernel();
            Assert.Throws<InvalidOperationException>(() => kernel.Get<string>());
            Assert.IsFalse(kernel.Get<IEnumerable<string>>().Any());
            Assert.IsFalse(kernel.Get<List<string>>().Any());
            kernel.Load(new TestModule());
            Assert.AreEqual("test", kernel.Get<string>());
            Assert.IsTrue(kernel.Get<IEnumerable<string>>().SequenceEqual(new[] { "str1", "str2" }));
            Assert.IsInstanceOf<TestModule.TestEnumerable>(kernel.Get<IEnumerable<string>>());
            Assert.IsTrue(kernel.Get<List<string>>().SequenceEqual(new[] { "test" }));
            kernel.Load(new TestModule2());
            FinalModuleTests(kernel);
        }

        private static void FinalModuleTests(FastKernel kernel)
        {
            Assert.AreEqual("test", kernel.Get<string>());
            Assert.IsTrue(kernel.Get<IEnumerable<string>>().SequenceEqual(new[] { "str1", "str2" }));
            Assert.IsInstanceOf<TestModule.TestEnumerable>(kernel.Get<IEnumerable<string>>());
            Assert.IsTrue(kernel.Get<List<string>>().SequenceEqual(new[] { "str1", "str2" }));
        }

        [Test]
        public void CanLoadTwoModules()
        {
            var kernel = new FastKernel();
            kernel.Load(new TestModule(), new TestModule2());
            FinalModuleTests(kernel);
            var reverseKernel = new FastKernel();
            reverseKernel.Load(new TestModule2(), new TestModule());
            FinalModuleTests(reverseKernel);
        }

        [Test]
        public void CanLoadFromAssembly()
        {
            var kernel = new FastKernel();
            kernel.Load(typeof(TestModule).Assembly);
            FinalModuleTests(kernel);
        }

        [Test]
        public void CanLoadParallelAssembly()
        {
            TestFewTimes(() =>
            {
                var kernel = new FastKernel();
                kernel.LoadParallel(typeof(TestModule).Assembly);
                FinalModuleTests(kernel);
            }, 100);
        }

        [Test]
        public void CanLoadParallelAssemblies()
        {
            TestFewTimes(() =>
            {
                var kernel = new FastKernel();
                kernel.LoadParallel(new[] { typeof(TestModule).Assembly });
                FinalModuleTests(kernel);
            }, 100);
        }
    }
}