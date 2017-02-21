using System;
using NUnit.Framework;
using SF.IoC;
using SF.Tests.IoC.TestData;

namespace SF.Tests.IoC
{
    [TestFixture]
    public class ArgBindTest
    {
        private IKernel _kernel;

        [SetUp]
        public void SetUp()
        {
            _kernel = new FastKernel();
        }

        [Test]
        public void CanBindSelfWithArgs()
        {
            Assert.AreEqual("test", _kernel.Get<string>(BindArg.Typed("test")));
            Assert.AreEqual("test2", _kernel.Get<string>(BindArg.Typed("test2"), BindArg.Typed(4), BindArg.Typed('c')));
            Assert.AreEqual(11, _kernel.Get<int>(BindArg.Typed("test2"), BindArg.Typed(11), BindArg.Typed(42UL), BindArg.Typed('c')));
            Assert.AreEqual(null, _kernel.Get<string>(BindArg.Typed<string>(null), BindArg.Typed(4), BindArg.Typed('c')));
        }

        [Test]
        public void CanBindUnbindableWithArgs()
        {
            Assert.Throws<InvalidOperationException>(() => _kernel.Get<TestClass>());
            Assert.AreEqual(new TestClass('c', 12), _kernel.Get<TestClass>(BindArg.Typed('c'), BindArg.Named("i", 12L)));
            _kernel.Bind<char>().ToConstant('d');
            Assert.Throws<InvalidOperationException>(() => _kernel.Get<TestClass>());
            Assert.AreEqual(new TestClass('d', 42), _kernel.Get<TestClass>(BindArg.Named("i", 42L)));
        }

        [Test]
        public void ArgsCoolerThanBind()
        {
            _kernel.Bind<char>().ToConstant('d');
            _kernel.Bind<long>().ToConstant(12);
            Assert.AreEqual(new TestClass('d', 12), _kernel.Get<TestClass>());
            Assert.AreEqual(new TestClass('d', 42), _kernel.Get<TestClass>(BindArg.Typed(42L)));
            Assert.AreEqual(new TestClass('c', 42), _kernel.Get<TestClass>(BindArg.Typed('c'), BindArg.Typed(42L)));
        }

        [Test]
        public void LameArgsAreIgnored()
        {
            _kernel.Bind<char>().ToConstant('d');
            _kernel.Bind<long>().ToConstant(12);
            Assert.AreEqual(new TestClass('d', 12), _kernel.Get<TestClass>());
            Assert.AreEqual(new TestClass('d', 42), _kernel.Get<TestClass>(BindArg.Typed(42L), BindArg.Typed(2)));
            Assert.AreEqual(new TestClass('c', 42), _kernel.Get<TestClass>(BindArg.Typed('c'), BindArg.Typed(false), BindArg.Named("i", "test"), BindArg.Typed(42L), BindArg.Typed(new object())));
            Assert.AreEqual(new TestClass('c', 42), _kernel.Get<TestClass>(
                BindArg.Named("c", "a"), BindArg.Named("c", 12L), BindArg.Named("c", 12), BindArg.Named("c", 'c'),
                BindArg.Typed(42L), BindArg.Named("i", 11), BindArg.Named("i", "i")));
        }

        [Test]
        public void CanBindDerivedUnbindableWithArgs()
        {
            Assert.Throws<InvalidOperationException>(() => _kernel.Get<TestClass2>());
            Assert.AreEqual(new TestClass('c', 12), _kernel.Get<TestClass2>(BindArg.Typed('c'), BindArg.Named("i", 12L)).Test);
            _kernel.Bind<char>().ToConstant('d');
            Assert.Throws<InvalidOperationException>(() => _kernel.Get<TestClass2>());
            Assert.AreEqual(new TestClass('d', 42), _kernel.Get<TestClass2>(BindArg.Named("i", 42L)).Test);
        }

        [Test]
        public void ArgBindUseBetterCtor()
        {
            _kernel.Bind<char>().ToConstant('d');
            _kernel.Bind<long>().ToConstant(12);
            Assert.AreEqual(new TestClass('d', 12), _kernel.Get<TestClass>());
            Assert.AreEqual(new TestClass('c', 12, "test"), _kernel.Get<TestClass>(BindArg.Typed('c'), BindArg.Typed("test")));
            Assert.AreEqual(new TestClass('c', 12, "test"), _kernel.Get<TestClass2>(BindArg.Typed('c'), BindArg.Typed("test")).Test);
        }

