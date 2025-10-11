
using System.Text;
using Plisky.Diagnostics;

internal class ReallyLongMessages {
    protected Bilge b = new Bilge("ReallyLongMessages");
    public ReallyLongMessages() {
    }

    internal void Go() {
        var sb = new StringBuilder();
        for (int i=0; i<10000; i++) {
            sb.Append($"Long {i}");
        }
        b.Info.Log(sb.ToString());
    }
}