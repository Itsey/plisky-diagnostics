namespace Plisky.Diagnostics.Test {
    using Plisky.Diagnostics.Copy;
    using Xunit;

    [Collection(nameof(QueueSensitiveTestCollectionDefinition))]
    public class AlertFeatureTests {

        [Fact(DisplayName = nameof(BasicAlert_GetsWritten_TraceOff))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void BasicAlert_GetsWritten_TraceOff() {
            Bilge sut = TestHelper.GetBilgeAndClearDown();

            var mmh = new MockMessageHandler();
            sut.AddHandler(mmh);

            Bilge.Alert.Online("test-appname");
            sut.Flush();

            Assert.True(mmh.AssertThisMessageMustExist("test-appname"));
        }

    }
}