        [Test]
        public void ArgBindDontUseBetterCtorIfTypeWasBind()
        {
            _kernel.Bind<char>().ToConstant('d');
            _kernel.Bind<long>().ToConstant(12);
            _kernel.Bind<TestClass>().ToSelf();
            Assert.AreEqual(new TestClass('d', 12), _kernel.Get<TestClass>());
            Assert.AreEqual(new TestClass('c', 12, "test"), _kernel.Get<TestClass>(BindArg.Typed('c'), BindArg.Typed("test")));
            Assert.AreEqual(new TestClass('c', 12), _kernel.Get<TestClass2>(BindArg.Typed('c'), BindArg.Typed("test")).Test);
        }

        [Test]
        public void ArgBindReturnsItsKernel()
        {
            var kernel = _kernel.Get<IGetKernel>(BindArg.Typed(12L), BindArg.Typed('c'));
            var argKernel = _kernel.Get<IArgKernel>(BindArg.Typed(12L), BindArg.Typed('d'));
            Assert.AreNotEqual(kernel, _kernel);
            Assert.AreNotEqual(argKernel, _kernel);
            Assert.Throws<InvalidOperationException>(() => _kernel.Get<TestClass>());
            Assert.AreEqual(new TestClass('c', 12), kernel.Get<TestClass>());
            Assert.AreEqual(new TestClass('d', 12), argKernel.Get<TestClass>());
        }

        [Test]
        public void ArgBindCanBeStackedAndOverriden()
        {
            var argKernel = _kernel.Get<IArgKernel>(BindArg.Typed(12L));
            Assert.Throws<InvalidOperationException>(() => argKernel.Get<TestClass>());
            Assert.AreEqual(new TestClass('c', 12), argKernel.Get<TestClass>(BindArg.Typed('c')));
            Assert.AreEqual(new TestClass('c', 42), argKernel.Get<TestClass>(BindArg.Typed('c'), BindArg.Typed(42L)));

            var argKernel2 = argKernel.Get<IArgKernel>(BindArg.Typed('d'));
            Assert.AreEqual(new TestClass('d', 12), argKernel2.Get<TestClass>());
        }

        [Test]
        public void ArgBindCanMakeFactories()
        {
            _kernel.Bind<Func<long, TestClass>>().ToMethod(c => l => c.Get<TestClass>(BindArg.Typed(l)));
            var f = _kernel.Get<Func<long, TestClass>>();
            Assert.Throws<InvalidOperationException>(() => f(11));
            _kernel.Bind<char>().ToConstant('d');
            Assert.AreEqual(new TestClass('d', 12), f(12));
            Assert.AreEqual(new TestClass('d', 42), f(42));
            _kernel.Bind<char>().ToConstant('c');
            Assert.AreEqual(new TestClass('c', 12), f(12));
            Assert.AreEqual(new TestClass('c', 42), f(42));
        }

        [Test]
        public void ArgBindFactoriesCaptureArgBind()
        {
            _kernel.Bind<Func<long, TestClass>>().ToMethod(c => l => c.Get<TestClass>(BindArg.Typed(l)));
            var argKernel = _kernel.Get<IArgKernel>(BindArg.Typed('c'));
            var f = argKernel.Get<Func<long, TestClass>>();
            _kernel.Bind<char>().ToConstant('d');
            Assert.AreEqual(new TestClass('c', 12), f(12));
            Assert.AreEqual(new TestClass('c', 42), f(42));
        }

        [Test]
        public void DerivedFactoriesCaptureArgBind()
        {
            _kernel.Bind<TestClass>().ToSelf();
            var f = _kernel.Get<Func<TestClass>>();
            Assert.Throws<InvalidOperationException>(() => f());
            var f2 = _kernel.Get<Func<TestClass>>(BindArg.Typed('c'), BindArg.Typed(11L));
            Assert.AreEqual(new TestClass('c', 11), f2());
        }
    }
}