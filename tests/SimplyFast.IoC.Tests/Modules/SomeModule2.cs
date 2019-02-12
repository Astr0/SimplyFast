using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SimplyFast.IoC.Modules;

namespace SimplyFast.IoC.Tests.Modules
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class SomeModule2: FastModule
    {
        public override void Load()
        {
            Bind<List<string>>().ToConstructor(c => new List<string>(c.Get<IEnumerable<string>>()));
        }
    }
}