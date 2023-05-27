using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using FluentAssertions;
using Plisky.Diagnostics.Copy;
using Xunit;

namespace Plisky.Diagnostics.Test {
    public class DynamicConfigurationTests {

        [Fact]
        [Trait(Traits.Age, Traits.Regression)]
        public void Context_Logs_WhenNotSet() {
            var mmh = new MockMessageHandler();
            var sut = TestHelper.GetBilgeAndClearDown();
            sut.ActiveTraceLevel = SourceLevels.Verbose;
            sut.AddHandler(mmh);
            sut.SessionContext = "session1";

            sut.Verbose.Log("Hello World");
            sut.Verbose.Log("Hello World 2");

            mmh.TotalMessagesRecieved.Should().Be(2);
        }

        [Theory]
        [Trait(Traits.Age, Traits.Regression)]
        [InlineData("session1", "session1", 2)]
        [InlineData("session1", "SESSION1", 2)]
        [InlineData("SESSION1", "SESSION1", 2)]
        [InlineData("session1", "SESSION2", 0)]
        [InlineData("session1", "session2", 0)]
        public void Context_Logs_WhenNotSet2(string sessionId, string filter, int expectedMessages) {
            var mmh = new MockMessageHandler();
            var sut = TestHelper.GetBilgeAndClearDown();
            sut.ActiveTraceLevel = SourceLevels.Verbose;
            sut.AddHandler(mmh);
            sut.SessionContext = sessionId;
            sut.SetSessionFilter(filter);

            sut.Verbose.Log("Hello World");
            sut.Verbose.Log("Hello World 2");

            mmh.TotalMessagesRecieved.Should().Be(expectedMessages);
        }

        [Fact(DisplayName = nameof(SetsTraceOnExistingInstance))]
        public void SetsTraceOnExistingInstance() {
            var sut = new DynamicTrace();
            var b = sut.CreateBilge("test");
            b.ActiveTraceLevel.Should().Be(SourceLevels.Off);

            sut.SetTraceLevel(SourceLevels.Verbose);

            b.ActiveTraceLevel.Should().Be(SourceLevels.Verbose);
        }

        [Fact(DisplayName = nameof(SetsTraceOnMultipleExistingInstances))]
        public void SetsTraceOnMultipleExistingInstances() {
            var sut = new DynamicTrace();
            var multipleInstances = new List<Bilge>();
            for (int i = 0; i < 10; i++) {
                var b = sut.CreateBilge("test");
                multipleInstances.Add(b);
            }

            sut.SetTraceLevel(SourceLevels.Verbose);

            for (int j = 0; j < 10; j++) {
                multipleInstances[j].ActiveTraceLevel.Should().Be(SourceLevels.Verbose);
            }
        }

        private void SubMethod(DynamicTrace sut) {
            var b = sut.CreateBilge("test");
            b.Info.Log("x");
        }

        [Fact(DisplayName = nameof(DoesNotSetTraceOnExpiredInstances))]
        public void DoesNotSetTraceOnExpiredInstances() {
            var sut = new DynamicTrace();
            SubMethod(sut);
            SubMethod(sut);
            GC.Collect();

            int result = sut.SetTraceLevel(SourceLevels.Verbose);

            result.Should().Be(0);
        }


        [Fact(DisplayName = nameof(SetConfigurationResolveOnExistingInstances))]
        public void SetConfigurationResolveOnExistingInstances() {
            var sut = new DynamicTrace();
            Bilge.ClearConfigurationResolver();

            var b1 = sut.CreateBilge("enabled");
            var b2 = sut.CreateBilge("disabled");

            sut.SetConfigurationResolver("v-enabled");

            b1.ActiveTraceLevel.Should().Be(SourceLevels.Verbose);
            b2.ActiveTraceLevel.Should().Be(SourceLevels.Off);
        }

    }
}
