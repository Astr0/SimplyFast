using NUnit.Framework;

namespace SimplyFast.Reflection.Tests
{
    public class ReflectionTests
    {
        protected static void CheckPrivateAccess()
        {
            if (!MemberInfoEx.PrivateAccess)
                Assert.Ignore("Private access disabled");
        }
    }
}