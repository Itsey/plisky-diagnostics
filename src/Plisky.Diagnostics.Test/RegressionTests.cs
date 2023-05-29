namespace Plisky.Diagnostics.Test {

    using System.Diagnostics;
    using System.Threading;
    using Plisky.Diagnostics;
    using Plisky.Diagnostics.Copy;
    using Xunit;

    [Collection(nameof(QueueSensitiveTestCollectionDefinition))]
    public class RegressionTests {

        [Fact]
        [Trait("V", "2")]
        [Trait("age", "regression")]
        public void Context_FromConstructorReachesMessage() {
            var mkHandler = new MockRouter();

            string dummyContext = "xxCtxtxx";
            mkHandler.AssertContextIs(dummyContext);
            var sut = TestHelper.GetBilgeAndClearDown(mkHandler, context: dummyContext, sl: SourceLevels.Verbose);

            sut.Info.Log("Message should have context");

            mkHandler.AssertAllConditionsMetForAllMessages(true);
        }

        [Fact]
        [Trait(Traits.Age, Traits.Regression)]
        public void Default_TraceLevelIsOff() {
            var sut = TestHelper.GetBilgeAndClearDown();
            Assert.Equal<SourceLevels>(SourceLevels.Off, sut.ActiveTraceLevel);
        }

        [Fact]
        [Trait(Traits.Age, Traits.Regression)]
        public void InfoNotLogged_IfNotInfo() {
            var mmh = new MockMessageHandler();
            var b = TestHelper.GetBilgeAndClearDown();
            b.ActiveTraceLevel = SourceLevels.Warning;
            b.AddHandler(mmh);

            b.Info.Log("msg");
            b.Verbose.Log("Msg");

            b.Flush();
            Assert.Equal<int>(0, mmh.TotalMessagesRecieved);
        }

        [Fact]
        [Trait("V", "2")]
        [Trait(Traits.Age, Traits.Regression)]
        public void MethodName_MatchesThisMethodName() {
            var mkHandler = new MockRouter();
            mkHandler.SetMethodNameMustContain(nameof(MethodName_MatchesThisMethodName));
            var b = TestHelper.GetBilgeAndClearDown(mkHandler, sl: SourceLevels.Verbose);

            b.Info.Log("This is a message");

            mkHandler.AssertAllConditionsMetForAllMessages(true);
        }

        [Fact]
        [Trait(Traits.Age, Traits.Regression)]
        public void MockMessageHandlerStartsEmpty() {
            var mmh = new MockMessageHandler();
            Assert.True(mmh.TotalMessagesRecieved == 0, "There should be no messages to start with");
        }

        [Fact]
        [Trait(Traits.Age, Traits.Regression)]
        public void MultiHandlers_RecieveMultiMessages() {
            var mmh1 = new MockMessageHandler();
            var mmh2 = new MockMessageHandler();
            var sut = TestHelper.GetBilgeAndClearDown(sl: SourceLevels.Verbose);
            sut.AddHandler(mmh1);
            sut.AddHandler(mmh2);

            sut.Info.Flow();
            sut.Flush();

            mmh1.AssertAllConditionsMetForAllMessages(true);
            mmh2.AssertAllConditionsMetForAllMessages(true);
        }

        [Fact]
        [Trait(Traits.Age, Traits.Regression)]
        public void NothingWrittenWhenTraceOff() {
            var mmh = new MockMessageHandler();
            var sut = TestHelper.GetBilgeAndClearDown();
            sut.ActiveTraceLevel = SourceLevels.Off;
            sut.AddHandler(mmh);

            sut.Info.Log("msg");
            sut.Verbose.Log("Msg");
            sut.Error.Log("Msg");
            sut.Warning.Log("Msg");

            sut.Flush();
            Assert.Equal<int>(0, mmh.TotalMessagesRecieved);
        }

        [Fact]
        [Trait("V", "2")]
        [Trait("age", "regression")]
        public void ProcessId_IsCorrectProcessId() {
            const string PROCID = "mockprocid";
            var mkHandler = new MockRouter(PROCID);
            int testProcId = Process.GetCurrentProcess().Id;
            mkHandler.AssertProcessId(PROCID);
            mkHandler.AssertManagedThreadId(Thread.CurrentThread.ManagedThreadId);
            var sut = TestHelper.GetBilgeAndClearDown(mkHandler, sl: SourceLevels.Verbose);

            sut.Info.Log("Message Written");

            mkHandler.AssertAllConditionsMetForAllMessages(true);
        }

        [Fact]
        [Trait(Traits.Age, Traits.Regression)]
        public void QueuedMessagesNotWrittenIfWriteOnFailSet() {
            var mmh = new MockMessageHandler();
            var b = TestHelper.GetBilgeAndClearDown();
            b.AddHandler(mmh);
            b.WriteOnFail = true;
            WriteASeriesOfMessages(b);
            b.Flush();
            Assert.Equal<int>(0, mmh.TotalMessagesRecieved);
        }

        [Fact]
        [Trait(Traits.Age, Traits.Regression)]
        public void QueuedMessagesWritten_AfterFlush() {
            var mmh = new MockMessageHandler();
            var sut = TestHelper.GetBilgeAndClearDown(sl: SourceLevels.Verbose, simplify: false);
            sut.AddHandler(mmh);
            sut.WriteOnFail = true;
            WriteASeriesOfMessages(sut);

            Assert.Equal<int>(0, mmh.TotalMessagesRecieved);
            sut.TriggerWrite();
            sut.Flush();

            Thread.Sleep(1);
            Thread.Sleep(1);
            Thread.Sleep(1);
            mmh.AssertAllConditionsMetForAllMessages(true);
        }

        [Fact]
        [Trait(Traits.Age, Traits.Regression)]
        public void TraceLevel_Constructor_GetsSet() {
            var b = new Bilge(tl: SourceLevels.Error, resetDefaults: true);
            Assert.Equal<SourceLevels>(SourceLevels.Error, b.ActiveTraceLevel);
        }

        [Fact]
        [Trait(Traits.Age, Traits.Regression)]
        public void VerboseNotLogged_IfNotVerbose() {
            var mmh = new MockMessageHandler();
            var sut = TestHelper.GetBilgeAndClearDown();
            sut.ActiveTraceLevel = SourceLevels.Information;
            sut.AddHandler(mmh);
            sut.Verbose.Log("Msg");
            Assert.Equal<int>(0, mmh.TotalMessagesRecieved);
        }

        [Fact]
        [Trait(Traits.Age, Traits.Regression)]
        public void WriteOnFail_DefaultsFalse() {
            var b = TestHelper.GetBilgeAndClearDown();
            Assert.False(b.WriteOnFail, "The write on fail must default to false");
        }

        private void WriteASeriesOfMessages(Bilge b) {
            b.Info.Log("Test message");
            b.Verbose.Log("test message");
            b.Error.Log("Test message");
            b.Warning.Log("Test message");
        }
    }
}