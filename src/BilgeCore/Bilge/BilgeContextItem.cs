#if NETCOREAPP
using System;

namespace Plisky.Diagnostics;

/// <summary>
/// Used as context for the ILogger begin scope item.
/// </summary>
public class BilgeContextItem : IDisposable {
    private readonly Bilge b;
    private string context;

    /// <summary>
    /// Creates a new instance of the BilgeContextItem passing the context to a new bilge instance.
    /// </summary>
    /// <param name="ctxt">The context</param>
    public BilgeContextItem(string ctxt) {
        b = new Bilge(ctxt);
        b.Info.EnterSection(ctxt);
        context = ctxt;
    }

    /// <summary>
    /// Disposes this context and leaves the current section.
    /// </summary>
    public void Dispose() {
        b.Info.LeaveSection(context);
    }
}
#endif