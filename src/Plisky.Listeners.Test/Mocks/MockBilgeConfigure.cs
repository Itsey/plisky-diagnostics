namespace Plisky.Diagnostics.Test {

    public class MockBilgeConfigure : BilgeConfigure {
        private int callOffset;

        public MockBilgeConfigure() {
            StaticFileCallIdx = EnvironmentVariableCallIdx = -1;
        }

        public int EnvironmentVariableCallIdx { get; internal set; }

        public int StaticFileCallIdx { get; internal set; }

        protected override string GetFromEnvironmentVariable(string v) {
            EnvironmentVariableCallIdx = callOffset;
            callOffset++;
            return base.GetFromEnvironmentVariable(v);
        }
    }
}