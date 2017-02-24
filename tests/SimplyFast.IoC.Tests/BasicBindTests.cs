using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace SimplyFast.IoC.Tests
{
    [TestFixture]
    public class BasicBindTests
    {
        private IKernel _kernel;

        [SetUp]
        public void SetUp()
        {
            _kernel = new FastKernel();
        }

        [Test]
        public void KernelBindsItself()
        {
            Assert.AreEqual(_kernel, _kernel.Get<IKernel>());
            Assert.AreEqual(_kernel, _kernel.Get<IGetKernel>());
        }

        [Test]
        public void KernelBindsItselfAsFactory()
        {
            Assert.AreEqual(_kernel, _kernel.Get<Func<IKernel>>()());
            Assert.AreEqual(_kernel, _kernel.Get<Func<IGetKernel>>()());
        }

        [Test]
        public void ConstantBindingWorks()
        {
            var o = new object();
            _kernel.Bind<object>().ToConstant(o);
            Assert.AreEqual(o, _kernel.Get<object>());
            Assert.AreEqual(o, _kernel.Get<Func<object>>()());
        }

        [Test]
        public void MethodBindingWorks()
        {
            var i = 0;
            _kernel.Bind<int>().ToMethod(c => i++);
            Assert.AreEqual(0, i);
            Assert.AreEqual(0, _kernel.Get<int>());
            Assert.AreEqual(1, i);
            Assert.AreEqual(1, _kernel.Get<int>());
            Assert.AreEqual(2, i);
            var func = _kernel.Get<Func<int>>();
            Assert.AreEqual(2, i);
            Assert.AreEqual(2, func());
            Assert.AreEqual(3, i);
            Assert.AreEqual(3, func());
            Assert.AreEqual(4, i);
        }

        [Test]
        public void NinjectSytaxOk()
        {
            _kernel.Bind<string>().ToConstructor(c => "test");
            Assert.AreEqual("test", _kernel.Inject<string>());
        }

        [Test]
        public void SingletonBindingWorksForStruct()
        {
            var i = 0;
            _kernel.Bind<int>().ToMethod(c => i++).InSingletonScope();
            Assert.AreEqual(0, i);
            Assert.AreEqual(0, _kernel.Get<int>());
            Assert.AreEqual(1, i);
            Assert.AreEqual(0, _kernel.Get<int>());
            Assert.AreEqual(1, i);
            var func = _kernel.Get<Func<int>>();
            Assert.AreEqual(1, i);
            Assert.AreEqual(0, func());
            Assert.AreEqual(1, i);
            Assert.AreEqual(0, func());
            Assert.AreEqual(1, i);
        }

        [Test]
        public void SingletonBindingWorksForClass()
        {
            object o = null;
            _kernel.Bind<object>().ToMethod(c => o = new object()).InSingletonScope();
            Assert.IsNull(o);
            var first = _kernel.Get<object>();
            Assert.AreEqual(o, first);
            Assert.AreEqual(first, _kernel.Get<object>());
            Assert.AreEqual(first, o);
            var func = _kernel.Get<Func<object>>();
            Assert.AreEqual(first, o);
            Assert.AreEqual(first, func());
            Assert.AreEqual(first, o);
            Assert.AreEqual(first, func());
            Assert.AreEqual(first, o);
        }

        [Test]
        public void ToSelfAndToWorks()
        {
            _kernel.Bind<List<int>>().ToSelf().InSingletonScope();
            _kernel.Bind<IList<int>>().To<List<int>>();
            _kernel.Bind<ICollection<int>>().To<IList<int>>();
            var list = _kernel.Get<List<int>>();
            Assert.AreEqual(list, _kernel.Get<ICollection<int>>());
            Assert.AreEqual(list, _kernel.Get<IList<int>>());
            Assert.AreEqual(list, _kernel.Get<ICollection<int>>());
            Assert.AreEqual(list, _kernel.Get<IList<int>>());
            Assert.AreEqual(list, _kernel.Get<List<int>>());
            Assert.AreEqual(list, _kernel.Get<Func<List<int>>>()());
            Assert.AreEqual(list, _kernel.Get<Func<ICollection<int>>>()());
            Assert.AreEqual(list, _kernel.Get<Func<ICollection<int>>>()());
        }

        [Test]
        public void ThrowsIfNoBinding()
        {
            Assert.Throws<InvalidOperationException>(() => _kernel.Get<int>());
            Assert.Throws<InvalidOperationException>(() => _kernel.Get<string>());
            _kernel.Bind<object>().To<string>();
            Assert.Throws<InvalidOperationException>(() => _kernel.Get<object>());
        }

        [Test]
        public void CreatesInstanceWithNoBinding()
        {
            var o1 = _kernel.Get<object>();
            Assert.IsNotNull(o1);
            var o2 = _kernel.Get<object>();
            Assert.IsNotNull(o2);
            Assert.AreNotEqual(o1, o2);
        }
    }
}