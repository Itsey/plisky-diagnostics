using Plisky.Diagnostics;

namespace TestClient {
    public class RunLoggingTests {
        public Bilge b = new Bilge();

        public void DoTests() {
            b.Info.Log("Info Log");
            b.Verbose.Log("Verbose Log");
            b.Critical.Log("Critical Log");
            b.Error.Log("Error Log");
            b.Info.Dump(null, "Hello");
            b.Verbose.Dump(null, "Hello");
            b.Critical.Dump(null, "Hello");
            b.Error.Dump(null, "Hello");
            b.Action.Occured("test", "test");
            b.Assert.True(true);
            b.Direct.Write("hello", "further");
            b.Util.Online("here");
        }
    }
}