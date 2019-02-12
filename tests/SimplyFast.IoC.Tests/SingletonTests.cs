using SimplyFast.IoC.Tests.TestData;
using Xunit;

namespace SimplyFast.IoC.Tests
{
    public class SingletonTests
    {
        private readonly IKernel _kernel;

        public  SingletonTests()
        {
            _kernel = KernelEx.Create();
        }

        [Fact]
        public void SingletonIsSingleton()
        {
            _kernel.Bind<object>().ToSelf().InSingletonScope();

            var o1 = _kernel.Get<object>();
            var o2 = _kernel.Get<object>();
            Assert.True(ReferenceEquals(o1, o2));
        }

        [Fact]
        public void SingletonReturnedFromMainKernel()
        {
            _kernel.Bind<char>().ToConstant('c');
            _kernel.Bind<long>().ToConstant(12);
            _kernel.Bind<SomeClass>().ToSelf().InSingletonScope();

            var o1 = _kernel.Get<SomeClass>();
            var o2 = _kernel.Get<SomeClass>(BindArg.Typed("test"));
            Assert.True(ReferenceEquals(o1, o2));
        }

        [Fact]
        public void SingletonCreatedOnMainKernel()
        {
            _kernel.Bind<char>().ToConstant('c');
            _kernel.Bind<long>().ToConstant(12);
            _kernel.Bind<SomeClass>().ToSelf().InSingletonScope();

            var o1 = _kernel.Get<SomeClass>(BindArg.Typed("test"));
            var o2 = _kernel.Get<SomeClass>();
            Assert.True(ReferenceEquals(o1, o2));
            Assert.Null(o2.Str);
        }
    }
}