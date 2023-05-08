namespace Plisky.Diagnostics.Test {

    using System.IO;
    using Plisky.Diagnostics.Copy;
    using Plisky.Diagnostics.Listeners;

    using Xunit;

    [Collection(nameof(ParalellEnabledTestCollection))]
    public class FileSystemHandlerTests {

        [Fact]
        [Trait(Traits.Age, Traits.Regression)]
        public void SimpleHandler_AppendsDate() {
            string wellKnownLogfileName = "bilgedefault.log";
            string expectedDestination = Path.Combine(Path.GetTempPath(), wellKnownLogfileName);
            var pfsh = new SimpleTraceFileHandler(overwriteEachTime: false);
            Assert.NotEqual(expectedDestination, pfsh.TraceFilename);
        }

        [Fact]
        [Trait(Traits.Age, Traits.Regression)]
        public void SimpleHandler_WellKnownFile() {
            string wellKnownLogfileName = "bilgedefault.log";
            string expectedDestination = Path.Combine(Path.GetTempPath(), wellKnownLogfileName);
            var pfsh = new SimpleTraceFileHandler();
            Assert.Equal(expectedDestination, pfsh.TraceFilename);
        }
    }
}