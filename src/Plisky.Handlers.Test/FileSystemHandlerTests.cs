namespace Plisky.Diagnostics.Test {

    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
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


        [Fact]
        [Trait(Traits.Style, Traits.Integration)]
        public async Task RFSH_DebugTest() {
            string fname = @"D:\temp\_scratch\logs\debuglog.log";

            if (File.Exists(fname)) {
                File.Delete(fname);
            }
            int howManyActive = 0;
            var sut = new RollingFileSystemHandler(new RollingFSHandlerOptions() {
                Directory = @"D:\temp\_scratch\logs\",
                FileName = "debuglog.log",
                MaxRollingFileSize = "120mb"
            });

            for(int i= 0; i < 100000; i++) {
                MessageMetadata md = new MessageMetadata();
                md.Body = "This is a test message to see how the rolling file system handler works in practice.  This is line PP" + i.ToString();
                md.CommandType = TraceCommandTypes.LogMessage;
                ThreadPool.QueueUserWorkItem(async (o) => {
                    int c = Interlocked.Increment(ref howManyActive);
                    await sut.HandleMessageAsync(new MessageMetadata[] { md });
                    Interlocked.Decrement(ref howManyActive);
                });
                
            }

            while (howManyActive > 0) {
                await Task.Delay(10);
            }
            Assert.True(sut.GetStatus() == "OK");
            // TOO SLOW
            var x = File.ReadAllLines(fname);
            int count = 0;
            while (count < 100000) {
                bool countFound = false;
                foreach (var l in x) {
                    countFound = l.Contains($"PP{count}");
                    if (countFound) {
                        break;
                    }
                }
                Assert.True(countFound, $"Did not find line PP{count}");
                count++;
            }
            
            Assert.Equal(100000, count);
        }
    }
}