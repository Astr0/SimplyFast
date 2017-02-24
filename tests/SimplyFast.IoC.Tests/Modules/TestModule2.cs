using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SimplyFast.IoC.Modules;

namespace SimplyFast.IoC.Tests.Modules
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class TestModule2: FastModule
    {
        public override void Load()
        {
            Bind<List<string>>().ToSelf();
        }
    }
}