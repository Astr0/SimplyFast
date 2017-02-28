using Xunit;

namespace SimplyFast.Log.Tests
{
    public class SeverityTests
    {
        private static void ShouldLog(Severity test, Severity should)
        {
            Assert.True(test.ShouldLog(should));
            var msg = MessageEx.Default(should);
            Assert.True(test.ShouldLog(msg));
        }

        private static void ShouldNotLog(Severity test, Severity shouldNot)
        {
            Assert.False(test.ShouldLog(shouldNot));
            var msg = MessageEx.Default(shouldNot);
            Assert.False(test.ShouldLog(msg));
        }

        [Fact]
        public void OffLogOk()
        {
            const Severity test = Severity.Off;
            ShouldNotLog(test, Severity.Debug);
            ShouldNotLog(test, Severity.Info);
            ShouldNotLog(test, Severity.Warn);
            ShouldNotLog(test, Severity.Error);
            ShouldNotLog(test, Severity.Fatal);
            ShouldNotLog(test, Severity.Off);
        }

        [Fact]
        public void FatalLogOk()
        {
            const Severity test = Severity.Fatal;
            ShouldNotLog(test, Severity.Debug);
            ShouldNotLog(test, Severity.Info);
            ShouldNotLog(test, Severity.Warn);
            ShouldNotLog(test, Severity.Error);
            ShouldLog(test, Severity.Fatal);
            ShouldNotLog(test, Severity.Off);
        }

        [Fact]
        public void ErrorLogOk()
        {
            const Severity test = Severity.Error;
            ShouldNotLog(test, Severity.Debug);
            ShouldNotLog(test, Severity.Info);
            ShouldNotLog(test, Severity.Warn);
            ShouldLog(test, Severity.Error);
            ShouldLog(test, Severity.Fatal);
            ShouldNotLog(test, Severity.Off);
        }

        [Fact]
        public void WarnLogOk()
        {
            const Severity test = Severity.Warn;
            ShouldNotLog(test, Severity.Debug);
            ShouldNotLog(test, Severity.Info);
            ShouldLog(test, Severity.Warn);
            ShouldLog(test, Severity.Error);
            ShouldLog(test, Severity.Fatal);
            ShouldNotLog(test, Severity.Off);
        }

        [Fact]
        public void InfoLogOk()
        {
            const Severity test = Severity.Info;
            ShouldNotLog(test, Severity.Debug);
            ShouldLog(test, Severity.Info);
            ShouldLog(test, Severity.Warn);
            ShouldLog(test, Severity.Error);
            ShouldLog(test, Severity.Fatal);
            ShouldNotLog(test, Severity.Off);
        }

        [Fact]
        public void DebugLogOk()
        {
            const Severity test = Severity.Debug;
            ShouldLog(test, Severity.Debug);
            ShouldLog(test, Severity.Info);
            ShouldLog(test, Severity.Warn);
            ShouldLog(test, Severity.Error);
            ShouldLog(test, Severity.Fatal);
            ShouldNotLog(test, Severity.Off);
        }
    }
}