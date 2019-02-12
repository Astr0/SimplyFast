using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SimplyFast.IoC.Tests.Modules
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class SomeModule: FastModule
    {
        public class SomeEnumerable : IEnumerable<string>
        {
            public IEnumerator<string> GetEnumerator()
            {
                yield return "str1";
                yield return "str2";
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public override void Load()
        {
            Bind<string>().ToConstant("test");
            Bind<IEnumerable<string>>().To<SomeEnumerable>();
        }
    }
}