using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace SimplyFast.IoC.Tests
{
    
    public class DerivedBindTests
    {
        public DerivedBindTests()
        {
            _kernel = KernelEx.Create();
        }

        private readonly IKernel _kernel;

        [Fact]
        public void DerivedExistsEvenIfNotBinded()
        {
            Assert.NotNull(_kernel.Get<Func<object>>());
            Assert.NotNull(_kernel.Get<IEnumerable<object>>());
            Assert.NotNull(_kernel.Get<object[]>());
            Assert.NotNull(_kernel.Get<List<object>>());
            Assert.NotNull(_kernel.Get<IList<object>>());
            Assert.NotNull(_kernel.Get<ICollection<object>>());
            Assert.NotNull(_kernel.Get<IReadOnlyList<object>>());
            Assert.NotNull(_kernel.Get<IReadOnlyCollection<object>>());
        }

        [Fact]
        public void DerivedReturnsDefaultIfNotBinded()
        {
            Assert.NotNull(_kernel.Get<Func<object>>()());
            AssertCollectionObj(_kernel.Get<IEnumerable<object>>());
            AssertCollectionObj(_kernel.Get<object[]>());
            AssertCollectionObj(_kernel.Get<List<object>>());
            AssertCollectionObj(_kernel.Get<IList<object>>());
            AssertCollectionObj(_kernel.Get<ICollection<object>>());
            AssertCollectionObj(_kernel.Get<IReadOnlyList<object>>());
            AssertCollectionObj(_kernel.Get<IReadOnlyCollection<object>>());
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static void AssertCollectionObj(IEnumerable<object> collection)
        {
            var item = collection.Single();
            Assert.NotNull(item);
            Assert.IsType<object>(item);
        }

        [Fact]
        public void DerivedBindingOkForConsts()
        {
            const string str1 = "test";
            const string str2 = "testy";
            var expected = new HashSet<string>(new[] { str1, str2 });
            _kernel.Bind<string>().ToConstant(str1);
            _kernel.Bind<string>().ToConstant(str2);
            Assert.Equal(str2, _kernel.Get<Func<string>>()());
            AssertCollections(expected);
        }


        [Fact]
        public void DerivedBindingAreDynamic()
        {
            const string str1 = "test";
            var expected = new HashSet<string>(new[] { str1 });
            _kernel.Bind<string>().ToConstant(str1);
            var func = _kernel.Get<Func<string>>();
            Assert.Equal(str1, func());
            AssertCollections(expected);
            var str2 = "test2";
            // ReSharper disable once AccessToModifiedClosure
            _kernel.Bind<string>().ToMethod(c => str2);
            expected.Add(str2);
            Assert.Equal(str2, func());
            Assert.Equal(str2, _kernel.Get<Func<string>>()());
            AssertCollections(expected);
            str2 = "test_o.O";
            Assert.Equal(str2, func());
            Assert.Equal(str2, _kernel.Get<Func<string>>()());

            expected = new HashSet<string> { str1, str2 };
            AssertCollections(expected);
        }

        [Fact]
        public void DerivedBindingCanBeOverriden()
        {
            const string str = "test";
            _kernel.Bind<string>().ToConstant(str);
            var func = _kernel.Get<Func<string>>();
            Assert.Equal(str, func());
            const string str2 = "o.O";
            _kernel.Bind<Func<string>>().ToConstant(() => str2);
            Assert.Equal(str, func());
            Assert.Equal(str2, _kernel.Get<Func<string>>()());
            AssertCollections(new HashSet<string> { str });
            var empty = new string[0];
            BindCollections(empty);
            AssertCollections(new HashSet<string>());
        }

        private void BindCollections(string[] array)
        {
            _kernel.Bind<IEnumerable<string>>().ToConstant(array);
            _kernel.Bind<IList<string>>().ToConstant(array);
            _kernel.Bind<ICollection<string>>().ToConstant(array);
            _kernel.Bind<IReadOnlyList<string>>().ToConstant(array);
            _kernel.Bind<IReadOnlyCollection<string>>().ToConstant(array);
            _kernel.Bind<IEnumerable<string>>().ToConstant(array);
            _kernel.Bind<List<string>>().ToConstant(array.ToList());
            _kernel.Bind<string[]>().ToConstant(array);
        }

        [Fact]
        public void DerivedDoesNotOverride()
        {
            var arr = new[] { "test1", "test2" };
            BindCollections(arr);
            _kernel.Bind<Func<string>>().ToConstant(() => "test");
            _kernel.Bind<string>().ToConstant("o.O");
            Assert.Equal("test", _kernel.Get<Func<string>>()());
            AssertCollections(new HashSet<string>(arr));
        }

        private void AssertCollections(HashSet<string> expected)
        {
            Assert.True(expected.SetEquals(_kernel.Get<IEnumerable<string>>()));
            Assert.True(expected.SetEquals(_kernel.Get<IList<string>>()));
            Assert.True(expected.SetEquals(_kernel.Get<ICollection<string>>()));
            Assert.True(expected.SetEquals(_kernel.Get<IReadOnlyList<string>>()));
            Assert.True(expected.SetEquals(_kernel.Get<IReadOnlyCollection<string>>()));
            Assert.True(expected.SetEquals(_kernel.Get<List<string>>()));
            Assert.True(expected.SetEquals(_kernel.Get<string[]>()));
        }
    }
}