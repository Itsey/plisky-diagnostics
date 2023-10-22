#if NETCOREAPP

namespace Plisky.Diagnostics;


using Microsoft.Extensions.Logging;
using Plisky.Diagnostics;

public partial class Bilge {

    /// <summary>
    /// Converts the current instance of Bilge to a class implementing ILogger.
    /// </summary>
    /// <returns></returns>
    public ILogger ToILogger() {
        return new ILoggerWrapper(this);
    }
}

#endif

