namespace Plisky.Diagnostics.Test {

    using System.Linq;

#pragma warning disable SA1600

    using Plisky.Diagnostics.Listeners;
    using Xunit;

    [Collection(nameof(ParalellEnabledTestCollection))]
    public class ListenerCreateTests {
#if false

 Jan 2022, currently visual studio can not run tests or any tests where the inline data with
 () is included, it causes the test filter to fail.

        [Theory(DisplayName = nameof(ExploratoryX))]
        [InlineData("(TEX)(TEX)(TEX;A,B,C)", '(', ')', 3)]
        [InlineData("Boneo (TEX)(TEX)(TEX;A,B,C)", '(', ')', 3)]
        [InlineData("()()()", '(', ')', 3)]
        [InlineData("xxxxx", '[', ']', 0)]
        public void ChunkifyTest2(string initString, char opener, char closer, int handlerStringCount) {
            var res = BilgeConfigure.Chunkify(initString, opener, closer);

            Assert.Equal(handlerStringCount, res.Length);
        }

#endif

        [Theory(DisplayName = nameof(Resolver))]
        [InlineData("[TLV:Off][RSL:xxxxx]", "xxxxx")]
        public void Resolver(string initString, string handlerStringCount) {
            var mbc = new MockBilgeConfigure();
            BilgeConfigure bc = mbc;
            bc.From.StringValue(initString);

            Assert.Equal(handlerStringCount, bc.Configuration.ResolverInitString);
        }

        [Fact(DisplayName = nameof(Apply_TCPListener_AddsListener))]
        public void Apply_TCPListener_AddsListener() {
            var mbc = new MockBilgeConfigure();
            BilgeConfigure bc = mbc;

            bc.Configuration.AddHandler("TCP:127.0.0.1,9060,true");
            bc.RegisterHandler(typeof(TCPHandler));

            bc.Apply();

            Bilge b = new Bilge();
            var lst = b.GetListeners();

            Assert.True(lst.Count() == 1);
            Assert.True(lst.First().GetType() == typeof(TCPHandler));
        }

        [Theory(DisplayName = nameof(SimpleListener_CreateTests))]
        [InlineData("SFL:Apple", true)]
        public void SimpleListener_CreateTests(string initString, bool shouldCreate) {
            var res = SimpleTraceFileHandler.InitiliaseFrom(initString);

            if (shouldCreate) {
                Assert.NotNull(res);
            } else {
                Assert.Null(res);
            }
        }

        [Theory(DisplayName = nameof(RFSH_SizeLimitString_Conversion))]
        [InlineData("1", 1)]
        [InlineData("1000", 1000)]
        [InlineData("1kb", 1024)]
        [InlineData("300kb", 307200)]
        [InlineData("1mb", 1048576)]
        [InlineData("10mb", 10485760)]
        public void RFSH_SizeLimitString_Conversion(string textValue, long sizeValue) {
            var roro = new RollingFSHandlerOptions() {
            };

            roro.MaxRollingFileSize = textValue;

            Assert.Equal(sizeValue, roro.FileSizeLimit);
        }

        [Theory(DisplayName = nameof(RFSH_InitialisationString_Works))]
        [InlineData("RFS:log_%dd%mm%yy_%hh%mi%ss_%nn.log", true, "log_%dd%mm%yy_%hh%mi%ss_%nn.log", "10mb")]
        [InlineData("RFS:log_%dd%mm%yy_%hh%mi%ss_%nn.log,50mb", true, "log_%dd%mm%yy_%hh%mi%ss_%nn.log", "50mb")]
        [InlineData("RFS:log_%dd%mm%yy_%hh%mi%ss_%nn.log,none", true, "log_%dd%mm%yy_%hh%mi%ss_%nn.log", "")]
        public void RFSH_InitialisationString_Works(string initString, bool isMask, string expectedMask, string expectedSize) {
            var roro = new RollingFSHandlerOptions() {
                InitialisationString = initString
            };

            roro.Parse();

            Assert.Equal(expectedMask, roro.FileName);
            Assert.Equal(isMask, roro.FilenameIsMask);
            Assert.Equal(expectedSize, roro.MaxRollingFileSize);
            Assert.True(roro.CanCreate);
        }

        [Theory(DisplayName = nameof(BadInitialisationString_WillNotCreate))]
        [InlineData("log_%dd%mm%yy_%hh%mi%ss_%nn.log")]
        [InlineData("log.txt")]
        [InlineData("RFS(log)")]
        [InlineData("RFSH:")]
        [InlineData("TCP:")]
        public void BadInitialisationString_WillNotCreate(string initString) {
            var roro = new RollingFSHandlerOptions() {
                InitialisationString = initString
            };

            roro.Parse();

            Assert.False(roro.CanCreate);
        }

