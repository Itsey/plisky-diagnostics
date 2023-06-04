namespace Plisky.Diagnostics.Test {

    using System;
    using System.Diagnostics;
    using Microsoft.VisualBasic;
    using Plisky.Diagnostics.Copy;
    using Xunit;

    [Collection(nameof(QueueSensitiveTestCollectionDefinition))]
    public class ConfigurationTests {

        [Fact(DisplayName = nameof(BasicConfiguration_StartsEmpty))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void BasicConfiguration_StartsEmpty() {
            var bc = new BilgeConfiguration();

            Assert.Empty(bc.HandlerStrings);
            Assert.Equal(SourceLevels.Off, bc.OverallSourceLevel);
        }

        [Theory(DisplayName = nameof(ConfigResolver_InitStringCreatesResolver))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        [InlineData("", "boblet", SourceLevels.Off)]
        [InlineData("none", "boblet", SourceLevels.Off)]
        [InlineData("off", "boblet", SourceLevels.Off)]
        [InlineData("e-**", "boblet", SourceLevels.Error)]
        [InlineData("e-**;v-boblet", "boblet", SourceLevels.Error)]
        [InlineData("v-boblet;e-**", "boblet", SourceLevels.Verbose)]
        [InlineData("v-bob*", "boblet", SourceLevels.Verbose)]
        [InlineData("w-bob*", "boblet", SourceLevels.Warning)]
        [InlineData("e-bob*", "boblet", SourceLevels.Error)]
        [InlineData("e-bob**", "bob*let", SourceLevels.Error)]
        [InlineData("e-bob", "boblet", SourceLevels.Off)]
        [InlineData("e-bob", "bob", SourceLevels.Error)]
        [InlineData("e-*bob", "monabob", SourceLevels.Error)]
        [InlineData("e-*bob", "monabobx", SourceLevels.Off)]
        [InlineData("e-*bob;v-mik*", "mikado", SourceLevels.Verbose)]
        [InlineData("e-*bob;v-mik*", "applebob", SourceLevels.Error)]
        [InlineData("e-*bob;;;v-mik*;", "applebob", SourceLevels.Error)]
        [InlineData("e-*bob;v-mik*", "monkey", SourceLevels.Off)]
        public void ConfigResolver_InitStringCreatesResolver(string init, string instance, SourceLevels result) {
            var sut = Bilge.SetConfigurationResolver(init);

            var res = sut(instance, SourceLevels.Off);

            Assert.Equal(result, res);
        }

        [Fact(DisplayName = nameof(TraceConfig_Timestamp_StartsOff))]
        [Trait("V", "2")]
        [Trait(Traits.Age, Traits.Regression)]
        public void TraceConfig_Timestamp_StartsOff() {
            var mkHandler = new MockRouter();
            mkHandler.SetTimeStampMustBeNull();
            var sut = TestHelper.GetBilgeAndClearDown(mkHandler);
            sut.ActiveTraceLevel = SourceLevels.Verbose;

            sut.Info.Log("Hi");

            mkHandler.AssertAllConditionsMetForAllMessages(true);
        }

        [Fact(DisplayName = nameof(TraceConfig_TimestampTrue_AddsTimestamp))]
        [Trait("V", "2")]
        [Trait(Traits.Age, Traits.Regression)]
        public void TraceConfig_TimestampTrue_AddsTimestamp() {
            var mkHandler = new MockRouter();
            mkHandler.SetTimestampMustBe(DateAndTime.Now.Subtract(new TimeSpan(0, 2, 0)), DateAndTime.Now.Add(new TimeSpan(0, 2, 0)));
            var sut = TestHelper.GetBilgeAndClearDown(mkHandler);
            sut.ActiveTraceLevel = SourceLevels.Verbose;

            sut.ConfigureTrace(new TraceConfiguration() {
                AddTimestamps = true
            });

            sut.Info.Log("Hi");

            mkHandler.AssertAllConditionsMetForAllMessages(true);
        }
    }
}