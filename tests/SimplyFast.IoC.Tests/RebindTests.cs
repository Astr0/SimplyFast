using System;
using System.Collections.Generic;
using System.Linq;
using SimplyFast.IoC.Tests.TestData;
using Xunit;

namespace SimplyFast.IoC.Tests
{
    public class RebindTests
    {
        private readonly IKernel _kernel;

        public RebindTests()
        {
            _kernel = KernelEx.Create();
        }

        [Fact]
        public void RebindConstWorks()
        {
            _kernel.Bind<List<int>>().ToConstant(new List<int>{1,2,3});

            _kernel.Bind<SomeClass>().ToConstant(null);

            var obj1 = _kernel.Get<SomeClass2>();
            Assert.Equal(null, obj1.Test);
            Assert.True(obj1.Ints.SequenceEqual(new []{1,2,3}));

            var sc = new SomeClass('c', 1);
            _kernel.Bind<SomeClass>().ToConstant(sc);

            var obj2 = _kernel.Get<SomeClass2>();
            Assert.Equal(sc, obj2.Test);
            Assert.True(obj2.Ints.SequenceEqual(new []{1,2,3}));
        }

        [Fact]
        public void RebindConstWorksForFunc()
        {
            _kernel.Bind<List<int>>().ToConstant(new List<int>{1,2,3});

            _kernel.Bind<SomeClass>().ToConstant(null);

            var getObj = _kernel.Get<Func<SomeClass2>>();

            var obj1 = getObj();
            Assert.Equal(null, obj1.Test);
            Assert.True(obj1.Ints.SequenceEqual(new []{1,2,3}));

            var sc = new SomeClass('c', 1);
            _kernel.Bind<SomeClass>().ToConstant(sc);

            var obj2 = getObj();
            Assert.Equal(sc, obj2.Test);
            Assert.True(obj2.Ints.SequenceEqual(new []{1,2,3}));
        }

    }
}