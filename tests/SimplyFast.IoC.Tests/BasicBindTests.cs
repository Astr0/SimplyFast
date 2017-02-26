using System;
using System.Collections.Generic;
using Xunit;

namespace SimplyFast.IoC.Tests
{
    
    public class BasicBindTests
    {
        private IKernel _kernel;

        public BasicBindTests()
        {
            _kernel = new FastKernel();
        }

        [Fact]
        public void KernelBindsItself()
        {
            Assert.Equal(_kernel, _kernel.Get<IKernel>());
            Assert.Equal(_kernel, _kernel.Get<IGetKernel>());
        }

        [Fact]
        public void KernelBindsItselfAsFactory()
        {
            Assert.Equal(_kernel, _kernel.Get<Func<IKernel>>()());
            Assert.Equal(_kernel, _kernel.Get<Func<IGetKernel>>()());
        }

        [Fact]
        public void ConstantBindingWorks()
        {
            var o = new object();
            _kernel.Bind<object>().ToConstant(o);
            Assert.Equal(o, _kernel.Get<object>());
            Assert.Equal(o, _kernel.Get<Func<object>>()());
        }

        [Fact]
        public void MethodBindingWorks()
        {
            var i = 0;
            _kernel.Bind<int>().ToMethod(c => i++);
            Assert.Equal(0, i);
            Assert.Equal(0, _kernel.Get<int>());
            Assert.Equal(1, i);
            Assert.Equal(1, _kernel.Get<int>());
            Assert.Equal(2, i);
            var func = _kernel.Get<Func<int>>();
            Assert.Equal(2, i);
            Assert.Equal(2, func());
            Assert.Equal(3, i);
            Assert.Equal(3, func());
            Assert.Equal(4, i);
        }

        [Fact]
        public void NinjectSytaxOk()
        {
            _kernel.Bind<string>().ToConstructor(c => "test");
            Assert.Equal("test", _kernel.Inject<string>());
        }

        [Fact]
        public void SingletonBindingWorksForStruct()
        {
            var i = 0;
            _kernel.Bind<int>().ToMethod(c => i++).InSingletonScope();
            Assert.Equal(0, i);
            Assert.Equal(0, _kernel.Get<int>());
            Assert.Equal(1, i);
            Assert.Equal(0, _kernel.Get<int>());
            Assert.Equal(1, i);
            var func = _kernel.Get<Func<int>>();
            Assert.Equal(1, i);
            Assert.Equal(0, func());
            Assert.Equal(1, i);
            Assert.Equal(0, func());
            Assert.Equal(1, i);
        }

        [Fact]
        public void SingletonBindingWorksForClass()
        {
            object o = null;
            _kernel.Bind<object>().ToMethod(c => o = new object()).InSingletonScope();
            Assert.Null(o);
            var first = _kernel.Get<object>();
            Assert.Equal(o, first);
            Assert.Equal(first, _kernel.Get<object>());
            Assert.Equal(first, o);
            var func = _kernel.Get<Func<object>>();
            Assert.Equal(first, o);
            Assert.Equal(first, func());
            Assert.Equal(first, o);
            Assert.Equal(first, func());
            Assert.Equal(first, o);
        }

        [Fact]
        public void ToSelfAndToWorks()
        {
            _kernel.Bind<List<int>>().ToSelf().InSingletonScope();
            _kernel.Bind<IList<int>>().To<List<int>>();
            _kernel.Bind<ICollection<int>>().To<IList<int>>();
            var list = _kernel.Get<List<int>>();
            Assert.Equal(list, _kernel.Get<ICollection<int>>());
            Assert.Equal(list, _kernel.Get<IList<int>>());
            Assert.Equal(list, _kernel.Get<ICollection<int>>());
            Assert.Equal(list, _kernel.Get<IList<int>>());
            Assert.Equal(list, _kernel.Get<List<int>>());
            Assert.Equal(list, _kernel.Get<Func<List<int>>>()());
            Assert.Equal(list, _kernel.Get<Func<ICollection<int>>>()());
            Assert.Equal(list, _kernel.Get<Func<ICollection<int>>>()());
        }

        [Fact]
        public void ThrowsIfNoBinding()
        {
            Assert.Throws<InvalidOperationException>(() => _kernel.Get<int>());
            Assert.Throws<InvalidOperationException>(() => _kernel.Get<string>());
            _kernel.Bind<object>().To<string>();
            Assert.Throws<InvalidOperationException>(() => _kernel.Get<object>());
        }

        [Fact]
        public void CreatesInstanceWithNoBinding()
        {
            var o1 = _kernel.Get<object>();
            Assert.NotNull(o1);
            var o2 = _kernel.Get<object>();
            Assert.NotNull(o2);
            Assert.NotEqual(o1, o2);
        }
    }
}