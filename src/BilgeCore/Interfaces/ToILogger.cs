#if NETCOREAPP
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Plisky.Diagnostics;

namespace Plisky.Diagnostics; 

internal class ILoggerWrapper : ILogger {

    private Bilge b;

    public IDisposable BeginScope<TState>(TState state) where TState : notnull {
        throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel) {




        throw new NotImplementedException();
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
        throw new NotImplementedException();
    }
}
#endif