        // TODO: Filename is not mask, directfilename returned
        // TODO: Filename not mask, %causes error
        // TODO: Contans %NN and %AB is an error

        // Note invalid date combinations here (like 0 in the parameters) are an issue with the mock not the underlying code.
        [Theory(DisplayName = nameof(RollingFileHandler_NameGeneration_Works))]
        [InlineData("log_%dd%mm%yy_%hh%mi%ss_%nn.log", "log_10112021_121212_01.log", 10, 11, 2021, 12, 12, 12, "")]
        [InlineData("log_%dd%mm%yy_%hh%mi%ss_%nn.bog", "log_10112021_121212_01.bog", 10, 11, 2021, 12, 12, 12, "")]
        [InlineData("log_%dd%mm%yy_%hh%mi%ss_%nn", "log_10112021_121212_01.log", 10, 11, 2021, 12, 12, 12, "")]
        [InlineData("log_%dd%MM%yy.bog", "log_10112021.bog", 10, 11, 2021, 12, 12, 12, "")]
        [InlineData("log_%DD%mm%yY.bog", "log_10112021.bog", 10, 11, 2021, 12, 12, 12, "")]
        [InlineData("log_%dd.bog", "log_20.bog", 20, 1, 2000, 12, 12, 12, "")]
        [InlineData("logfile_%nn.bog", "logfile_01.bog", 10, 11, 2021, 12, 12, 12, "")]
        [InlineData("logfile", "logfile.log", 10, 11, 2021, 12, 12, 12, "")]
        [InlineData("logfile_%AB", "logfile_A.log", 10, 11, 2021, 12, 12, 12, "")]
        [InlineData("logfile_%PID", "logfile_5678.log", 10, 11, 2021, 12, 12, 12, "5678")]
        [InlineData("log_%dd%mm%yy_%hh%mi%ss_%nn_%pid", "log_10112021_121212_01_5678.log", 10, 11, 2021, 12, 12, 12, "5678")]
        public void RollingFileHandler_NameGeneration_Works(string mask, string expected, int dd, int mm, int yy, int hh, int min, int sec, string pid) {
            var roro = new RollingFSHandlerOptions() {
                FileName = mask
            };
            var mrfs = new MockRollingFileSystemHandler(roro);
            mrfs.SetPid(pid);
            mrfs.SetDate(dd, mm, yy);
            mrfs.SetTime(hh, min, sec);

            RollingFileSystemHandler sut = mrfs;

            var fn = sut.GetFilenameFromMask(string.Empty, roro.FileName);

            Assert.Equal(expected, fn);
        }

        [Fact(DisplayName = nameof(FileNumber_IncrementsWhenFilesExist))]
        public void FileNumber_IncrementsWhenFilesExist() {
            var roro = new RollingFSHandlerOptions() {
                FileName = "boglog_%nn.bog"
            };
            var mrfs = new MockRollingFileSystemHandler(roro);
            mrfs.AddExistingFile("c:\\boglog_01.bog");
            mrfs.AddExistingFile("c:\\boglog_02.bog");

            RollingFileSystemHandler sut = mrfs;

            var fn = sut.GetFilenameFromMask("c:\\", roro.FileName);

            Assert.Equal("c:\\boglog_03.bog", fn);
        }

        [Theory(DisplayName = nameof(TcpListener_CreateTests))]
        [InlineData("TCP:www.boblet.com,9060,true", true, "www.boblet.com", 9060)]
        [InlineData("TCP:127.0.0.1,9060,true", true, "127.0.0.1", 9060)]
        [InlineData("BBB:127.0.0.1,9060,true", false, "", 0)]
        [InlineData("TCP:127.0.0.1,9060,false", true, "127.0.0.1", 9060)]
        [InlineData("TCP:127.0.0.1,Apple,true", false, "", 0)]
        public void TcpListener_CreateTests(string initString, bool shouldCreate, string targetIp, int targetPort) {
            TCPHandler res = TCPHandler.InitiliaseFrom(initString);

            if (shouldCreate) {
                Assert.NotNull(res);
                Assert.Equal(targetIp, res.DestinationAddress);
                Assert.Equal(targetPort, res.DestinationPort);
            } else {
                Assert.Null(res);
            }
        }
    }

#pragma warning restore SA1600
}