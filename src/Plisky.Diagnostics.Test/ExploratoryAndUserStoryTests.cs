using System;

namespace Plisky.Diagnostics.Test {

    using System.Diagnostics;
    using FluentAssertions;
    using Microsoft.VisualBasic;
    using Plisky.Diagnostics.Copy;
    using Xunit;

    [Collection(nameof(QueueSensitiveTestCollectionDefinition))]
    public class ExploratoryAndUserStoryTests {

        [Theory(DisplayName = nameof(ErrorReturnsErrorCode))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        [InlineData(0x00010001)]
        [InlineData(0x10011001)]
        [InlineData(55)]
        [InlineData(12345678)]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void ErrorReturnsErrorCode(int hResult) {

            var sut = TestHelper.GetBilgeAndClearDown();
            var result = sut.Error.Record(new ErrorDescription(hResult, "this context"));

            result.Should().Be(hResult);
        }


        [Fact(DisplayName = nameof(Action_CallCount_IncrementsEachTime))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Action_CallCount_IncrementsEachTime() {
            var sut = TestHelper.GetBilgeAndClearDown();

            int lastCallCount = 0;
            _ = sut.Action.RegisterHandler((x) => {
                Assert.True(x.CallCount > lastCallCount);
                lastCallCount = x.CallCount;
            }, "default");

            for (int i = 0; i < 10; i++) {
                sut.Action.Occured("test", "dummy");
            }
        }

        [Fact(DisplayName = nameof(Action_CallCount_StartsAtOne))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Action_CallCount_StartsAtOne() {
            var sut = TestHelper.GetBilgeAndClearDown();
            _ = sut.Action.RegisterHandler((x) => {
                Assert.Equal(1, x.CallCount);
            }, "default");

            sut.Action.Occured("test", "dummy");
        }

        [Fact(DisplayName = nameof(Action_RegisterHandler_Works))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Action_RegisterHandler_Works() {
            var sut = TestHelper.GetBilgeAndClearDown();
            bool res = sut.Action.RegisterHandler((x) => {
            }, "default");

            Assert.True(res);
        }

        [Fact(DisplayName = nameof(Action_RegisterHandlerTwice_Fails))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Action_RegisterHandlerTwice_Fails() {
            var sut = TestHelper.GetBilgeAndClearDown();
            bool res1 = sut.Action.RegisterHandler((x) => {
            }, "default");
            bool res2 = sut.Action.RegisterHandler((x) => {
            }, "default");

            Assert.False(res2);
        }

        [Fact(DisplayName = nameof(Action_RegisterTwoHandlersWithDifferentContext_Succeeds), Skip = "Not Implemented")]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Action_RegisterTwoHandlersWithDifferentContext_Succeeds() {
            var sut = TestHelper.GetBilgeAndClearDown();
            bool res1 = sut.Action.RegisterHandler((x) => {
            }, "default");
            bool res2 = sut.Action.RegisterHandler((x) => {
            }, "default"); // Add some context to filter on

            Assert.True(res2);
        }

        [Fact(DisplayName = nameof(Action_RegisterAnUnregisterHandler_LeavesNone))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Action_RegisterAnUnregisterHandler_LeavesNone() {
            var sut = TestHelper.GetBilgeAndClearDown();
            int actionCount = 0;
            System.Action<IBilgeActionEvent> action = (x) => {
                actionCount++;
            };
            bool res = sut.Action.RegisterHandler(action, "default");
            sut.Action.UnregisterHandler(action, "default");
            sut.Action.Occured("default", string.Empty);
            actionCount.Should().Be(0);
        }

        [Fact(DisplayName = nameof(Action_RightData_SentToHandler))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Action_RightData_SentToHandler() {
            const string ACTIONDATA = "myActionDaTa";
            var sut = TestHelper.GetBilgeAndClearDown();

            _ = sut.Action.RegisterHandler((x) => {
                Assert.Equal(ACTIONDATA, x.Data);
            }, "default");

            sut.Action.Occured("dummy", ACTIONDATA);
        }

        [Fact(DisplayName = nameof(Action_RightName_SentToHandler))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Action_RightName_SentToHandler() {
            const string ACTIONNAME = "myActionName";

            var sut = TestHelper.GetBilgeAndClearDown();

            _ = sut.Action.RegisterHandler((x) => {
                Assert.Equal(ACTIONNAME, x.Name);
            }, "default");

            sut.Action.Occured(ACTIONNAME, "dummy");
        }

        [Fact(DisplayName = nameof(Action_WhenExecuted_WritesToTraceStream))]
        [Trait(Traits.Age, Traits.Regression)]
        public void Action_WhenExecuted_WritesToTraceStream() {
            var mmh = new MockMessageHandler();
            mmh.SetMethodNameMustContain(nameof(Action_WhenExecuted_WritesToTraceStream));
            var sut = TestHelper.GetBilgeAndClearDown();
            sut.ActiveTraceLevel = SourceLevels.Verbose;
            sut.AddHandler(mmh);
            Assert.Equal(0, mmh.TotalMessagesRecieved);
            sut.Action.Occured("test", "test");

            sut.Flush();

            Assert.True(mmh.TotalMessagesRecieved > 0);
        }
    }
}