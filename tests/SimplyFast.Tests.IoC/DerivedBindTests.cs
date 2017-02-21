using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SF.IoC;

namespace SF.Tests.IoC
{
    [TestFixture]
    public class DerivedBindTests
    {
        [SetUp]
        public void SetUp()
        {
            _kernel = new FastKernel();
        }

        private IKernel _kernel;

        [Test]
        public void DerivedExistsOnlyIfBinded()
        {
            Assert.Throws<InvalidOperationException>(() => _kernel.Get<Func<object>>());
            Assert.Throws<InvalidOperationException>(() => _kernel.Get<IEnumerable<object>>());
            Assert.Throws<InvalidOperationException>(() => _kernel.Get<object[]>());
            Assert.Throws<InvalidOperationException>(() => _kernel.Get<IList<object>>());
            Assert.Throws<InvalidOperationException>(() => _kernel.Get<ICollection<object>>());
            Assert.Throws<InvalidOperationException>(() => _kernel.Get<IReadOnlyList<object>>());
            Assert.Throws<InvalidOperationException>(() => _kernel.Get<IReadOnlyCollection<object>>());
            _kernel.Bind<object>().ToSelf();
            Assert.DoesNotThrow(() => _kernel.Get<Func<object>>());
            Assert.DoesNotThrow(() => _kernel.Get<IEnumerable<object>>());
            Assert.DoesNotThrow(() => _kernel.Get<object[]>());
            Assert.DoesNotThrow(() => _kernel.Get<List<object>>());
            Assert.DoesNotThrow(() => _kernel.Get<IList<object>>());
            Assert.DoesNotThrow(() => _kernel.Get<ICollection<object>>());
            Assert.DoesNotThrow(() => _kernel.Get<IReadOnlyList<object>>());
            Assert.DoesNotThrow(() => _kernel.Get<IReadOnlyCollection<object>>());
        }

        [Test]
        public void DerivedBindingOkForConsts()
        {
            const string str1 = "test";
            const string str2 = "testy";
            var expected = new HashSet<string>(new[] { str1, str2 });
            _kernel.Bind<string>().ToConstant(str1);
            _kernel.Bind<string>().ToConstant(str2);
            Assert.AreEqual(str2, _kernel.Get<Func<string>>()());
            AssertCollections(expected);
        }


        [Test]
        public void DerivedBindingAreDynamic()
        {
            const string str1 = "test";
            var expected = new HashSet<string>(new[] { str1 });
            _kernel.Bind<string>().ToConstant(str1);
            var func = _kernel.Get<Func<string>>();
            Assert.AreEqual(str1, func());
            AssertCollections(expected);
            var str2 = "test2";
            // ReSharper disable once AccessToModifiedClosure
            _kernel.Bind<string>().ToMethod(c => str2);
            expected.Add(str2);
            Assert.AreEqual(str2, func());
            Assert.AreEqual(str2, _kernel.Get<Func<string>>()());
            AssertCollections(expected);
            str2 = "test_o.O";
            Assert.AreEqual(str2, func());
            Assert.AreEqual(str2, _kernel.Get<Func<string>>()());

            expected = new HashSet<string> { str1, str2 };
            AssertCollections(expected);
        }

        [Test]
        public void DerivedBindingCanBeOverriden()
        {
            const string str = "test";
            _kernel.Bind<string>().ToConstant(str);
            var func = _kernel.Get<Func<string>>();
            Assert.AreEqual(str, func());
            const string str2 = "o.O";
            _kernel.Bind<Func<string>>().ToConstant(() => str2);
            Assert.AreEqual(str, func());
            Assert.AreEqual(str2, _kernel.Get<Func<string>>()());
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

        [Test]
        public void DerivedDoesNotOverride()
        {
            var arr = new[] { "test1", "test2" };
            BindCollections(arr);
            _kernel.Bind<Func<string>>().ToConstant(() => "test");
            _kernel.Bind<string>().ToConstant("o.O");
            Assert.AreEqual("test", _kernel.Get<Func<string>>()());
            AssertCollections(new HashSet<string>(arr));
        }

        private void AssertCollections(HashSet<string> expected)
        {
            Assert.IsTrue(expected.SetEquals(_kernel.Get<IEnumerable<string>>()));
            Assert.IsTrue(expected.SetEquals(_kernel.Get<IList<string>>()));
            Assert.IsTrue(expected.SetEquals(_kernel.Get<ICollection<string>>()));
            Assert.IsTrue(expected.SetEquals(_kernel.Get<IReadOnlyList<string>>()));
            Assert.IsTrue(expected.SetEquals(_kernel.Get<IReadOnlyCollection<string>>()));
            Assert.IsTrue(expected.SetEquals(_kernel.Get<List<string>>()));
            Assert.IsTrue(expected.SetEquals(_kernel.Get<string[]>()));
        }
    }
}