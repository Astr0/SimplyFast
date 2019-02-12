using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SimplyFast.IoC.Tests.TestData;

namespace SimplyFast.IoC.Tests
{
    
    public class ResolvingTests
    {
        public  ResolvingTests()
        {
            _kernel = KernelEx.Create();
        }

        private readonly IKernel _kernel;

        [Fact]
        public void UsesNonDefaultCtorWhenBindingExists()
        {
            Assert.Equal(11, _kernel.Get<SomeClass3>().Value);

            _kernel.Bind<long>().ToConstant(100);
            _kernel.Bind<int>().ToConstant(55);
            Assert.Equal(55, _kernel.Get<SomeClass3>().Value);
            Assert.Throws<InvalidOperationException>(() => _kernel.Get<SomeClass>());
            _kernel.Bind<char>().ToConstant('a');
            var test = _kernel.Get<SomeClass>();
            Assert.Equal(new SomeClass('a', 100), test);
            var test2 = _kernel.Get<SomeClass>();
            Assert.Equal(new SomeClass('a', 100), test2);
            Assert.False(ReferenceEquals(test, test2));
            _kernel.Bind<long>().ToConstant(10);
            var test3 = _kernel.Get<SomeClass>();
            Assert.Equal(new SomeClass('a', 10), test3);
            const string testStr = "test";
            _kernel.Bind<string>().ToConstant(testStr);
            var test4 = _kernel.Get<SomeClass>();
            Assert.Equal(new SomeClass('a', 10, testStr), test4);
            Assert.True(ReferenceEquals(testStr, test4.Str));
        }

        [Fact]
        public void CanBindHierarchy()
        {
            var integers = new[] {1, 2, 3, 4};
            _kernel.Bind<IEnumerable<int>>().ToConstant(integers);
            _kernel.Bind<List<int>>().ToConstructor(x => new List<int>(x.Get<IEnumerable<int>>()));
            _kernel.Bind<char>().ToConstant('c');
            _kernel.Bind<long>().ToMethod(c => c.Get<IEnumerable<int>>().First());
            var test = _kernel.Get<SomeClass2>();
            Assert.True(test.Ints.SequenceEqual(integers));
            Assert.Equal(new SomeClass('c', 1), test.Test);
            
            // Funcs are not resolved dynamically
            // Assert.Throws<InvalidOperationException>(() => _kernel.Get<Func<SomeClass2>>());
            _kernel.Bind<SomeClass2>().ToSelf();
            
            var func = _kernel.Get<Func<SomeClass2>>();
            integers[0] = 10;
            Assert.False(test.Ints.SequenceEqual(integers));
            var test2 = func();
            Assert.True(test2.Ints.SequenceEqual(integers));
            Assert.Equal(new SomeClass('c', 10), test2.Test);

            var list = new List<int> {2, 3, 4};
            _kernel.Bind<List<int>>().ToConstant(list);
            integers[0] = 11;
            var test3 = func();
            Assert.True(test3.Ints.SequenceEqual(list));
            Assert.True(ReferenceEquals(test3.Ints, list));
            Assert.Equal(new SomeClass('c', 11), test3.Test);
        }
    }
}