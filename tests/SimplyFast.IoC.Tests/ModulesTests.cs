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
            var kernel = KernelEx.Create();
            Assert.Throws<InvalidOperationException>(() => kernel.Get<string>());
            //Assert.Throws<InvalidOperationException>(() => kernel.Get<IEnumerable<string>>());
            Assert.False(kernel.Get<IEnumerable<string>>().Any());
            Assert.False(kernel.Get<List<string>>().Any());
            kernel.Load(new SomeModule());
            Assert.Equal("test", kernel.Get<string>());
            Assert.True(kernel.Get<IEnumerable<string>>().SequenceEqual(new[] {"str1", "str2"}));
            Assert.IsType<SomeModule.SomeEnumerable>(kernel.Get<IEnumerable<string>>());
            Assert.True(kernel.Get<List<string>>().SequenceEqual(new[] {"test"}));
            kernel.Load(new SomeModule2());
            FinalModuleTests(kernel);
        }

        private static void FinalModuleTests(IKernel kernel)
        {
            Assert.Equal("test", kernel.Get<string>());
            Assert.True(kernel.Get<IEnumerable<string>>().SequenceEqual(new[] {"str1", "str2"}));
            Assert.IsType<SomeModule.SomeEnumerable>(kernel.Get<IEnumerable<string>>());
            Assert.True(kernel.Get<List<string>>().SequenceEqual(new[] {"str1", "str2"}));
        }

        [Fact]
        public void CanLoadTwoModules()
        {
            var kernel = KernelEx.Create();
            kernel.Load(new SomeModule(), new SomeModule2());
            FinalModuleTests(kernel);
            var reverseKernel = KernelEx.Create();
            reverseKernel.Load(new SomeModule2(), new SomeModule());
            FinalModuleTests(reverseKernel);
        }

        private static void RunTestFewTimes(Action action, int times)
        {
            for (var i = 0; i < times; i++)
            {
                action();
            }
        }

        [Fact]
        public void CanLoadFromAssembly()
        {
            var kernel = KernelEx.Create();
            kernel.Load(typeof(SomeModule).Assembly);
            FinalModuleTests(kernel);
        }

        [Fact]
        public void CanLoadParallelAssembly()
        {
            RunTestFewTimes(() =>
            {
                var kernel = KernelEx.Create();
                kernel.LoadParallel(typeof(SomeModule).Assembly);
                FinalModuleTests(kernel);
            }, 100);
        }

        [Fact]
        public void CanLoadParallelAssemblies()
        {
            RunTestFewTimes(() =>
            {
                var kernel = KernelEx.Create();
                kernel.LoadParallel(new[] {typeof(SomeModule).Assembly});
                FinalModuleTests(kernel);
            }, 100);
        }
    }
}