using System.Collections.Generic;
using System.Linq;
using Xunit;
using SimplyFast.IoC.Tests.TestData;

namespace SimplyFast.IoC.Tests
{
    
    public class InjectionTests
    {
        public InjectionTests()
        {
            _kernel = new FastKernel();
        }

        private readonly IKernel _kernel;

        [Fact]
        public void GetInjects()
        {
            var test = _kernel.Get<InjectTestClass>();
            Assert.NotNull(test.Longs);
            Assert.Equal(0, test.Longs.Count);
            Assert.Equal(0, test.Long);
            Assert.Equal(null, test.String);
        }

        [Fact]
        public void CanInjectMultipleTimes()
        {
            var test = _kernel.Get<InjectTestClass>();
            Assert.NotNull(test.Longs);
            Assert.Equal(0, test.Longs.Count);
            Assert.Equal(0, test.Long);
            Assert.Equal(null, test.String);
            test.Longs = null;
            _kernel.Inject(test);
            Assert.NotNull(test.Longs);
            Assert.Equal(0, test.Longs.Count);
            Assert.Equal(0, test.Long);
            Assert.Equal(null, test.String);
        }

        [Fact]
        public void CanInjectNonGetObject()
        {
            var test = new InjectTestClass();
            Assert.Null(test.Longs);
            _kernel.Inject(test);
            Assert.NotNull(test.Longs);
            Assert.Equal(0, test.Longs.Count);
            Assert.Equal(0, test.Long);
            Assert.Equal(null, test.String);
        }

        [Fact]
        public void InjectUsesBinding()
        {
            var list = new List<long>(){1,2,3,4};
            _kernel.Bind<List<long>>().ToConstant(list);
            var test = new InjectTestClass();
            Assert.Null(test.Longs);
            _kernel.Inject(test);
            Assert.True(ReferenceEquals(test.Longs, list));
            Assert.Equal(0, test.Long);
            Assert.Equal(null, test.String);
        }

        [Fact]
        public void InjectUsesBestMethod()
        {
            _kernel.Bind<long>().ToConstant(5);
            var test = new InjectTestClass();
            Assert.Null(test.Longs);
            _kernel.Inject(test);
            Assert.Equal(null, test.String);
            Assert.Equal(5, test.Long);
            Assert.NotNull(test.Longs);
            Assert.True(test.Longs.SequenceEqual(new long[]{5}));
        }

        [Fact]
        public void InjectGetsAlways()
        {
            _kernel.Bind<long>().ToConstant(5);
            var test = new InjectTestClass();
            Assert.Null(test.Longs);
            _kernel.Inject(test);
            Assert.Equal(null, test.String);
            Assert.Equal(5, test.Long);
            Assert.NotNull(test.Longs);
            Assert.True(test.Longs.SequenceEqual(new long[] { 5 }));

            _kernel.Bind<long>().ToConstant(10);
            _kernel.Inject(test);
            Assert.Equal(null, test.String);
            Assert.Equal(10, test.Long);
            Assert.Equal(2, test.Longs.Count);
            Assert.True(new HashSet<long>{5, 10}.SetEquals(test.Longs));
        }

        [Fact]
        public void InjectUpgradesMethodWhenNewBindingAvailable()
        {
            var test = new InjectTestClass();
            Assert.Null(test.Longs);
            _kernel.Inject(test);
            Assert.Equal(null, test.String);
            Assert.Equal(0, test.Long);
            Assert.NotNull(test.Longs);
            Assert.Equal(0, test.Longs.Count);

            _kernel.Bind<long>().ToConstant(5);
            _kernel.Inject(test);
            Assert.Equal(null, test.String);
            Assert.Equal(5, test.Long);
            Assert.NotNull(test.Longs);
            Assert.True(test.Longs.SequenceEqual(new long[] { 5 }));

            _kernel.Bind<string>().ToConstant("test");
            _kernel.Inject(test);
            Assert.Equal("test", test.String);
            Assert.Equal(5, test.Long);
            Assert.NotNull(test.Longs);
            Assert.True(test.Longs.SequenceEqual(new long[] { 5 }));
        }

        [Fact]
        public void InjectUsesArgBindings()
        {
            var test = new InjectTestClass();
            Assert.Null(test.Longs);
            _kernel.Inject(test);
            Assert.Equal(null, test.String);
            Assert.Equal(0, test.Long);
            Assert.NotNull(test.Longs);
            Assert.Equal(0, test.Longs.Count);

            _kernel.Bind<long>().ToConstant(5);
            _kernel.Inject(test);
            Assert.Equal(null, test.String);
            Assert.Equal(5, test.Long);
            Assert.NotNull(test.Longs);
            Assert.True(test.Longs.SequenceEqual(new long[] { 5 }));

            var argKernel = _kernel.Get<IGetKernel>(BindArg.Typed("test"));
            argKernel.Inject(test);
            Assert.Equal("test", test.String);
            Assert.Equal(5, test.Long);
            Assert.NotNull(test.Longs);
            Assert.True(test.Longs.SequenceEqual(new long[] { 5 }));
        }
    }
}