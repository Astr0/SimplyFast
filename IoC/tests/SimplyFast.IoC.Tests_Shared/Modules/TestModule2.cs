using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SF.IoC;
using SF.IoC.Modules;

namespace SF.Tests.IoC.Modules
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