using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SimplyFast.IoC.Modules;
using SimplyFast.IoC.Tests.Modules;

namespace SimplyFast.IoC.Tests
{
    
    public class ModulesTests
    {


        [Fact]
        public void CanLoadOneModule()
        {
            var kernel = new FastKernel();
            Assert.Throws<InvalidOperationException>(() => kernel.Get<string>());
            Assert.False(kernel.Get<IEnumerable<string>>().Any());
            Assert.False(kernel.Get<List<string>>().Any());
            kernel.Load(new TestModule());
            Assert.Equal("test", kernel.Get<string>());
            Assert.True(kernel.Get<IEnumerable<string>>().SequenceEqual(new[] { "str1", "str2" }));
            Assert.IsType<TestModule.TestEnumerable>(kernel.Get<IEnumerable<string>>());
            Assert.True(kernel.Get<List<string>>().SequenceEqual(new[] { "test" }));
            kernel.Load(new TestModule2());
            FinalModuleTests(kernel);
        }

        private static void FinalModuleTests(FastKernel kernel)
        {
            Assert.Equal("test", kernel.Get<string>());
            Assert.True(kernel.Get<IEnumerable<string>>().SequenceEqual(new[] { "str1", "str2" }));
            Assert.IsType<TestModule.TestEnumerable>(kernel.Get<IEnumerable<string>>());
            Assert.True(kernel.Get<List<string>>().SequenceEqual(new[] { "str1", "str2" }));
        }

        [Fact]
        public void CanLoadTwoModules()
        {
            var kernel = new FastKernel();
            kernel.Load(new TestModule(), new TestModule2());
            FinalModuleTests(kernel);
            var reverseKernel = new FastKernel();
            reverseKernel.Load(new TestModule2(), new TestModule());
            FinalModuleTests(reverseKernel);
        }

#if NET
        private static void TestFewTimes(Action action, int times)
        {
            for (var i = 0; i < times; i++)
            {
                action();
            }
        }

        [Fact]
        public void CanLoadFromAssembly()
        {
            var kernel = new FastKernel();
            kernel.Load(typeof(TestModule).Assembly);
            FinalModuleTests(kernel);
        }

        [Fact]
        public void CanLoadParallelAssembly()
        {
            TestFewTimes(() =>
            {
                var kernel = new FastKernel();
                kernel.LoadParallel(typeof(TestModule).Assembly);
                FinalModuleTests(kernel);
            }, 100);
        }

        [Fact]
        public void CanLoadParallelAssemblies()
        {
            TestFewTimes(() =>
            {
                var kernel = new FastKernel();
                kernel.LoadParallel(new[] { typeof(TestModule).Assembly });
                FinalModuleTests(kernel);
            }, 100);
        }
#endif
    }
}