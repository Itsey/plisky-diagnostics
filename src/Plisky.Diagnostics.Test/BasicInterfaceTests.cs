namespace Plisky.Diagnostics.Test {

    using System;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Linq;
    using System.Threading;
    using Plisky.Diagnostics;
    using Plisky.Diagnostics.Copy;
    using Xunit;

    [Collection(nameof(QueueSensitiveTestCollectionDefinition))]
    public class BasicInterfaceTests {

        public BasicInterfaceTests() {
        }

        [Fact(DisplayName = nameof(AddHandler_DoesAddHandler))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void AddHandler_DoesAddHandler() {
            _ = TestHelper.GetBilgeAndClearDown();
            string s = Process.GetCurrentProcess().ProcessName;
            bool worked = Bilge.AddHandler(new MockMessageHandler());
            int count = Bilge.GetHandlers().Count();

            Assert.True(worked);
            Assert.Equal(1, count);
        }

        [Fact(DisplayName = nameof(AddHandler_DuplicateAddsTwoHandlers))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void AddHandler_DuplicateAddsTwoHandlers() {
            _ = TestHelper.GetBilgeAndClearDown();

            bool worked = Bilge.AddHandler(new MockMessageHandler());
            worked &= Bilge.AddHandler(new MockMessageHandler());

            Assert.True(worked);
            int ct = Bilge.GetHandlers().Count();
            Assert.Equal(2, ct);
        }

        [Fact(DisplayName = nameof(AddHandler_DuplicateByNameFailsOnSecond))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void AddHandler_DuplicateByNameFailsOnSecond() {
            _ = TestHelper.GetBilgeAndClearDown();

            Bilge.AddHandler(new MockMessageHandler());
            Bilge.AddHandler(new MockMessageHandler());

            int ct = Bilge.GetHandlers().Count();
            Assert.Equal(2, Bilge.GetHandlers().Count());
        }


       

        [Fact(DisplayName = nameof(AddHandler_SingleName_AddsDifferentNames))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void AddHandler_SingleName_AddsDifferentNames() {
            _ = TestHelper.GetBilgeAndClearDown();

            Bilge.AddHandler(new MockMessageHandler("arfle"), HandlerAddOptions.SingleName);
            Bilge.AddHandler(new MockMessageHandler("barfle"), HandlerAddOptions.SingleName);
            int ct = Bilge.GetHandlers().Count();

            Assert.Equal(2, ct);
        }

        [Fact(DisplayName = nameof(AddHandler_SingleName_DoesNotAddSecondName))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void AddHandler_SingleName_DoesNotAddSecondName() {
            _ = TestHelper.GetBilgeAndClearDown();

            Bilge.AddHandler(new MockMessageHandler("arfle"), HandlerAddOptions.SingleName);
            Bilge.AddHandler(new MockMessageHandler("arfle"), HandlerAddOptions.SingleName);
            int ct = Bilge.GetHandlers().Count();

            Assert.Equal(1, ct);
        }

        [Fact(DisplayName = nameof(AddHandler_SingleType_DoesNotAddSecond))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void AddHandler_SingleType_DoesNotAddSecond() {
            _ = TestHelper.GetBilgeAndClearDown();

            Bilge.AddHandler(new MockMessageHandler(), HandlerAddOptions.SingleType);
            Bilge.AddHandler(new MockMessageHandler(), HandlerAddOptions.SingleType);
            int ct = Bilge.GetHandlers().Count();

            Assert.Equal(1, ct);
        }

        [Fact]
        [Trait(Traits.Age, Traits.Regression)]
        public void Assert_False_DoesNothingIfFalse() {
            var mmh = new MockMessageHandler();
            var sut = TestHelper.GetBilgeAndClearDown();
            sut.AddHandler(mmh);
            sut.Assert.True(true);
            Assert.Equal(0, mmh.AssertionMessageCount);
        }

        // TODO : Investigate this failure
        [Fact(Skip ="Fails since move to GH")]
        [Trait(Traits.Age, Traits.Regression)]
        public void Assert_False_FailsIfTrue() {
            var mmh = new MockMessageHandler();
            var sut = TestHelper.GetBilgeAndClearDown();
            sut.Assert.ConfigureAsserts(AssertionStyle.Nothing);
            sut.AddHandler(mmh);
            sut.Assert.True(false);

            sut.Flush();

            Assert.True(mmh.AssertionMessageCount > 0);
        }

        // TODO : Investigate this failure
        [Fact(Skip ="Intermittent Failure")] 
        [Trait(Traits.Age, Traits.Regression)]
        public void Assert_True_DoesFailsIfFalse() {
            var mmh = new MockMessageHandler();
            var sut = TestHelper.GetBilgeAndClearDown();
            sut.AddHandler(mmh);

            sut.Assert.True(false);

            sut.Flush().Wait();

            Assert.True(mmh.AssertionMessageCount > 0);
        }

        [Fact]
        [Trait(Traits.Age, Traits.Regression)]
        public void Assert_True_DoesNothingIfTrue() {
            var mmh = new MockMessageHandler();
            var sut = TestHelper.GetBilgeAndClearDown();
            sut.AddHandler(mmh);

            sut.Assert.True(true);
            Assert.Equal(0, mmh.AssertionMessageCount);
        }

        [Fact]
        [Trait(Traits.Age, Traits.Regression)]
        public void Bilge_EnterSection_TracesSection() {
            var mmh = new MockMessageHandler();
            var sut = TestHelper.GetBilgeAndClearDown(sl: SourceLevels.Verbose);
            sut.AddHandler(mmh);
            mmh.SetMethodNameMustContain("monkeyfish");
            sut.Info.EnterSection("random sectiion", "monkeyfish");
            sut.Flush().Wait();

            mmh.AssertAllConditionsMetForAllMessages(true, true);
        }

        [Fact]
        [Trait(Traits.Age, Traits.Regression)]
        public void Bilge_LeaveSection_TracesSection() {
            var mmh = new MockMessageHandler();
            var sut = TestHelper.GetBilgeAndClearDown(sl: SourceLevels.Verbose);

            sut.AddHandler(mmh);

            mmh.SetMethodNameMustContain("bannanaball");
            sut.Info.LeaveSection("bannanaball");

            sut.Flush().Wait();

            mmh.AssertAllConditionsMetForAllMessages(true, true);
        }

        [Fact(DisplayName = nameof(DirectWrite_IsPossible))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void DirectWrite_IsPossible() {
            var sut = TestHelper.GetBilgeAndClearDown();
            sut.DisableMessageBatching();
            sut.ActiveTraceLevel = SourceLevels.Verbose;

            var mmh = new MockMessageHandler();
            sut.AddHandler(mmh);

            sut.Direct.Write("DirectMessage", "DirectFurther");
            sut.Flush().Wait();

            Assert.True(mmh.AssertThisMessageMustExist("DirectMessage"));
        }

        [Fact(DisplayName = nameof(GetHandlers_DefaultReturnsAll))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void GetHandlers_DefaultReturnsAll() {
            _ = TestHelper.GetBilgeAndClearDown();

            Bilge.AddHandler(new MockMessageHandler());
            Bilge.AddHandler(new MockMessageHandler());
            Bilge.AddHandler(new MockMessageHandler());
            Bilge.AddHandler(new MockMessageHandler());

            Assert.Equal(4, Bilge.GetHandlers().Count());
        }

        [Fact(DisplayName = nameof(GetHandlers_FilterReturnsNamed))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void GetHandlers_FilterReturnsNamed() {
            _ = TestHelper.GetBilgeAndClearDown();

            Bilge.AddHandler(new MockMessageHandler("arfle"));
            Bilge.AddHandler(new MockMessageHandler("barfle"));
            int count = Bilge.GetHandlers("arf*").Count();

            Assert.Equal(1, count);
        }

        [Fact(DisplayName = nameof(MessageBatching_Works_Default1))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void MessageBatching_Works_Default1() {
            var sut = TestHelper.GetBilgeAndClearDown();
            sut.ActiveTraceLevel = SourceLevels.Information;
            var mmh = new MockMessageHandler();
            sut.AddHandler(mmh);

            sut.Info.Log("Dummy Message");
            sut.Flush().Wait();
            sut.Info.Log("Dummy Message");
            sut.Flush().Wait();
            Assert.Equal(1, mmh.LastMessageBatchSize);
        }

        [Fact(DisplayName = nameof(MessageBatching_Works_Enabled))]
        public void MessageBatching_Works_Enabled() {
            const int MESSAGE_BATCHSIZE = 10;

            var sut = TestHelper.GetBilgeAndClearDown();

            sut.SetMessageBatching(MESSAGE_BATCHSIZE, 500000);

            sut.ActiveTraceLevel = SourceLevels.Information;
            var mmh = new MockMessageHandler();
            sut.AddHandler(mmh);

            for (int i = 1; i < 100; i++) {
                sut.Info.Log("Dummy Message");

                if (i % 25 == 0) {
                    // The flush forces the write, this is needed otherwise it bombs through
                    // too fast for more than one write to the handler to occur.
                    sut.Flush().Wait();
                }

                if (mmh.TotalMessagesRecieved > 0) {
                    // Any time that we get a batch it must be at least MESSAGE_BATCHSIZE msgs.  However it is only sure to happen
                    // the first few times.  When we run out of messages the batch can be < minimimum as flush forces the write.
                    Assert.True(mmh.LastMessageBatchSize >= MESSAGE_BATCHSIZE, $"Batch Size NotBig Enough at {i} batch Size {mmh.LastMessageBatchSize}");
                    break;
                }
            }
        }

        [Fact(DisplayName = nameof(MessageBatching_Works_Timed))]
        public void MessageBatching_Works_Timed() {
            const int MESSAGE_BATCHSIZE = 10000;

            var sut = TestHelper.GetBilgeAndClearDown();

            sut.SetMessageBatching(MESSAGE_BATCHSIZE, 250);

            sut.ActiveTraceLevel = SourceLevels.Information;
            var mmh = new MockMessageHandler();
            sut.AddHandler(mmh);

            sut.Info.Log("Dummy Message");

            var timeSoFar = new Stopwatch();
            timeSoFar.Start();

            bool writesFound = false;

            while (timeSoFar.Elapsed.TotalMilliseconds < 750) {
                // This is not particularly precise because of threading and guarantees so we are using some generous margins for error.
                // With the write time of not less than 250 we shouldnt see any writes for the first 175 MS.  If we do then its a test fail.
                // Similarly if we reach 750 ms and havent seen any writes thats a test fail.

                if (timeSoFar.ElapsedMilliseconds < 175) {
                    Assert.Equal(0, mmh.TotalMessagesRecieved);
                } else {
                    if (mmh.TotalMessagesRecieved > 0) {
                        writesFound = true;
                        break;
                    }
                }
                if (timeSoFar.ElapsedMilliseconds > 350) {
                    sut.Flush().Wait();
                }
            }

            if (!writesFound) {
                throw new InvalidOperationException("The writes never hit the listener");
            }
        }

        [Fact(DisplayName = nameof(Trace_Enter_WritesMethodName))]        
        [Trait(Traits.Age, Traits.Regression)]
        public void Trace_Enter_WritesMethodName() {
            var mkHandler = new MockRouter();
            mkHandler.SetMustContainForBody(nameof(Trace_Enter_WritesMethodName));
            var sut = TestHelper.GetBilgeAndClearDown(mkHandler, sl: SourceLevels.Verbose);

            sut.Info.E();

            // E generates more than one message, therefore we have to check that one of the messages had the name in it.
            mkHandler.AssertAllConditionsMetForAllMessages(true, true);
        }

        [Fact(DisplayName = nameof(Trace_Exit_IncludesMethodName),Skip ="Disabled")]        
        [Trait(Traits.Age, Traits.Regression)]
        public void Trace_Exit_IncludesMethodName() {
            var mkHandler = new MockRouter();
            mkHandler.SetMethodNameMustContain(nameof(Trace_Exit_IncludesMethodName));
            var sut = TestHelper.GetBilgeAndClearDown(mkHandler);
            sut.ActiveTraceLevel = SourceLevels.Verbose;

            sut.Error.X();
            sut.Info.X();
            sut.Verbose.X();

            // X generates more than one message, therefore we have to check that one of the messages had the name in it.
            mkHandler.AssertAllConditionsMetForAllMessages(true, true);
        }

        [Fact(DisplayName = nameof(AddHandler_DuplicateByNameFailsOnSecond),Skip ="Disabled")]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Contexts_AreNotPassedByDefault() {
            _ = TestHelper.GetBilgeAndClearDown();

            Bilge.AddHandler(new MockMessageHandler());



            Assert.Fail();
        }


        [Fact(DisplayName = nameof(AddHandler_DuplicateByNameFailsOnSecond),Skip = "Disabled")]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Contexts_ArePassedWhenRequested() {

            var mkHandler = new MockRouter();
            mkHandler.SetContextMustExistForEveryMessage("testcontext.added");
            var b = TestHelper.GetBilgeAndClearDown(mkHandler);            
            b.ConfigureTrace(new TraceConfiguration() {
                PassContextToHandler = true
            });
            b.AddContext("testcontext.added", "istrue");

            b.Info.Log("Boom!");


            Assert.Fail();
        }

        [Fact(DisplayName = nameof(Trace_Flow_IncludesClassName))]
        [Trait("V", "2")]
        [Trait(Traits.Age, Traits.Regression)]
        public void Trace_Flow_IncludesClassName() {
            var mkHandler = new MockRouter();
            mkHandler.SetClassnameMustBe(nameof(BasicInterfaceTests));
            var sut = TestHelper.GetBilgeAndClearDown(mkHandler);
            sut.ConfigureTrace(new TraceConfiguration() {
                AddClassDetailToTrace = true
            });
            sut.ActiveTraceLevel = SourceLevels.Verbose;

            sut.Info.Flow();
            sut.Error.Flow();
            sut.Verbose.Flow();

            mkHandler.AssertAllConditionsMetForAllMessages(true);
        }


        [Fact(DisplayName = nameof(Trace_Log_IncluesMethodName))]        
        [Trait(Traits.Age, Traits.Regression)]
        public void Trace_Log_IncluesMethodName() {
            var mkHandler = new MockRouter();
            mkHandler.SetMethodNameMustContain(nameof(Trace_Log_IncluesMethodName));            
            var sut = TestHelper.GetBilgeAndClearDown(mkHandler);
            sut.ActiveTraceLevel = SourceLevels.Verbose;

            sut.Info.Log("Hello Cruiel World");
            sut.Info.Log("Hello Cruiel World","You are very cruel");


            mkHandler.AssertAllConditionsMetForAllMessages(true);
        }

        [Fact(DisplayName = nameof(Trace_Flow_IncludesMethodNameInBody))]
        [Trait("V", "2")]
        [Trait(Traits.Age, Traits.Regression)]
        public void Trace_Flow_IncludesMethodNameInBody() {
            var mkHandler = new MockRouter();
            mkHandler.SetMethodNameMustContain(nameof(Trace_Flow_IncludesMethodNameInBody));
            mkHandler.SetMustContainForBody(nameof(Trace_Flow_IncludesMethodNameInBody));
            var sut = TestHelper.GetBilgeAndClearDown(mkHandler);

            sut.ActiveTraceLevel = SourceLevels.Verbose;

            sut.Info.Flow();
            sut.Error.Flow();
            sut.Verbose.Flow();

            mkHandler.AssertAllConditionsMetForAllMessages(true);
        }

        [Fact(DisplayName = nameof(Trace_Log_IncludesClassName))]
        [Trait("V", "2")]
        [Trait(Traits.Age, Traits.Regression)]
        public void Trace_Log_IncludesClassName() {
            var mkHandler = new MockRouter();
            mkHandler.SetClassnameMustBe(nameof(BasicInterfaceTests));
            var sut = TestHelper.GetBilgeAndClearDown(mkHandler);
            sut.ActiveTraceLevel = SourceLevels.Verbose;

            sut.ConfigureTrace(new TraceConfiguration() {
                AddClassDetailToTrace = true
            });

            sut.Info.Log("Hi");
            sut.Error.Log("Hi");
            sut.Verbose.Log("hi");

            mkHandler.AssertAllConditionsMetForAllMessages(true);
        }

        [Theory(DisplayName = nameof(WriteMessage_ArrivesAsMessage))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        [InlineData(TraceCommandTypes.LogMessage)]
        [InlineData(TraceCommandTypes.LogMessageVerb)]
        [InlineData(TraceCommandTypes.ErrorMsg)]
        [InlineData(TraceCommandTypes.WarningMsg)]
        public void WriteMessage_ArrivesAsMessage(TraceCommandTypes testCase) {
            var sut = TestHelper.GetBilgeAndClearDown();
            sut.ActiveTraceLevel = SourceLevels.Verbose;
            var mmh = new MockMessageHandler();
            sut.AddHandler(mmh);

            WriteCorrectTypeOfMessage(sut, testCase);
            sut.Flush().Wait();
            Thread.Sleep(10);

            var msg = mmh.GetMostRecentMessage();

            Assert.NotNull(msg);
            Assert.Equal(testCase, msg.CommandType);
        }

        private void WriteASeriesOfMessages(Bilge b) {
            b.Info.Log("Test message");
            b.Verbose.Log("test message");
            b.Error.Log("Test message");
            b.Warning.Log("Test message");
        }

        private void WriteCorrectTypeOfMessage(Bilge sut, TraceCommandTypes testCase) {
            switch (testCase) {
                case TraceCommandTypes.LogMessage: sut.Info.Log("Test"); break;
                case TraceCommandTypes.LogMessageVerb: sut.Verbose.Log("Test"); break;
                case TraceCommandTypes.TraceMessageIn: sut.Info.EnterSection("Test"); break;
                case TraceCommandTypes.ErrorMsg: sut.Error.Log("Test"); break;
                case TraceCommandTypes.WarningMsg: sut.Warning.Log("Test"); break;

                case TraceCommandTypes.LogMessageMini:
                case TraceCommandTypes.InternalMsg:
                case TraceCommandTypes.TraceMessageOut:
                case TraceCommandTypes.TraceMessage:
                case TraceCommandTypes.AssertionFailed:
                case TraceCommandTypes.MoreInfo:
                case TraceCommandTypes.CommandOnly:
                case TraceCommandTypes.ExceptionBlock:
                case TraceCommandTypes.ExceptionData:
                case TraceCommandTypes.ExcStart:
                case TraceCommandTypes.ExcEnd:
                case TraceCommandTypes.SectionStart:
                case TraceCommandTypes.SectionEnd:
                case TraceCommandTypes.ResourceEat:
                case TraceCommandTypes.ResourcePuke:
                case TraceCommandTypes.ResourceCount:
                case TraceCommandTypes.Standard:
                case TraceCommandTypes.CommandData:
                case TraceCommandTypes.Custom:
                case TraceCommandTypes.Alert:
                case TraceCommandTypes.Unknown:
                default:
                    throw new NotImplementedException();
            }
        }
    }
}